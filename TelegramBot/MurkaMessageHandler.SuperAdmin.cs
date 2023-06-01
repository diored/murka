using DioRed.Murka.Core.Entities;
using DioRed.Vermilion;
using DioRed.Vermilion.Attributes;

namespace DioRed.Murka.TelegramBot;

public partial class MurkaMessageHandler
{
    [BotCommand(UserRole.ChatAdmin, @"/addDayEvent!", BotCommandOptions.CaseInsensitive)]
    public void AddGlobalDayEvent(string name, string time, string repeat)
    {
        AddDayEventInternal(name, repeat, time, MessageContext.ChatClient.Chat.ToChatInfo().ChatId);
    }

    [BotCommand(UserRole.SuperAdmin, "/addEvent")]
    public void AddEvent(string eventName, string? starts, string? ends)
    {
        eventName = eventName.Trim();
        starts = starts?.Trim();
        ends = ends?.Trim();

        Event newEvent = new(eventName, ServerDateTime.ParseOrDefault(starts), ServerDateTime.ParseOrDefault(ends));
        MurkaChat.Logic.AddEvent(newEvent);
    }

    [BotCommand(UserRole.SuperAdmin, "/addPromocode")]
    public void AddPromocode(string code, string validTo, string description)
    {
        code = code.Trim();
        validTo = validTo.Trim();
        description = description.Trim();

        Promocode newPromocode = new(code, description, null, ServerDateTime.ParseOrDefault(validTo));
        MurkaChat.Logic.AddPromocode(newPromocode);
    }

    [BotCommand(UserRole.SuperAdmin, "/cleanup")]
    public void Cleanup()
    {
        MurkaChat.Logic.Cleanup();
    }

    [BotCommand(UserRole.SuperAdmin, "/ga")]
    public async Task GlobalAnnounce(string message)
    {
        await MessageContext.Broadcaster.SendTextAsync(message);
    }

    [BotCommand(UserRole.SuperAdmin, "/setDaily")]
    public void SetDaily(string month, string dailies)
    {
        int monthNumber = int.Parse(month.Trim());
        dailies = dailies.Trim();

        if (DateTime.DaysInMonth(DateTime.Now.Year, monthNumber) != dailies.Length)
        {
            throw new ArgumentException("Wrong month length", nameof(dailies));
        }

        MurkaChat.Logic.SetDaily(monthNumber, dailies);
    }
}