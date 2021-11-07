using Telegram.Bot;

namespace DioRed.Murka.TelegramBot;

public record HandleContext(ITelegramBotClient BotClient, long ChatId, CancellationToken CancellationToken);