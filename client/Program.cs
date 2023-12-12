using cipher.Extensions;
using Cipher.Services;
using Cipher.Settings;
using client.Models;
using client.Services;
using Flurl;
using Flurl.Http;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add settings.
builder.Services.Configure<CipherOptions>(builder.Configuration.GetRequiredSection("Cipher"));

// Add cipher services.
builder.Services.AddEncoderServices();
builder.Services.AddCipherServices();

// Add custom services.
builder.Services.AddSingleton<IDiffieHellmanService, DiffieHellmanService>();
builder.Services.AddSingleton<ISessionStore, SessionStore>();

// Add logging.
builder.Services.AddLogging();

var app = builder.Build();

app.MapGet("/keys/public", ([FromServices] IDiffieHellmanService diffieHellman) => diffieHellman.GetPublicKey());
app.MapPost("/session/initialize", async (
    [FromServices] IDiffieHellmanService diffieHellmanService, 
    [FromServices] IBase64EncoderService base64EncoderService, 
    [FromServices] ITextEncoderService textEncoderService, 
    [FromServices] ISessionStore sessionStore,
    [FromServices] ILogger<Program> logger,
    [FromQuery] string key,
    [FromQuery] string target,
    [FromQuery] CipherType type,
    HttpContext context) =>
{
    // Retrieve the public key from the target.
    var publicKeyResponse = await $"http://{target}/keys/public".GetStringAsync();
    var publicKey = diffieHellmanService.ImportPublicKey(publicKeyResponse);

    // Decode en encrypt the given symmetric key.
    var keyData = textEncoderService.Decode(key);
    var keyCipherData = diffieHellmanService.Encrypt(publicKey, keyData, out var keyIvData);

    // Register the session.
    var session = sessionStore.Register(type, keyData);
    
    // Initialize the session with the target client by sending the symmetric key, encrypted.
    await $"http://{target}/session/verify"
        .SetQueryParam("sender", context.Request.Host)
        .SetQueryParam("cipher", base64EncoderService.Encode(keyCipherData))
        .SetQueryParam("iv", base64EncoderService.Encode(keyIvData))
        .SetQueryParam("type", type)
        .SetQueryParam("session", session)
        .PostAsync();

    // Log the session initialization.
    logger.Log(
        logLevel: LogLevel.Information,
        message: "Initialized session '{session}' with '{target}' (with key '{key}' of cipher '{cipher}').", 
        session, target, key, type.ToString().ToLower()
    );
    
    return new SessionInitializedResponse(session);
});

app.MapPost("/session/verify", async (
    [FromServices] IDiffieHellmanService diffieHellmanService,
    [FromServices] IBase64EncoderService base64EncoderService, 
    [FromServices] ITextEncoderService textEncoderService, 
    [FromServices] ISessionStore sessionStore,
    [FromServices] ILogger<Program> logger,
    [FromQuery] string sender,
    [FromQuery] string cipher,
    [FromQuery] string iv,
    [FromQuery] CipherType type,
    [FromQuery] Guid session) =>
{
    // Retrieve the public key from the sender.
    var publicKeyData = await $"http://{sender}/keys/public".GetStringAsync();
    var publicKey = diffieHellmanService.ImportPublicKey(publicKeyData);
    
    // Decode the ciper and initialization vector from base64.
    var cipherData = base64EncoderService.Decode(cipher);
    var ivData = base64EncoderService.Decode(iv);
    
    // Decrypt the ciper with the key pair and the given initialization vector.
    var keyData = diffieHellmanService.Decrypt(publicKey, cipherData, ivData);
    
    // Register the session.
    sessionStore.Register(session, type, keyData);

    // Log the session initialization.
    logger.Log(
        logLevel: LogLevel.Information,
        message: "Initialized session '{session}' with '{sender}' (with key '{key}' of cipher '{cipher}').", 
        session, sender, textEncoderService.Encode(keyData), type.ToString().ToLower()
    );
    
    return new SessionInitializedResponse(session);
});

app.MapPost("/session/send", async (
    [FromServices] IBase64EncoderService base64EncoderService, 
    [FromServices] ITextEncoderService textEncoderService, 
    [FromServices] IVigenereCipherService vigenereCipherService,
    [FromServices] ICaesarCipherService caesarCipherService,
    [FromServices] ISessionStore sessionStore,
    [FromServices] ILogger<Program> logger,
    [FromQuery] string message,
    [FromQuery] string target,
    [FromQuery] Guid session,
    HttpContext context) =>
{
    // Decode encoded cipher data from other client, from plaintext.
    var messageData = textEncoderService.Decode(message);
    
    // Retrieve the stored key for the given session.
    var current = sessionStore.Retrieve(session);
    if (current is null)
        throw new NullReferenceException("The given session has not been registered at the current client.");

    // Encrypt the data with the stored key and send the message.
    var cipherData = current.Cipher == CipherType.Caesar
        ? caesarCipherService.Encrypt(messageData, current.Key)
        : vigenereCipherService.Encrypt(messageData, current.Key);
    
    await $"http://{target}/session/receive"
        .SetQueryParam("sender", context.Request.Host)
        .SetQueryParam("cipher", base64EncoderService.Encode(cipherData))
        .SetQueryParam("session", session)
        .PostAsync();
    
    // Log the message to the console.
    logger.Log(
        logLevel: LogLevel.Information,
        message: "Sent a message of '{message}' to '{target}' in {cipher} session '{session}'.", 
        message, target, current.Cipher.ToString().ToLower(), session
    );
    
    return new SessionTransmittedResponse(session, message, target);
});

app.MapPost("/session/receive", (
    [FromServices] IBase64EncoderService base64EncoderService, 
    [FromServices] ITextEncoderService textEncoderService, 
    [FromServices] IVigenereCipherService vigenereCipherService,
    [FromServices] ICaesarCipherService caesarCipherService,
    [FromServices] ISessionStore sessionStore,
    [FromServices] ILogger<Program> logger,
    [FromQuery] string sender,
    [FromQuery] string cipher,
    [FromQuery] Guid session) =>
{
    // Decode encoded cipher data from other client, from base64.
    var cipherData = base64EncoderService.Decode(cipher);
    
    // Retrieve the stored key for the given session.
    var current = sessionStore.Retrieve(session);
    if (current is null)
        throw new NullReferenceException("The given session has not been registered at the current client.");
    
    // Decrypt the sent data with the stored key and retrieve the sent message in plaintext.
    var messageData = current.Cipher == CipherType.Caesar
        ? caesarCipherService.Decrypt(cipherData, current.Key)
        : vigenereCipherService.Decrypt(cipherData, current.Key);
    
    var message = textEncoderService.Encode(messageData);
    
    // Log the message to the console.
    logger.Log(
        logLevel: LogLevel.Information,
        message: "Received a message of '{message}' from '{sender}' in {cipher} session '{session}' (key of '{key}').", 
        message, sender, current.Cipher.ToString().ToLower(), session, textEncoderService.Encode(current.Key)
    );
});

// Run the web application.
app.Run();