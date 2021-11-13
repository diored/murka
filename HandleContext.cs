using DioRed.Murka.Core;

using Telegram.Bot;

namespace DioRed.Murka.TelegramBot;

public record HandleContext(
    ITelegramBotClient BotClient,
    long ChatId,
    IDataSource DataSource,
    CancellationToken CancellationToken);