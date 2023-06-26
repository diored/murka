using System.Diagnostics.CodeAnalysis;

using DioRed.Vermilion;

namespace DioRed.Murka.Core;

internal static class ChatIdConvertor
{
    [return: NotNullIfNotNull(nameof(chatId))]
    public static string? ToString(ChatId? chatId)
    {
        if (chatId is null)
        {
            return null;
        }

        return $"{ToTypeString(chatId)}:{chatId.Id}";
    }

    [return: NotNullIfNotNull(nameof(chatIdString))]
    public static ChatId? FromString(string? chatIdString)
    {
        if (chatIdString is null)
        {
            return null;
        }

        string[] parts = chatIdString.Split(':');
        BotSystem system = Enum.GetValues<BotSystem>().First(systemName => parts[0].StartsWith(systemName.ToString()));

        return new ChatId
        (
            System: system,
            Type: parts[0][system.ToString().Length..],
            Id: parts[1]
        );
    }

    [return: NotNullIfNotNull(nameof(chatId))]
    public static string? ToTypeString(ChatId? chatId)
    {
        if (chatId is null)
        {
            return null;
        }

        return $"{chatId.System}{chatId.Type}";
    }

    [return: NotNullIfNotNull(nameof(typeString))]
    public static (BotSystem system, string type)? FromTypeString(string? typeString)
    {
        if (typeString is null)
        {
            return null;
        }

        BotSystem system = Enum.GetValues<BotSystem>().First(systemName => typeString.StartsWith(systemName.ToString()));

        return (system, typeString[system.ToString().Length..]);
    }
}
