using DioRed.Murka.Launcher;

using Microsoft.Extensions.Configuration;

IConfigurationRoot configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var autoStart = args.Contains("-a");

ApplicationConfiguration.Initialize();
Application.Run(new MainForm(configuration, autoStart));