using DioRed.Api.Client;
using DioRed.Auth.Client;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DioRed.Murka.Core.Setup;

public static class MurkaServicesExtension
{
    public static IServiceCollection AddMurkaDependencies(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services
            .AddDioRedApiRequestBuilder(settings =>
            {
                settings.BaseAddress = configuration["apiUri"]!;
                settings.AccessTokenProvider = AccessTokenProvider.Create(
                    AuthClientConfiguration.Load(
                        configuration.GetRequiredSection("auth")
                    )
                );
            })
            .AddSingleton<IApiFacade, ApiFacade>()
            .AddSingleton<ILogic, Logic>();
    }
}