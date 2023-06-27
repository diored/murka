using DioRed.Murka.Core.Entities;
using DioRed.Vermilion;
using DioRed.Vermilion.Handlers;

namespace DioRed.Murka.BotCore;

public partial class MessageHandler
{
    [BotCommand(UserRole.SuperAdmin, @"/addDayEvent!", BotCommandOptions.CaseInsensitive)]
    public void AddGlobalDayEvent(string name, string time, string repeat)
    {
        AddDayEventInternal(name, repeat, time, null);
    }

    [BotCommand(UserRole.SuperAdmin, "/addEvent")]
    public void AddEvent(string eventName, string? starts, string? ends)
    {
        eventName = eventName.Trim();
        starts = starts?.Trim();
        ends = ends?.Trim();

        Event newEvent = new(eventName, ServerDateTime.ParseOrDefault(starts), ServerDateTime.ParseOrDefault(ends));
        _logic.AddEvent(newEvent);
    }

    [BotCommand(UserRole.SuperAdmin, "/addPromocode")]
    public void AddPromocode(string code, string validTo, string description)
    {
        code = code.Trim();
        validTo = validTo.Trim();
        description = description.Trim();

        Promocode newPromocode = new(code, description, null, ServerDateTime.ParseOrDefault(validTo));
        _logic.AddPromocode(newPromocode);
    }

    [BotCommand(UserRole.SuperAdmin, "/cleanup")]
    public void Cleanup()
    {
        _logic.Cleanup();
    }

    [BotCommand(UserRole.SuperAdmin, "/ga")]
    public async Task GlobalAnnounce(string message)
    {
        await MessageContext.Bot.Manager.Broadcast(chat => chat.SendTextAsync(message));
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

        _logic.SetDaily(monthNumber, dailies);
    }
}