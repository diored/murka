using System.Text;

using DioRed.Murka.TelegramBot;

using Microsoft.Extensions.Configuration;

IConfigurationRoot configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets("3371d872-5073-497e-817e-7f06e7a254a9")
    .AddEnvironmentVariables()
    .Build();

Console.OutputEncoding = Encoding.UTF8;

MurkaBot bot = new(configuration);
using CancellationTokenSource cts = new();

bot.InfoMessage += (_, e) => Console.WriteLine(e.Message);
bot.Error += (_, e) => Console.WriteLine(e.Message);

await bot.StartAsync(cts.Token);

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