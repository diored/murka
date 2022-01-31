using DioRed.Murka.Core.Contracts;
using DioRed.Murka.Core.Entities;
using DioRed.Vermilion;

using Telegram.Bot.Types;

namespace DioRed.Murka.TelegramBot;

public class MurkaBot : Bot
{
    private readonly ILogic _logic;

    private bool _newChatsDetection;

    public MurkaBot(string botToken, ILogic logic, CancellationToken cancellationToken)
        : base(CreateConfiguration(botToken, logic), cancellationToken)
    {
        _logic = logic;
        _newChatsDetection = true;

        Job.SetupDaily(this, () => DailyRoutine(), TimeSpan.FromHours(21));
    }

    protected override void OnChatClientAdded(Chat chat)
    {
        base.OnChatClientAdded(chat);

        if (_newChatsDetection)
        {
            _logic.AddChat(chat.ToChatInfo());
        }
    }

    public override void StartReceiving()
    {
        _newChatsDetection = false;
        ReconnectToChats();
        _newChatsDetection = true;

        base.StartReceiving();
    }

    private void ReconnectToChats()
    {
        ICollection<ChatInfo> chats = _logic.GetChats();

        foreach (ChatInfo chat in chats)
        {
            ReconnectToChatAsync(chat).GetAwaiter().GetResult();
        }
    }

    private async Task ReconnectToChatAsync(ChatInfo chat)
    {
        try
        {
            await ConnectToChatAsync(long.Parse(chat.Id));
        }
        catch (Exception ex) when (ex.Message.Contains("kicked") || ex.Message.Contains("blocked"))
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