using DioRed.Murka.Core.Entities;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DioRed.Murka.TelegramBot;

public static class ChatExtensions
{
    public static ChatInfo ToChatInfo(this Chat chat)
    {
        return new ChatInfo(
            Id: chat.Id.ToString(),
            Type: "Telegram" + chat.Type,
            Title: chat.Type == ChatType.Private
                ? $"{chat.FirstName} {chat.LastName}".Trim()
                : chat.Title ?? string.Empty);
    }
}
