using DioRed.Murka.Core.Contracts;
using DioRed.Murka.Core.Entities;
using DioRed.Murka.TelegramBot.Configuration;
using DioRed.Vermilion;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DioRed.Murka.TelegramBot;

public class MurkaBot : Bot
{
    private readonly MurkaConfiguration _configuration;
    private readonly ILogic _logic;

    private bool _newChatsDetection;

    public MurkaBot(MurkaConfiguration configuration, ILogic logic, ILogger logger, CancellationTokenSource cancellationTokenSource)
        : base(configuration, cancellationTokenSource.Token)
    {
        _configuration = configuration;
        _logic = logic;

        _newChatsDetection = true;

        Job.SetupDaily(this, () => DailyRoutine(), TimeSpan.FromHours(21));

        AddLogger(logger);
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

    protected override IChatClient CreateChatClient(Chat chat)
    {
        bool isAdmin = chat.Type == ChatType.Private && chat.Id == _configuration.AdminId;

        return new MurkaChatClient(chat, isAdmin, _logic, Broadcaster);
    }
}