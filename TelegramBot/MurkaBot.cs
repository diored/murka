using DioRed.Murka.Core.Contracts;
using DioRed.Murka.Core.Entities;
using DioRed.Murka.TelegramBot.Configuration;
using DioRed.Vermilion;

using Telegram.Bot.Types;

namespace DioRed.Murka.TelegramBot;

public class MurkaBot : Bot
{
    private readonly MurkaConfiguration _configuration;
    private readonly ILogic _logic;
    private readonly IChatWriter _globalWriter;

    private bool _newChatsDetection;

    public MurkaBot(MurkaConfiguration configuration, ILogic logic, ILogger logger, CancellationTokenSource cancellationTokenSource)
        : base(configuration, cancellationTokenSource.Token)
    {
        _configuration = configuration;
        _logic = logic;
        _globalWriter = new GlobalWriter(this);

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
        return new MurkaChatClient(chat, _configuration.AdminId, _logic, _globalWriter);
    }

    class GlobalWriter : IChatWriter
    {
        private readonly MurkaBot _bot;

        public GlobalWriter(MurkaBot bot)
        {
            _bot = bot;
        }

        public event Action<Exception>? OnException;

        public async Task SendHtmlAsync(string html)
        {
            await _bot.Broadcast(writer => writer.SendHtmlAsync(html));
        }

        public async Task SendPhotoAsync(string url)
        {
            await _bot.Broadcast(writer => writer.SendPhotoAsync(url));
        }

        public async Task SendPhotoAsync(Stream stream)
        {
            await _bot.Broadcast(writer => writer.SendPhotoAsync(stream));
        }

        public async Task SendTextAsync(string text)
        {
            await _bot.Broadcast(writer => writer.SendTextAsync(text));
        }
    }
}