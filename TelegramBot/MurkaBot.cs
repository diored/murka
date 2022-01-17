using DioRed.Murka.Core.Contracts;
using DioRed.Murka.Core.Entities;
using DioRed.Vermilion;

namespace DioRed.Murka.TelegramBot;

public class MurkaBot : Bot
{
    private readonly ILogic _logic;

    public MurkaBot(string botToken, ILogic logic, CancellationToken cancellationToken)
        : base(CreateConfiguration(botToken, logic), cancellationToken)
    {
        _logic = logic;

        Job.SetupDaily(this, () => DailyRoutine(), TimeSpan.FromHours(21));
    }

    public async Task ReconnectToChats()
    {
        ICollection<ChatInfo> chats = _logic.GetChats();

        foreach (ChatInfo chat in chats)
        {
            await ReconnectToChatAsync(chat);
        }
    }

    private async Task ReconnectToChatAsync(ChatInfo chat)
    {
        try
        {
            await ConnectToChatAsync(long.Parse(chat.Id));
        }
        catch (Exception ex) when (ex.Message.Contains("kicked"))
        {
            _logic.RemoveChat(chat);
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

    private static BotConfiguration CreateConfiguration(string botToken, ILogic logic)
    {
        return new BotConfiguration(
            botToken,
            chat => new MurkaChatClient(chat, logic)
        );
    }
}