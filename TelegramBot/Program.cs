using System.Text;

using DioRed.Murka.Core;
using DioRed.Murka.Core.Contracts;
using DioRed.Murka.TelegramBot;
using DioRed.Murka.TelegramBot.Configuration;
using DioRed.Vermilion;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Console.OutputEncoding = Encoding.UTF8;

MurkaConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets("3371d872-5073-497e-817e-7f06e7a254a9")
    .AddEnvironmentVariables()
    .Build()
    .Get<MurkaConfiguration>();

CancellationTokenSource cts = new();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        services.AddSingleton(configuration);
        services.AddSingleton<IStorageEndpoint>(new StorageEndpoint(configuration.Azure.Account, configuration.Azure.Key));
        services.AddSingleton<ILogic, Logic>();
        services.AddSingleton<ILogger, ConsoleLogger>();
        services.AddTransient<CancellationTokenSource>(_ => new CancellationTokenSource());
        services.AddSingleton<MurkaBot>();
    })
    .Build();

host.Services.GetRequiredService<MurkaBot>().StartReceiving();

Console.WriteLine("Bot is started.\nPress Ctrl+C to stop the bot.");
host.Run();

cts.Cancel();
Console.WriteLine("Bot was stopped.");

//MurkaConfiguration configuration = configurationRoot.Get<MurkaConfiguration>();

//ILogic logic = new Logic(configuration.Azure);

//MurkaBot bot = new(configuration.BotToken, configuration.AdminId, logic, cts.Token);
//bot.AddLogger(new ConsoleLogger());
//bot.StartReceiving();

//Console.WriteLine("Bot is started.\nPress Ctrl+C to stop the bot.");

//Console.CancelKeyPress += (_, _) =>
//{
//    cts.Cancel();
//    Console.WriteLine("Bot was stopped.");
//    Environment.Exit(0);
//};

//await Task.Run(async () =>
//{
//    while (!cts.IsCancellationRequested)
//    {
//        await Task.Delay(TimeSpan.FromMinutes(1));
//    }
//}, cts.Token);