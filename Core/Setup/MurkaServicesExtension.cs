using DioRed.Api.Client;
using DioRed.Auth.Client;
using DioRed.Murka.Core;
using DioRed.Murka.Core.Entities;
using DioRed.Vermilion;
using DioRed.Vermilion.Interaction.Content;
using DioRed.Vermilion.Interaction.Receivers;

using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

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

    public static IServiceProvider SetupMurkaJobs(
        this IServiceProvider services
    )
    {
        return services.SetupDailyJob(
            async (services, botCore) =>
            {
                ILogic logic = services.GetRequiredService<ILogic>();

                await logic.CleanupAsync();

                await botCore.PostAsync(
                    Receiver.Everyone,
                    async chatId => new HtmlContent
                    {
                        Html = await logic.BuildAgendaAsync(
                            chatId,
                            ServerDateTime.GetCurrent().Date
                        )
                    }
                );
            },
            timeOfDay: TimeOnly.MinValue,
            timeZoneOffset: CommonValues.ServerTimeZoneShift,
            repeatNumber: 0,
            id: "Cleanup and agenda"
        );
    }
}