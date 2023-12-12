using System.Security.Cryptography;
using Cipher.Services;

namespace client.Services;

/// <summary>
/// An interface for a service which implements the Diffie-Hellman key exchange
/// and which uses it to transfer data by its public-private key encryption.
/// </summary>
public interface IDiffieHellmanService
{
    /// <summary>
    /// Retrieve the public key from the keypair.
    /// </summary>
    /// <returns>The encoded public key of the service.</returns>
    string GetPublicKey();

    /// <summary>
    /// Import and decode a public key from an encoded string of text.
    /// </summary>
    /// <param name="key">The encoded public key which needs to be decoded.</param>
    /// <returns>The decoded public key.</returns>
    ECDiffieHellmanPublicKey ImportPublicKey(string key);
    
    /// <summary>
    /// Encrypt data by the public key of the other client.
    /// </summary>
    /// <param name="key">The public key by which the message should be encrypted.</param>
    /// <param name="message">The message which should be encrypted.</param>
    /// <param name="iv">The initialization vectors used to encrypt the message.</param>
    /// <returns>The encrypted plaintext, the ciphertext.</returns>
    byte[] Encrypt(ECDiffieHellmanPublicKey key, byte[] message, out byte[] iv);
    
    /// <summary>
    /// Decrypt data encrypted by the given public key of the other client.
    /// </summary>
    /// <param name="key">The public key by which the message should be decrypted.</param>
    /// <param name="cipher">The cipher which should be decrypted.</param>
    /// <param name="iv">The initialization vectors used to encrypt the message.</param>
    /// <returns>The decrypted ciphertext, the plaintext.</returns>
    byte[] Decrypt(ECDiffieHellmanPublicKey key, byte[] cipher, byte[] iv);
}

/// <summary>
/// An implementation of the <see cref="IDiffieHellmanService"/>.
/// </summary>
public class DiffieHellmanService : IDiffieHellmanService
{
    private readonly IEncoderService _encoderService;
    private readonly ECParameters _parameters;
    
    public DiffieHellmanService(IEncoderService encoderService)
    {
        _encoderService = encoderService;
        
        using var diffie = ECDiffieHellman.Create();
        _parameters = diffie.ExportParameters(includePrivateParameters: true);
    }
    
    /// <inheritdoc cref="IDiffieHellmanService.GetPublicKey"/>
    public string GetPublicKey()
    {
        using var diffie = ECDiffieHellman.Create();
        diffie.ImportParameters(_parameters);

        return _encoderService.Encode(diffie.PublicKey.ExportSubjectPublicKeyInfo());
    }

    /// <inheritdoc cref="IDiffieHellmanService.ImportPublicKey"/>
    public ECDiffieHellmanPublicKey ImportPublicKey(string key)
    {
        using var diffie = ECDiffieHellman.Create();
        var data = _encoderService.Decode(key);
        diffie.ImportSubjectPublicKeyInfo(data, out _);
        
        return diffie.PublicKey;
    }

    /// <inheritdoc cref="IDiffieHellmanService.Decrypt"/>
    public byte[] Decrypt(ECDiffieHellmanPublicKey key, byte[] cipher, byte[] iv)
    {
        using var diffie = ECDiffieHellman.Create();
        diffie.ImportParameters(_parameters);
        var keyData = diffie.DeriveKeyMaterial(key);

        using Aes aes = Aes.Create();
        aes.Key = keyData;
        aes.IV = iv;
        
        using var plainText = new MemoryStream();
        using var cryptoStream = new CryptoStream(plainText, aes.CreateDecryptor(), CryptoStreamMode.Write);
        
        cryptoStream.Write(cipher, 0, cipher.Length);
        cryptoStream.Close();

        return plainText.ToArray();
    }

    /// <inheritdoc cref="IDiffieHellmanService.Encrypt"/>
    public byte[] Encrypt(ECDiffieHellmanPublicKey key, byte[] message, out byte[] iv)
    {
        using var diffie = ECDiffieHellman.Create();
        diffie.ImportParameters(_parameters);
        var keyData = diffie.DeriveKeyMaterial(key);

        using var aes = Aes.Create();
        aes.Key = keyData;
        iv = aes.IV;

        using var ciphertext = new MemoryStream();
        using var cryptoStream = new CryptoStream(ciphertext, aes.CreateEncryptor(), CryptoStreamMode.Write);
        
        cryptoStream.Write(message, 0, message.Length);
        cryptoStream.Close();
                
        return ciphertext.ToArray();
    }
}