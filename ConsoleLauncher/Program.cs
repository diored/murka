using DioRed.Murka.Core.Entities;
using DioRed.Vermilion.Hosting;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Defaults.ConsoleLoggerTimeZone = CommonValues.ServerTimeZoneShift;

IHost host = Host.CreateDefaultBuilder(args)
    .BuildDefaultVermilionHost(
        botName: "Murka",
        configureServices: (context, services) => services.AddMurkaDependencies(context.Configuration),
        assembliesWithHandlers: [typeof(MurkaServicesExtension).Assembly]
    );

host.Services.SetupMurkaJobs();

await host.RunAsync();
return;