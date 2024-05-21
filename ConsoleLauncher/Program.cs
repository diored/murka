using DioRed.Common.AzureStorage;
using DioRed.Common.Logging;
using DioRed.Murka.Core.Entities;
using DioRed.Vermilion;
using DioRed.Vermilion.ChatStorage;
using DioRed.Vermilion.Subsystems.Telegram;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging => logging.UseDioRedLogging(
        "Murka",
        options =>
        {
            options.EventColors.Add(Events.JobStarted, "mediumspringgreen");
            options.EventColors.Add(Events.JobFinished, "mediumspringgreen");
            options.EventColors.Add(Events.JobScheduled, "mediumspringgreen");

            options.DateTimeOffset = CommonValues.ServerTimeZoneShift;
        }
    ))
    .ConfigureServices((context, services) => services
        .AddMurkaDependencies(context.Configuration)
        .AddVermilion(builder => builder
            .UseAzureTableChatStorage(AzureStorageSettings.MicrosoftAzure(
                accountName: ReadRequired(context.Configuration, "Vermilion:AzureTable:AccountName"),
                accountKey: ReadRequired(context.Configuration, "Vermilion:AzureTable:AccountKey")
            ))
            .AddCommandHandlersFromAssembly(typeof(MurkaServicesExtension).Assembly)
            .AddTelegram()
        )
    )
    .Build();

host.Services.SetupMurkaJobs();

await host.RunAsync();
return;

static string ReadRequired(
    IConfiguration configuration,
    string keyName
)
{
    return configuration[keyName] ?? throw new InvalidOperationException($"""Cannot read "{keyName}" value""");
}