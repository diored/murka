using DioRed.Murka.Common;
using DioRed.Murka.Core.Entities;
using DioRed.Vermilion;
using DioRed.Vermilion.Attributes;

using Telegram.Bot;

namespace DioRed.Murka.TelegramBot;

public partial class MurkaMessageHandler
{
    [BotCommand(UserRole.ChatAdmin, @"/addDayEvent!", BotCommandOptions.CaseInsensitive)]
    public void AddGlobalDayEvent(string name, string time, string repeat)
    {
        AddDayEventInternal(MessageContext.ChatClient.Chat.ToChatInfo().ToString(), name, time, repeat);
    }

    [BotCommand(UserRole.SuperAdmin, "/addEvent")]
    public void AddEvent(string eventName, string? starts, string? ends)
    {
        eventName = eventName.Trim();
        starts = starts?.Trim();
        ends = ends?.Trim();

        Event newEvent = new(eventName, ParseServerTimeRange(starts, ends));
        MurkaChat.Logic.AddEvent(newEvent);
    }

    [BotCommand(UserRole.SuperAdmin, "/addPromocode")]
    public void AddPromocode(string code, string validTo, string description)
    {
        code = code.Trim();
        validTo = validTo.Trim();
        description = description.Trim();

        Promocode newPromocode = new(code, description, new ServerTimeRange(null, ServerTime.SafeParse(validTo)));
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

    //[BotCommand(UserRole.SuperAdmin, "/x")]
    //public async void DeleteMe()
    //{
    //    if (MessageContext.MessageId != 0)
    //    {
    //        await RemoveMessage();
    //    }
    //}
}