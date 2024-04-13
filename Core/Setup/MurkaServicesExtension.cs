using DioRed.Api.Client;
using DioRed.Auth.Client;
using DioRed.Common.Jobs;
using DioRed.Murka.Core;
using DioRed.Murka.Core.Entities;
using DioRed.Vermilion;
using DioRed.Vermilion.Interaction.Content;
using DioRed.Vermilion.Interaction.Receivers;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
        ILogic logic = services.GetRequiredService<ILogic>();

        BotCore botCore = services.GetServices<IHostedService>()
            .OfType<BotCore>()
            .Single();

        ILogger<BotCore> logger = services.GetRequiredService<ILogger<BotCore>>();

        var job = Job.SetupDaily(
            async () =>
            {
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
            TimeOnly.FromDateTime(DateTime.Now.AddSeconds(10)),
            CommonValues.ServerTimeZoneShift,
            "Cleanup and agenda"
        );

        job.LogInfo += (_, message) => logger.LogInformation(
            "Job {JobId} returns message: {Message}",
            job.Id,
            message);

        job.Start();

        return services;
    }
}