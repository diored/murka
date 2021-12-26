using DioRed.Murka.Core.Contracts;
using DioRed.Murka.Core.Entities;
using DioRed.Vermilion;

using Microsoft.Extensions.Configuration;

namespace DioRed.Murka.TelegramBot;

public class MurkaBot : Bot
{
    private readonly ILogic _logic;

    public MurkaBot(IConfiguration configuration, ILogic logic, CancellationToken cancellationToken)
        : base(CreateConfiguration(configuration, logic), cancellationToken)
    {
        _logic = logic;

        Job.SetupDaily(this, () => DailyRoutine(), TimeSpan.FromHours(21));
    }

    public async Task ReconnectToChats()
    {
        ICollection<ChatInfo> chats = _logic.GetChats();

        foreach (ChatInfo chat in chats)
        {
            bool result = await ConnectToChatAsync(long.Parse(chat.Id));
            if (!result)
            {
                _logic.RemoveChat(chat);
            }
        }
    }

    private async Task DailyRoutine()
    {
        _logic.Cleanup();
        await Broadcast(DailyAgenda);
    }

    private async Task DailyAgenda(IChatClient chatClient, CancellationToken token)
    {
        await ((MurkaChatClient)chatClient).ShowAgendaAsync(BotClient, token);
    }

    private static BotConfiguration CreateConfiguration(IConfiguration configuration, ILogic logic)
    {
        return new BotConfiguration(
            configuration["token"],
            chat => new MurkaChatClient(chat, logic)
        );
    }
}