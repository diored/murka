using DioRed.Murka.Core;
using DioRed.Vermilion;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DioRed.Murka.TelegramBot;

public class MurkaChatClient : IChatClient
{
    private string? _botName;

    public MurkaChatClient(Chat chat, IDataSource dataSource)
    {
        Chat = chat;
        DataSource = dataSource;
    }

    public Chat Chat { get; }

    public IDataSource DataSource { get; }

    public DateTime? LatestGreeting { get; set; }

    public async Task HandleCallbackQueryAsync(Bot bot, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        MurkaMessageHandler handler = GetMessageHandler(bot.BotClient, cancellationToken);
        await handler.HandleAsync(callbackQuery.Data!);
    }

    public async Task HandleMessageAsync(Bot bot, Message message, CancellationToken cancellationToken)
    {
        if (message!.Type == MessageType.Text &&
            message.From?.IsBot == false)
        {
            _botName ??= "@" + await GetBotNameAsync(bot.BotClient, cancellationToken);
            string messageText = TrimBotName(message.Text!, _botName);

            MurkaMessageHandler handler = GetMessageHandler(bot.BotClient, cancellationToken);
            await handler.HandleAsync(messageText);
        }
    }

    public async Task ShowAgendaAsync(ITelegramBotClient botClient, CancellationToken cancellationToken)
    {
        MurkaMessageHandler handler = GetMessageHandler(botClient, cancellationToken);
        await handler.HandleAsync("/agenda");
    }

    private MurkaMessageHandler GetMessageHandler(ITelegramBotClient botClient, CancellationToken cancellationToken)
    {
        return new(new MessageContext(botClient, this, cancellationToken));
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
