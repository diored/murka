using DioRed.Murka.Core;
using DioRed.Murka.Core.Contracts;
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
        //Job.SetupDaily(this, () => Cleanup(), TimeSpan.FromHours(21.05));
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

    private Task Cleanup()
    {
        throw new NotImplementedException();
    }

    private static BotConfiguration CreateConfiguration(IConfiguration configuration)
    {
        ILogic logic = new Logic(configuration);

        return new BotConfiguration(
            configuration["token"],
            chat => new MurkaChatClient(chat, logic),
            configuration.GetSection("reconnectToChats").Get<long[]>()
        );
    }
}