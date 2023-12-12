using Cipher.Services.Ciphers;
using Cipher.Settings;

namespace Cipher.Services;

/// <summary>
/// An interface for a service used to encrypt and decrypt data with a symmetric key.
/// </summary>
public interface ICipherService
{
    /// <summary>
    /// Encrypt the given content with the given key.
    /// </summary>
    /// <param name="content">The content data which should be encrypted.</param>
    /// <param name="key">The key by which the content data should be encrypted and transformed.</param>
    /// <returns>The encrypted data, the ciphertext.</returns>
    byte[] Encrypt(byte[] content, byte[] key);

    /// <summary>
    /// Decrypt the given cipher with the given key.
    /// </summary>
    /// <param name="cipher">The cipher data which should be decrypted.</param>
    /// <param name="key">The key by which the content data should be encrypted and transformed.</param>
    /// <returns>The decrypted data, the plaintext.</returns>
    byte[] Decrypt(byte[] cipher, byte[] key);

    /// <summary>
    /// Retrieve the type of cipher service corresponding to the given <see cref="CipherType"/>.
    /// </summary>
    /// <param name="type">The type of cipher which should be used.</param>
    /// <returns>The type of cipher service corresponding to the given <see cref="CipherType"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the given <see cref="CipherType"/> is not supported.</exception>
    public static Type From(CipherType type)
        => type switch
        {
            CipherType.Caesar => typeof(CaesarCipherService),
            CipherType.Vigenere => typeof(VigenereCipherService),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
}

public interface ICaesarCipherService : ICipherService { }
public interface IVigenereCipherService : ICipherService { }