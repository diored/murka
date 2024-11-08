using DioRed.Murka.Core;
using DioRed.Murka.Core.Setup;
using DioRed.Vermilion.Hosting;

using Microsoft.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
    .BuildDefaultVermilionHost(
        botName: "Murka",
        configureServices: (context, services) => services.AddMurkaDependencies(context.Configuration),
        assemblies: [typeof(Anchor).Assembly]
    );

await host.RunAsync();