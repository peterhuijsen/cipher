using Cipher.Services;
using Cipher.Services.Ciphers;
using Cipher.Services.Encoders;
using Cipher.Services.General;
using Cipher.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace cipher.Extensions;

/// <summary>
/// An extensions class used to inject services related to this cipher package
/// to a DI container.
/// </summary>
public static class CipherExtensions
{
    public static IServiceCollection AddCipherServices(this IServiceCollection services)
    {
        // Services guard
        ArgumentNullException.ThrowIfNull(services);

        var provider = services.BuildServiceProvider();
        var configuration = provider.GetRequiredService<IOptions<CipherOptions>>();

        services.AddSingleton<IVigenereCipherService, VigenereCipherService>();
        services.AddSingleton<ICaesarCipherService, CaesarCipherService>();
        
        services.AddSingleton<ICipherSupportService, CipherSupportService>();
        services.AddSingleton(
            serviceType: typeof(ICipherService),
            implementationType: ICipherService.From(configuration.Value.Type)
        );
        
        return services;
    }

    public static IServiceCollection AddEncoderServices(this IServiceCollection services)
    {
        // Services guard
        ArgumentNullException.ThrowIfNull(services);
        
        var provider = services.BuildServiceProvider();
        var configuration = provider.GetRequiredService<IOptions<CipherOptions>>();

        services.AddSingleton<IBase64EncoderService, Base64EncoderService>();
        services.AddSingleton<ITextEncoderService, TextEncoderService>();
        
        services.AddSingleton(
            serviceType: typeof(IEncoderService),
            implementationType: IEncoderService.From(configuration.Value.Encoding)
        );

        return services;
    }
}