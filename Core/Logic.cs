using DioRed.Murka.Core.Entities;

namespace DioRed.Murka.Core;

public class Logic : ILogic
{
    private readonly ApiClient _api;

    public Logic(ApiClient api)
    {
        _api = api;
    }

    public void Cleanup()
    {
        _api.Log("cleanup", "Storage cleanup started").GetAwaiter().GetResult();
        _api.CleanupPromocodes().GetAwaiter().GetResult();
        _api.CleanupEvents().GetAwaiter().GetResult();
        _api.Log("cleanup", "Storage cleanup finished").GetAwaiter().GetResult();
    }

    public ICollection<Event> GetActiveEvents()
    {
        return _api.GetActiveEvents().GetAwaiter().GetResult();
    }

    public ICollection<Promocode> GetActivePromocodes()
    {
        return _api.GetActivePromocodes().GetAwaiter().GetResult();
    }

    public Daily GetDaily(DateOnly date)
    {
        return _api.GetDaily(date.ToString(CommonValues.DateFormat)).GetAwaiter().GetResult();
    }

    public ICollection<DayEvent> GetDayEvents(DateOnly date, string chatId)
    {
        return _api.GetDayEvents(date.ToString(CommonValues.DateFormat), chatId).GetAwaiter().GetResult();
    }

    public Northlands GetNorthLands(DateOnly date)
    {
        return _api.GetNorthlands(date.ToString(CommonValues.DateFormat)).GetAwaiter().GetResult();
    }

    public string GetRandomGreeting()
    {
        return _api.GetRandomGreeting().GetAwaiter().GetResult();
    }

    public ICollection<ChatInfo> GetChats()
    {
        return _api.GetTelegramChats().GetAwaiter().GetResult();
    }

    public void AddChat(ChatInfo chatInfo)
    {
        const string level = "chat";

        try
        {
            _api.AddChat(chatInfo.Id, chatInfo.Type, chatInfo.Title).GetAwaiter().GetResult();
            _api.Log(level, "Chat added", chatInfo).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            _api.Log(level, "Chat adding failed", chatInfo, ex).GetAwaiter().GetResult();

            throw;
        }
    }

    public void RemoveChat(ChatInfo chatInfo)
    {
        const string level = "chat";

        try
        {
            _api.RemoveChat(chatInfo.Id, chatInfo.Type).GetAwaiter().GetResult();
            _api.Log(level, "Chat removed", chatInfo).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            _api.Log(level, "Chat removing failed", chatInfo, ex).GetAwaiter().GetResult();

            throw;
        }
    }

    public BinaryData GetCalendar()
    {
        return new BinaryData(_api.GetDailyCalendar().GetAwaiter().GetResult());
    }

    public void Log(string level, string message, object? argumentObject = null, Exception? exception = null)
    {
        _api.Log(level, message, argumentObject, exception).GetAwaiter().GetResult();
    }

    public void AddEvent(Event newEvent)
    {
        _api.AddEvent(newEvent.Name, newEvent.ValidFrom?.ToString(), newEvent.ValidTo?.ToString()).GetAwaiter().GetResult();
    }

    public void AddPromocode(Promocode promocode)
    {
        _api.AddPromocode(promocode.Code, promocode.ValidFrom?.ToString(), promocode.ValidTo?.ToString(), promocode.Content).GetAwaiter().GetResult();
    }

    public void AddDayEvent(string name, string occurrence, TimeOnly time, string? chatId)
    {
        _api.AddDayEvent(name, occurrence, time.ToString(CommonValues.TimeFormat), chatId).GetAwaiter().GetResult();
    }

    public void SetDaily(int month, string dailies)
    {
        _api.SetDailyMonth(month, dailies).GetAwaiter().GetResult();
    }

    public void UpdatePromocode(Promocode promocode)
    {
        _api.UpdatePromocode(promocode.Code, promocode.ValidFrom?.ToString(), promocode.ValidTo?.ToString(), promocode.Content).GetAwaiter().GetResult();
    }

    public void RemovePromocode(string code)
    {
        _api.RemovePromocode(code).GetAwaiter().GetResult();
    }

    public string GetLink(string id)
    {
        return _api.GetLink(id).GetAwaiter().GetResult();
    }
}
