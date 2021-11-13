using DioRed.Murka.Core;

using System.Collections.Concurrent;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DioRed.Murka.TelegramBot;

public class BotUpdateHandler : IUpdateHandler
{
    private string? _botName;
    private readonly ConcurrentDictionary<long, ChatClient> _chatClients = new();
    private readonly IDataSource _dataSource;

    public BotUpdateHandler(IDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public UpdateType[]? AllowedUpdates => null;

    private static async Task<string> GetBotName(ITelegramBotClient botClient, CancellationToken cancellationToken)
    {
        return (await botClient.GetMeAsync(cancellationToken)).Username!;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message &&
            update.Message != null &&
            update.Message.Type == MessageType.Text &&
            update.Message.From?.IsBot == false)
        {
            await HandleTextMessage(botClient, update.Message, cancellationToken);
            return;
        }

        if (update.Type == UpdateType.CallbackQuery &&
            update.CallbackQuery?.Message != null &&
            update.CallbackQuery?.Data != null)
        {
            await HandleInlineQuery(botClient, update.CallbackQuery, cancellationToken);
        }
    }

    private async Task HandleTextMessage(ITelegramBotClient botClient, Message updateMessage, CancellationToken cancellationToken)
    {        
        _botName ??= "@" + await GetBotName(botClient, cancellationToken);
        string message = TrimBotName(updateMessage.Text!, _botName);

        ChatClient chatClient = GetChatClient(updateMessage);
        await chatClient.HandleMessage(botClient, message, cancellationToken);
    }

    private async Task HandleInlineQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        ChatClient chatClient = GetChatClient(callbackQuery.Message!);
        
        await chatClient.HandleMessage(botClient, callbackQuery.Data!, cancellationToken);
    }

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is HttpRequestException httpRequestException)
        {
            Console.WriteLine($"HTTP request error: {httpRequestException.Message}");
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            return;
        }

        string message = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API error [{apiRequestException.ErrorCode}]:\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(message);
    }

    private ChatClient GetChatClient(Message updateMessage)
    {
        long chatId = updateMessage.Chat?.Id ?? updateMessage.From!.Id;

        if (!_chatClients.ContainsKey(chatId))
        {
            _chatClients.TryAdd(chatId, new ChatClient(chatId, _dataSource));
        }

        return _chatClients[chatId];
    }

    private static string TrimBotName(string message, string botName)
    {
        Index start = message.StartsWith(botName + " ") ? botName.Length + 1 : 0;
        Index end = message.EndsWith(botName) ? ^botName.Length : ^0;

        return message[start..end];
    }
}