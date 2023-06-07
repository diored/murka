using DioRed.Auth.Client;
using DioRed.Murka.Core;
using DioRed.Murka.TelegramBot.Configuration;
using DioRed.Vermilion;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DioRed.Murka.TelegramBot;

public static class ServicesExtension
{
    public static IServiceCollection AddMurkaBot(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration.Get<MurkaConfiguration>()!);
        services.AddSingleton<ApiClient>();
        services.AddSingleton<ILogic, Logic>();

        var authConfiguration = AuthClientConfiguration.Load(configuration.GetSection("auth"));
        var authClient = new AuthClient(authConfiguration);

        services.AddSingleton(new ApiSettings
        {
            Uri = configuration["apiUri"]!,
            GetAccessToken = authClient.GetAccessToken
        });
        services.AddSingleton<ILogger, ConsoleLogger>();
        services.AddTransient<CancellationTokenSource>();

        services.AddSingleton<MurkaBot>();
        
        return services;
    }
}
