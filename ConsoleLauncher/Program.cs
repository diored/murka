using System.Text;

using DioRed.Murka.TelegramBot;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Console.OutputEncoding = Encoding.UTF8;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddMurkaBot(context.Configuration);
    })
    .Build();

var bot = host.Services.GetRequiredService<MurkaBot>();
bot.StartReceiving();

Console.WriteLine("Bot is started.\nPress Ctrl+C to stop the bot.");
host.Run();

Console.WriteLine("Bot was stopped.");