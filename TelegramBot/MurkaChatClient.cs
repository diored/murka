using DioRed.Murka.Core.Contracts;
using DioRed.Vermilion;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DioRed.Murka.TelegramBot;

public class MurkaChatClient : ChatClient
{
    private readonly Broadcaster _broadcaster;
    private readonly bool _isSuperAdmin;

    private string? _botName;

    public MurkaChatClient(Chat chat, bool isSuperAdmin, ILogic logic, Broadcaster broadcaster)
        : base(chat)
    {
        _isSuperAdmin = isSuperAdmin;
        Logic = logic;
        _broadcaster = broadcaster;
    }

    public ILogic Logic { get; }

    public DateTime? LatestGreeting { get; set; }

    public override async Task HandleCallbackQueryAsync(Bot bot, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        MurkaMessageHandler handler = await GetMessageHandlerAsync(bot.BotClient, callbackQuery.From.Id, 0, cancellationToken);

        await handler.HandleAsync(callbackQuery.Data!);
    }

    public override async Task HandleMessageAsync(Bot bot, Message message, CancellationToken cancellationToken)
    {
        if (message!.Type != MessageType.Text ||
            (message.From?.IsBot) != false)
        {
            return;
        }

        _botName ??= "@" + await GetBotNameAsync(bot.BotClient, cancellationToken);
        string messageText = TrimBotName(message.Text!, _botName);

        MurkaMessageHandler handler = await GetMessageHandlerAsync(bot.BotClient, message.From.Id, message.MessageId, cancellationToken);

        await handler.HandleAsync(messageText);
    }

    public async Task ShowAgendaAsync(ITelegramBotClient botClient, long senderId, CancellationToken cancellationToken)
    {
        MurkaMessageHandler handler = await GetMessageHandlerAsync(botClient, senderId, 0, cancellationToken);
        await handler.ShowAgendaAsync();
    }

    private async Task<MurkaMessageHandler> GetMessageHandlerAsync(ITelegramBotClient botClient, long senderId, int messageId, CancellationToken cancellationToken)
    {
        UserRole role = await GetUserRoleAsync(botClient, senderId, cancellationToken);
        MessageContext messageContext = new(botClient, this, role, _broadcaster, messageId, cancellationToken);
        return new MurkaMessageHandler(messageContext);
    }

    protected override async Task<UserRole> GetUserRoleAsync(ITelegramBotClient botClient, long senderId, CancellationToken cancellationToken)
    {
        var role = await base.GetUserRoleAsync(botClient, senderId, cancellationToken);

        if (_isSuperAdmin)
        {
            role |= UserRole.SuperAdmin;
        }

        return role;
    }
}
