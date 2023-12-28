using System.Text;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using DioRed.Common.Logging;

Console.OutputEncoding = Encoding.UTF8;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddMurkaBot(context.Configuration);
    })
    .ConfigureLogging(logging => logging.SetupDioRedLogging("Murka"))
    .Build();

host.Services.UseMurkaBot();

Console.WriteLine("Bot is started.\nPress Ctrl+C to stop the bot.");
host.Run();

Console.WriteLine("Bot was stopped.");