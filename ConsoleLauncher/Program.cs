using DioRed.Murka.Core.Setup;
using DioRed.Vermilion.Hosting;

using Microsoft.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
    .BuildDefaultVermilionHost(
        botName: "Murka",
        configureServices: (context, services) => services.AddMurkaDependencies(context.Configuration),
        assemblies: [AppDomain.CurrentDomain.GetAssemblies().First(a => a.FullName?.StartsWith("Murka.Core,") == true)]
    );

await host.RunAsync();