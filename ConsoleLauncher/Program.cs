using DioRed.Vermilion.Hosting;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
    .BuildDefaultVermilionHost(
        botName: "Murka",
        configureServices: (context, services) => services.AddMurkaDependencies(context.Configuration),
        assembliesWithHandlers: [typeof(MurkaServicesExtension).Assembly]
    );

host.Services.SetupMurkaJobs();

await host.RunAsync();