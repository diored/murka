using DioRed.Common.AzureStorage;
using DioRed.Murka.Core;
using DioRed.Murka.Core.Modules;
using DioRed.Murka.Infrastructure.AzureStorage;
using DioRed.Murka.Infrastructure.Modules;
using DioRed.Vermilion.Hosting;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) => services
        .AddKeyedSingleton("Murka", new AzureStorageClient(
            AzureStorageSettings.Load(
                context.Configuration.GetRequiredSection("Vermilion:AzureTable")
            )
        ))
        .AddSingleton<DayEventsStorage>()
        .AddSingleton<EventsStorage>()
        .AddSingleton<ImagesStorage>()
        .AddSingleton<LinksStorage>()
        .AddSingleton<ParametersStorage>()
        .AddSingleton<PromocodesStorage>()
        .AddSingleton<IDailyModule, DailyModule>()
        .AddSingleton<IDayEventsModule, DayEventsModule>()
        .AddSingleton<IEventsModule, EventsModule>()
        .AddSingleton<IGreetingModule, GreetingModule>()
        .AddSingleton<ILinksModule, LinksModule>()
        .AddSingleton<INorthModule, NorthModule>()
        .AddSingleton<IPromocodesModule, PromocodesModule>()
        .AddSingleton<ILogic, Logic>()
    )
    .ConfigureVermilion(
        "Murka",
        builder => builder
            .UseAzureTableChatStorage()
            .UseTelegram()
            .LoadCommandHandlersFromAssembly(typeof(Anchor).Assembly)
            .LoadScheduledJobsFromAssembly(typeof(Anchor).Assembly)
    )
    .Build();

await host.RunAsync();