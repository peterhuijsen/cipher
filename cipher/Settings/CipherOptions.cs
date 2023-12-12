using Microsoft.Extensions.Configuration;

namespace Cipher.Settings;

/// <summary>
/// An options class containing settings for the cipher package.
/// </summary>
public class CipherOptions
{
    /// <summary>
    /// Gets or sets the <see cref="ModeType"/> of the cipher package.
    /// </summary>
    public ModeType Mode { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="EncodingType"/> of the cipher package.
    /// </summary>
    public EncodingType Encoding { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="CipherType"/> of the cipher package.
    /// </summary>
    public CipherType Type { get; set; }
    
    /// <summary>
    /// Gets or sets the path to the input text file for the console application.
    /// </summary>
    public string Input { get; set; } = null!;

    /// <summary>
    /// Gets or sets the alphabet which should be used by the cipher package.
    /// </summary>
    public string Alphabet { get; set; } = null!;
    
    /// <summary>
    /// Get the configuration from an appsettings file.
    /// </summary>
    /// <returns>The options in an <see cref="IConfiguration"/> object.</returns>
    public static IConfiguration Get()
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build()
            .GetRequiredSection("Cipher");

        return builder;
    }
}