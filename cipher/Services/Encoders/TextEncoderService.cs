using System.Text;

namespace Cipher.Services.Encoders;

/// <summary>
/// A text implementation of the <see cref="IEncoderService"/>, where bytes are converted to UTF8 text, and vice versa.
/// </summary>
public class TextEncoderService : ITextEncoderService
{
    /// <inheritdoc cref="IEncoderService.Encode"/>
    public string Encode(byte[] data) => Encoding.UTF8.GetString(data);

    /// <inheritdoc cref="IEncoderService.Decode"/>
    public byte[] Decode(string data) => Encoding.UTF8.GetBytes(data);
}