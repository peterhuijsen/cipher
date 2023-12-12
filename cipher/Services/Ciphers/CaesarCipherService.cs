using Cipher.Services.General;

namespace Cipher.Services.Ciphers;

/// <summary>
/// A caesar cipher implementation of the <see cref="ICipherService"/>.
/// </summary>
public class CaesarCipherService : ICaesarCipherService
{
    private readonly ITextEncoderService _textEncoderService;

    private readonly string _alphabet;
    private readonly Dictionary<char, int> _map;

    public CaesarCipherService(ITextEncoderService textEncoderService, ICipherSupportService supportService)
    {
        _textEncoderService = textEncoderService;
        
        _alphabet = supportService.GetAlphabet();
        _map = supportService.GetCipherMap(_alphabet);
    }

    /// <inheritdoc cref="ICipherService.Encrypt"/>
    public byte[] Encrypt(byte[] content, byte[] key)
    {
        var keyContent = _textEncoderService.Encode(key);
        if (!int.TryParse(keyContent, out var shift))
            throw new ApplicationException("Unable to apply Caesar cipher with an invalid key; it needs to be a number.");
        
        var result = "";
        var textContent = _textEncoderService.Encode(content);

        foreach (var plaintextChar in textContent)
        {
            var difference = _map[plaintextChar] + (shift % _alphabet.Length);
            
            var index = difference >= _alphabet.Length ? difference - _alphabet.Length : difference;
            result += _alphabet[index];
        }

        return _textEncoderService.Decode(result);
    }

    /// <inheritdoc cref="ICipherService.Decrypt"/>
    public byte[] Decrypt(byte[] content, byte[] key)
    {
        var keyContent = _textEncoderService.Encode(key);
        if (!int.TryParse(keyContent, out var shift))
            throw new ApplicationException("Unable to apply Caesar cipher with an invalid key; it needs to be a number.");

        var result = "";
        var textContent = _textEncoderService.Encode(content);

        foreach (var cipherChar in textContent)
        {
            var difference = _map[cipherChar] - (shift % _alphabet.Length);
            var index = difference < 0 ? _alphabet.Length + difference : difference;
            
            result += _alphabet[index];
        }
        
        return _textEncoderService.Decode(result);
    }
}