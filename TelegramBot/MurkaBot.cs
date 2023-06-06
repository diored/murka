using DioRed.Murka.Core;
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

    public MurkaBot(
        MurkaConfiguration configuration,
        ILogic logic,
        ILogger logger,
        CancellationTokenSource cancellationTokenSource)
            : base(configuration, cancellationTokenSource.Token)
    {
        _configuration = configuration;
        _logic = logic;

        _newChatsDetection = true;

        Logger.Loggers.Add(logger);

        Job.SetupDaily(() => DailyRoutine(), new TimeOnly(21, 0), Logger, "CleanupAndAgenda");
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

    public async Task DailyAgenda(IChatClient chatClient, CancellationToken token)
    {
        await ((MurkaChatClient)chatClient).ShowAgendaAsync(BotClient, BotSenderId, token);
    }

    protected override IChatClient CreateChatClient(Chat chat)
    {
        bool isSuperAdmin = chat.Type == ChatType.Private && chat.Id == _configuration.SuperAdminId;

        return new MurkaChatClient(chat, isSuperAdmin, _logic, Broadcaster);
    }
}