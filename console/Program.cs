using cipher.Extensions;
using Cipher.Services;
using Cipher.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

var builder = new ServiceCollection();

// Add settings.
builder.AddOptions<CipherOptions>().Bind(CipherOptions.Get());

// Add cipher services.
builder.AddEncoderServices();
builder.AddCipherServices();

// Build application.
var serviceProvider = builder.BuildServiceProvider();

// Retrieve encoders.
var encoderService = serviceProvider.GetRequiredService<IEncoderService>();
var textEncoderService = serviceProvider.GetRequiredService<ITextEncoderService>();

// Retrieve matching cipher service and settings.
var cipherService = serviceProvider.GetRequiredService<ICipherService>();
var cipherOptions = serviceProvider.GetRequiredService<IOptions<CipherOptions>>();

// Retrieve input text and key.
Console.Write("Enter your symmetric key: ");
var key = textEncoderService.Decode(Console.ReadLine()!);
var input = encoderService.Decode(File.ReadAllText(cipherOptions.Value.Input));

// Transform the data.
var result = cipherOptions.Value.Mode == ModeType.Decrypt
    ? cipherService.Decrypt(input, key)
    : cipherService.Encrypt(input, key);

// Encode the result.
var text = textEncoderService.Encode(result);

// Write the result.
Console.WriteLine($"Result: {text}");