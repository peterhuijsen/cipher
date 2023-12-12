using Cipher.Services.General;

namespace Cipher.Services.Ciphers;

/// <summary>
/// A vigenere cipher implementation of the <see cref="ICipherService"/>.
/// </summary>
public class VigenereCipherService : IVigenereCipherService
{
    private readonly ITextEncoderService _textEncoderService;
    
    private readonly string _alphabet;
    private readonly Dictionary<char, int> _map;

    public VigenereCipherService(ITextEncoderService textEncoderService, ICipherSupportService supportService)
    {
        _textEncoderService = textEncoderService;
        
        _alphabet = supportService.GetAlphabet();
        _map = supportService.GetCipherMap(_alphabet);
    }

    /// <inheritdoc cref="ICipherService.Encrypt"/>
    public byte[] Encrypt(byte[] content, byte[] key)
    {
        var result = "";
       
        // Decode the contents of the plaintext and the key.
        var textContent = _textEncoderService.Encode(content);
        var keyContent = _textEncoderService.Encode(key);

        for (var i = 0; i < content.Length; i++)
        {
            // Map the cipher character to an index, and the corresponding key character
            // to an index by using modulo to retrieve the remainder.
            var plaintextChar = _map[textContent[i]];
            var keyChar = _map[keyContent[i % keyContent.Length]];

            // Calculate the difference between the two and reverse the 
            // index of the cipher character to retrieve the plaintext character by shifting
            // forward and prevent overflows.
            var difference = plaintextChar + keyChar;
            var index = difference >= _alphabet.Length ? difference - _alphabet.Length : difference;
            
            result += _alphabet[index];
        }
        
        return _textEncoderService.Decode(result);
    }

    /// <inheritdoc cref="ICipherService.Decrypt"/>
    public byte[] Decrypt(byte[] cipher, byte[] key)
    {
        var result = "";
        
        // Decode the contents of the cipher and the key.
        var cipherContent = _textEncoderService.Encode(cipher);
        var keyContent = _textEncoderService.Encode(key);

        for (var i = 0; i < cipherContent.Length; i++)
        {
            // Map the cipher character to an index, and the corresponding key character
            // to an index by using modulo to retrieve the remainder.
            var cipherChar = _map[cipherContent[i]];
            var keyChar = _map[keyContent[i % keyContent.Length]];

            // Calculate the difference between the two and reverse the 
            // index of the cipher character to retrieve the plaintext character by shifting
            // back.
            var difference = cipherChar - keyChar;
            var index = difference < 0 ? _alphabet.Length + difference : difference;
            
            result += _alphabet[index];
        }
        
        // Decode the resulting text for the final data result.
        return _textEncoderService.Decode(result);
    }
}