using System.Text;

using DioRed.Murka.Core;
using DioRed.Murka.Core.Contracts;
using DioRed.Murka.Core.Entities;
using DioRed.Murka.TelegramBot;

using Microsoft.Extensions.Configuration;

using Telegram.Bot.Types.Enums;

IConfigurationRoot configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets("3371d872-5073-497e-817e-7f06e7a254a9")
    .AddEnvironmentVariables()
    .Build();

Console.OutputEncoding = Encoding.UTF8;

ILogic logic = new Logic(configuration);
CancellationTokenSource cts = new();

MurkaBot bot = new(configuration, logic, cts.Token);

bot.InfoMessage += (_, e) => Console.WriteLine(e.Message);
bot.Error += (_, e) => Console.WriteLine(e.Message);

await bot.ReconnectToChats();

bot.ChatClientAdded += (_, e) =>
{
    var chatInfo = new ChatInfo(
        Id: e.Chat.Id.ToString(),
        Type: "Telegram" + e.Chat.Type,
        Title: e.Chat.Type == ChatType.Private
            ? $"{e.Chat.FirstName} {e.Chat.LastName}".Trim()
            : e.Chat.Title ?? string.Empty);

    logic.AddChat(chatInfo);
};

bot.StartReceiving();

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