using DioRed.Murka.Core.Entities;
using DioRed.Vermilion;

using Microsoft.Extensions.Logging;

namespace DioRed.Murka.Core;

internal class Logic(ApiFacade api, ILoggerFactory logger) : ILogic
{
    private readonly ILogger _logger = logger.CreateLogger("Logic");

    public void Cleanup()
    {
        _logger.LogInformation(EventIDs.CleanupStarted, "Storage cleanup started");
        try
        {
            api.CleanupPromocodes().GetAwaiter().GetResult();
            api.CleanupEvents().GetAwaiter().GetResult();
            _logger.LogInformation(EventIDs.CleanupFinished, "Storage cleanup finished");
        }
        catch (Exception ex)
        {
            _logger.LogError(EventIDs.CleanupFailed, ex, "Storage cleanup failed");
        }
    }

    public ICollection<Event> GetActiveEvents()
    {
        return api.GetActiveEvents().GetAwaiter().GetResult();
    }

    public ICollection<Promocode> GetActivePromocodes()
    {
        return api.GetActivePromocodes().GetAwaiter().GetResult();
    }

    public Daily GetDaily(DateOnly date)
    {
        return api.GetDaily(date.ToString(CommonValues.DateFormat)).GetAwaiter().GetResult();
    }

    public ICollection<DayEvent> GetDayEvents(DateOnly date, ChatId chatId)
    {
        return api.GetDayEvents(date.ToString(CommonValues.DateFormat), chatId).GetAwaiter().GetResult();
    }

    public Northlands GetNorthLands(DateOnly date)
    {
        return api.GetNorthlands(date.ToString(CommonValues.DateFormat)).GetAwaiter().GetResult();
    }

    public string GetRandomGreeting()
    {
        return api.GetRandomGreeting().GetAwaiter().GetResult();
    }

    public void AddChat(ChatId chatId, string title)
    {
        try
        {
            api.AddChat(chatId.System, chatId.Type, chatId.Id, title).GetAwaiter().GetResult();
            _logger.LogInformation(EventIDs.ChatAdded, "Chat {ChatId} ({Title}) added", chatId, title);
        }
        catch (Exception ex)
        {
            _logger.LogError(EventIDs.ChatAddFailure, ex, "Chat {ChatId} ({Title}) add failure", chatId, title);
            throw;
        }
    }

    public ICollection<ChatId> GetChats()
    {
        return api.GetChats().GetAwaiter().GetResult();
    }

    public void RemoveChat(ChatId chatId)
    {
        try
        {
            api.RemoveChat(chatId.System, chatId.Id).GetAwaiter().GetResult();
            _logger.LogInformation(EventIDs.ChatRemoved, "Chat {ChatId} removed", chatId);
        }
        catch (Exception ex)
        {
            _logger.LogError(EventIDs.ChatRemoveFailure, ex, "Chat {ChatId} remove failure", chatId);
            throw;
        }
    }

    public void AddEvent(Event newEvent)
    {
        api.AddEvent(newEvent.Name, newEvent.ValidFrom?.ToString(), newEvent.ValidTo?.ToString()).GetAwaiter().GetResult();
    }

    public void AddPromocode(Promocode promocode)
    {
        api.AddPromocode(promocode.Code, promocode.ValidFrom?.ToString(), promocode.ValidTo?.ToString(), promocode.Content).GetAwaiter().GetResult();
    }

    public void AddDayEvent(string name, string occurrence, TimeOnly time, ChatId? chatId)
    {
        api.AddDayEvent(name, occurrence, time.ToString(CommonValues.TimeFormat), chatId).GetAwaiter().GetResult();
    }

    public void SetDaily(int month, string dailies)
    {
        api.SetDailyMonth(month, dailies).GetAwaiter().GetResult();
    }

    public void UpdatePromocode(Promocode promocode)
    {
        api.UpdatePromocode(promocode.Code, promocode.ValidFrom?.ToString(), promocode.ValidTo?.ToString(), promocode.Content).GetAwaiter().GetResult();
    }

    public void RemovePromocode(string code)
    {
        api.RemovePromocode(code).GetAwaiter().GetResult();
    }

    public string GetLink(string id)
    {
        return api.GetLink(id).GetAwaiter().GetResult();
    }
}