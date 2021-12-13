using DioRed.Murka.Core;
using DioRed.Murka.TelegramBot;
using DioRed.TelegramBot;

using Microsoft.Extensions.Configuration;

using Telegram.Bot;

IConfigurationRoot configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets("3371d872-5073-497e-817e-7f06e7a254a9")
    .AddEnvironmentVariables()
    .Build();

ApiDataSource dataSource = new(new Uri(configuration["api"]));
using CancellationTokenSource cts = new();

TelegramBotClient bot = new(configuration["token"]);

var updateHandler = new BotUpdateHandler(chat =>
{
    MurkaChatClient chatClient = new(chat, dataSource);
    chatClient.ConfigureJobs(bot, cts.Token);
    return chatClient;
});

bot.StartReceiving(updateHandler, cancellationToken: cts.Token);

Console.WriteLine("Bot is started.\nPress Ctrl+C to stop the bot.");

Console.CancelKeyPress += (_, _) =>
{
    cts.Cancel();
    Console.WriteLine("Bot was stopped.");
    Environment.Exit(0);
};

await Task.Run(async () =>
{
    while (!cts.IsCancellationRequested)
    {
        await Task.Delay(TimeSpan.FromMinutes(1));
    }
}, cts.Token);