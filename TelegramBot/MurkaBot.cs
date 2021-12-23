using DioRed.Murka.Core;
using DioRed.Vermilion;

using Microsoft.Extensions.Configuration;

using Telegram.Bot;

namespace DioRed.Murka.TelegramBot;

public class MurkaBot : Bot
{
    public MurkaBot(IConfiguration configuration)
        : base(CreateConfiguration(configuration))
    {
        Job.SetupDaily(this, () => Broadcast(DailyAgenda, CancellationToken), TimeSpan.FromHours(21));
        //Job.SetupOneTime(this, () => Broadcast(ConnectionTest, CancellationToken), DateTime.UtcNow.AddSeconds(15));
    }

    private async Task DailyAgenda(IChatClient chatClient, CancellationToken token)
    {
        await ((MurkaChatClient)chatClient).ShowAgendaAsync(BotClient, token);
    }

    private async Task ConnectionTest(IChatClient chatClient, CancellationToken token)
    {
        await BotClient.SendTextMessageAsync(chatClient.Chat.Id, "Проверка связи", cancellationToken: token);
    }

    private static BotConfiguration CreateConfiguration(IConfiguration configuration)
    {
        ApiDataSource dataSource = new(new Uri(configuration["api"]));

        return new BotConfiguration(
            configuration["token"],
            chat => new MurkaChatClient(chat, dataSource),
            configuration.GetSection("reconnectToChats").Get<long[]>()
        );
    }
}