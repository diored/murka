using DioRed.Murka.Core;
using DioRed.Murka.Core.Setup;
using DioRed.Vermilion.Hosting;

using Microsoft.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) => services.AddMurkaDependencies(context.Configuration))
    .ConfigureVermilion(
        "Murka",
        builder => builder
            .ConfigureChatStorage(c => c.UseAzureTable())
            .ConfigureConnectors(c => c.AddTelegram())
            .ConfigureCommandHandlers(c => c.LoadFromAssembly(typeof(Anchor).Assembly))
            .ConfigureDailyJobs(c => c.LoadFromAssembly(typeof(Anchor).Assembly))
    )
    .Build();

await host.RunAsync();