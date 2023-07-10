using DioRed.Murka.Core.Entities;
using DioRed.Vermilion;

using Microsoft.Extensions.Logging;

namespace DioRed.Murka.Core;

public class Logic : ILogic
{
    private readonly ApiClient _api;
    private readonly ILogger _logger;

    public Logic(ApiClient api, ILoggerFactory logger)
    {
        _api = api;
        _logger = logger.CreateLogger("Logic");
    }

    public void Cleanup()
    {
        _logger.LogInformation(EventIDs.CleanupStarted, "Storage cleanup started");
        _api.CleanupPromocodes().GetAwaiter().GetResult();
        _api.CleanupEvents().GetAwaiter().GetResult();
        _logger.LogInformation(EventIDs.CleanupFinished, "Storage cleanup finished");
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

    public ICollection<DayEvent> GetDayEvents(DateOnly date, ChatId chatId)
    {
        return _api.GetDayEvents(date.ToString(CommonValues.DateFormat), ChatIdConvertor.ToString(chatId)).GetAwaiter().GetResult();
    }

    public Northlands GetNorthLands(DateOnly date)
    {
        return _api.GetNorthlands(date.ToString(CommonValues.DateFormat)).GetAwaiter().GetResult();
    }

    public string GetRandomGreeting()
    {
        return _api.GetRandomGreeting().GetAwaiter().GetResult();
    }

    public void AddChat(ChatId chatId, string title)
    {
        try
        {
            _api.AddChat(chatId.Id, ChatIdConvertor.ToTypeString(chatId), title).GetAwaiter().GetResult();
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
        return _api.GetTelegramChats().GetAwaiter().GetResult()
            .Select(chatInfo =>
            {
                (BotSystem system, string type) = ChatIdConvertor.FromTypeString(chatInfo.Type).Value;
                return new ChatId(system, type, chatInfo.Id);
            })
            .ToList();
    }

    public void RemoveChat(ChatId chatId)
    {
        try
        {
            _api.RemoveChat(chatId.Id, ChatIdConvertor.ToTypeString(chatId)).GetAwaiter().GetResult();
            _logger.LogInformation(EventIDs.ChatRemoved, "Chat {ChatId} removed", chatId);
        }
        catch (Exception ex)
        {
            _logger.LogError(EventIDs.ChatRemoveFailure, ex, "Chat {ChatId} remove failure", chatId);
            throw;
        }
    }

    public BinaryData GetCalendar()
    {
        return new BinaryData(_api.GetDailyCalendar().GetAwaiter().GetResult());
    }

    public void AddEvent(Event newEvent)
    {
        _api.AddEvent(newEvent.Name, newEvent.ValidFrom?.ToString(), newEvent.ValidTo?.ToString()).GetAwaiter().GetResult();
    }

    public void AddPromocode(Promocode promocode)
    {
        _api.AddPromocode(promocode.Code, promocode.ValidFrom?.ToString(), promocode.ValidTo?.ToString(), promocode.Content).GetAwaiter().GetResult();
    }

    public void AddDayEvent(string name, string occurrence, TimeOnly time, ChatId? chatId)
    {
        _api.AddDayEvent(name, occurrence, time.ToString(CommonValues.TimeFormat), ChatIdConvertor.ToString(chatId)).GetAwaiter().GetResult();
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