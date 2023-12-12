using Cipher.Services.Encoders;
using Cipher.Settings;

namespace Cipher.Services;

/// <summary>
/// A service used to encode and decode data.
/// </summary>
public interface IEncoderService
{
    /// <summary>
    /// Encode a set of data to a string.
    /// </summary>
    /// <param name="data">The data which should be encoded.</param>
    /// <returns>The encoded string of the data.</returns>
    string Encode(byte[] data);
    
    /// <summary>
    /// Decode some text to a set of data.
    /// </summary>
    /// <param name="data">The text which should be decoded.</param>
    /// <returns>The decoded set of data of the string.</returns>
    byte[] Decode(string data);
    
    /// <summary>
    /// Retrieve the type of encoder corresponding to the given <see cref="EncodingType"/>.
    /// </summary>
    /// <param name="type">The type of encoding which should be used.</param>
    /// <returns>The type of encoder corresponding to the given <see cref="EncodingType"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the given <see cref="EncodingType"/> is not supported.</exception>
    public static Type From(EncodingType type)
        => type switch
        {
            EncodingType.Base64 => typeof(Base64EncoderService),
            EncodingType.Text => typeof(TextEncoderService),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
}


public interface IBase64EncoderService : IEncoderService { }
public interface ITextEncoderService : IEncoderService { }
