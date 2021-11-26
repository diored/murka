using DioRed.Murka.Launcher;

using Microsoft.Extensions.Configuration;

IConfigurationRoot configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

ApplicationConfiguration.Initialize();
Application.Run(new MainForm(configuration));