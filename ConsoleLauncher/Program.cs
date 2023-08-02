using System.Text;

using DioRed.Murka.Core;

using Microsoft.Extensions.Hosting;

Console.OutputEncoding = Encoding.UTF8;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddMurkaBot(context.Configuration);
    })
    .Build();

host.UseMurkaBot();

Console.WriteLine("Bot is started.\nPress Ctrl+C to stop the bot.");
host.Run();

Console.WriteLine("Bot was stopped.");