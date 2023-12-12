using Cipher.Settings;
using Microsoft.Extensions.Options;

namespace Cipher.Services.General;

/// <summary>
/// An interface for a support service for the <see cref="ICipherService"/> implementations.
/// </summary>
public interface ICipherSupportService
{
    /// <summary>
    /// Get the supported alphabet which should be used for encrypting and decrypting. 
    /// </summary>
    /// <returns>The alphabet which should be used.</returns>
    string GetAlphabet();

    /// <summary>
    /// Generate a map from an alphabet which maps a character to an index.
    /// </summary>
    /// <param name="alphabet">The alphabet which should be mapped.</param>
    /// <returns>The map of the alphabet to a list of indices.</returns>
    Dictionary<char, int> GetCipherMap(string alphabet);
}

/// <summary>
/// A basic implementation of the <see cref="ICipherSupportService"/>.
/// </summary>
public class CipherSupportService : ICipherSupportService
{
    private readonly CipherOptions _cipherOptions;

    public CipherSupportService(IOptions<CipherOptions> cipherOptions)
    {
        _cipherOptions = cipherOptions.Value;
    }

    /// <inheritdoc cref="ICipherSupportService.GetAlphabet"/>
    public string GetAlphabet()
        => _cipherOptions.Alphabet;

    /// <inheritdoc cref="ICipherSupportService.GetCipherMap"/>
    public Dictionary<char, int> GetCipherMap(string alphabet)
        => alphabet.ToDictionary(a => a, alphabet.IndexOf);
}