using DioRed.Murka.Core;
using DioRed.TelegramBot;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DioRed.Murka.TelegramBot;

public class MurkaChatClient : IChatClient
{
    private string? _botName;
    private Job? _agendaJob;

    public MurkaChatClient(Chat chat, IDataSource dataSource)
    {
        Chat = chat;
        DataSource = dataSource;
    }

    public Chat Chat { get; }
    public IDataSource DataSource { get; }

    public DateTime? LatestGreeting { get; set; }

    public void ConfigureJobs(ITelegramBotClient botClient, CancellationToken cancellationToken)
    {
        _agendaJob = Job.SetupDaily(ct => CheckAgendaSubscription(botClient, ct), TimeSpan.FromHours(21));
    }

    public async Task HandleCallbackQueryMessage(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        MurkaMessageHandler handler = GetMessageHandler(botClient, cancellationToken);
        await handler.Handle(callbackQuery.Data);
    }

    public async Task HandleMessage(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        if (message!.Type == MessageType.Text &&
            message.From?.IsBot == false)
        {
            _botName ??= "@" + await GetBotName(botClient, cancellationToken);
            string messageText = TrimBotName(message.Text!, _botName);

            MurkaMessageHandler handler = GetMessageHandler(botClient, cancellationToken);
            await handler.Handle(messageText);
        }
    }

    private async Task CheckAgendaSubscription(ITelegramBotClient botClient, CancellationToken cancellationToken)
    {
        MurkaMessageHandler handler = GetMessageHandler(botClient, cancellationToken);
        await handler.Handle("/agenda");
    }

    private MurkaMessageHandler GetMessageHandler(ITelegramBotClient botClient, CancellationToken cancellationToken)
    {
        return new(new MessageContext(botClient, this, cancellationToken));
    }

    private static async Task<string> GetBotName(ITelegramBotClient botClient, CancellationToken cancellationToken)
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
