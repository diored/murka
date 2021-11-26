using DioRed.Murka.Core;
using DioRed.Murka.TelegramBot;

using Microsoft.Extensions.Configuration;

using Telegram.Bot;

IConfigurationRoot _configuration = new ConfigurationBuilder()
	.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
	.AddUserSecrets("3371d872-5073-497e-817e-7f06e7a254a9")
	.AddEnvironmentVariables()
	.Build();

string token = _configuration["token"];
var bot = new TelegramBotClient(token);

var cts = new CancellationTokenSource();

var dataSource = new ApiDataSource(new Uri(_configuration["api"]));
var updateHandler = new BotUpdateHandler(dataSource);
bot.StartReceiving(updateHandler, cancellationToken: cts.Token);

Console.WriteLine("Bot is started.");
Console.WriteLine("Press ENTER to stop the bot.");
Console.ReadLine();

cts.Cancel();
Console.WriteLine("Bot was stopped.");