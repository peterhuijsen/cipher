namespace Cipher.Services.Encoders;

/// <summary>
/// A base64url implementation of the &lt;see cref="IEncoderService"/&gt;, where bytes are converted to base64url, and vice versa.
/// </summary>
public class Base64EncoderService : IBase64EncoderService
{
    /// <inheritdoc cref="IEncoderService.Encode"/>
    public string Encode(byte[] data)
        => Convert.ToBase64String(data)
            .Split('=')[0]
            .Replace('+', '-')
            .Replace('/', '_');

    /// <inheritdoc cref="IEncoderService.Decode"/>
    public byte[] Decode(string data)
        => Convert.FromBase64String(
            data.Replace('-', '+')
                .Replace('_', '/')
                .PadRight(data.Length + ((data.Length % 4) switch { 2 => 2, 3 => 1, _ => 0 }), '=')
        );
}