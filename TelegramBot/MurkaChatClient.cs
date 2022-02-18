using DioRed.Murka.Core.Contracts;
using DioRed.Vermilion;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DioRed.Murka.TelegramBot;

public class MurkaChatClient : IChatClient
{
    private readonly Broadcaster _broadcaster;

    private string? _botName;

    public MurkaChatClient(Chat chat, bool isAdmin, ILogic logic, Broadcaster broadcaster)
    {
        Chat = chat;
        IsAdmin = isAdmin;
        Logic = logic;
        _broadcaster = broadcaster;
    }

    public Chat Chat { get; }
    public bool IsAdmin { get; }
    public ILogic Logic { get; }

    public DateTime? LatestGreeting { get; set; }

    public async Task HandleCallbackQueryAsync(Bot bot, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        MurkaMessageHandler handler = GetMessageHandler(bot.BotClient, cancellationToken);
        await HandleCommand(callbackQuery.Data!, handler);
    }

    public async Task HandleMessageAsync(Bot bot, Message message, CancellationToken cancellationToken)
    {
        if (message!.Type == MessageType.Text &&
            message.From?.IsBot == false)
        {
            _botName ??= "@" + await GetBotNameAsync(bot.BotClient, cancellationToken);
            string messageText = TrimBotName(message.Text!, _botName);

            MurkaMessageHandler handler = GetMessageHandler(bot.BotClient, cancellationToken);
            await HandleCommand(messageText, handler);
        }
    }

    public async Task ShowAgendaAsync(ITelegramBotClient botClient, CancellationToken cancellationToken)
    {
        MurkaMessageHandler handler = GetMessageHandler(botClient, cancellationToken);
        await handler.ShowAgendaAsync();
    }

    private async Task HandleCommand(string command, MurkaMessageHandler handler)
    {
        await handler.HandleAsync(command, IsAdmin);
    }

    private MurkaMessageHandler GetMessageHandler(ITelegramBotClient botClient, CancellationToken cancellationToken)
    {
        MessageContext messageContext = new(botClient, this, _broadcaster, cancellationToken);
        return new MurkaMessageHandler(messageContext);
    }

    private static async Task<string> GetBotNameAsync(ITelegramBotClient botClient, CancellationToken cancellationToken)
    {
        return (await botClient.GetMeAsync(cancellationToken)).Username!;
    }

    private static string TrimBotName(string message, string botName)
    {
        Index start = message.StartsWith(botName + " ") ? botName.Length + 1 : 0;
        Index end = message.EndsWith(botName) ? ^botName.Length : ^0;

        return message[start..end];
    }
}
