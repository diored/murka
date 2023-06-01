using DioRed.Murka.Core.Entities;

namespace DioRed.Murka.Core;

public interface ILogic
{
    string GetRandomGreeting();

    ICollection<ChatInfo> GetChats();
    void AddChat(ChatInfo chatInfo);
    void RemoveChat(ChatInfo chatInfo);

    Daily GetDaily(DateOnly date);
    void SetDaily(int monthNumber, string dailies);

    ICollection<DayEvent> GetDayEvents(DateOnly date, string chatId);
    void AddDayEvent(string name, string occurrence, TimeOnly time, string? chatId);

    ICollection<Event> GetActiveEvents();
    void AddEvent(Event newEvent);

    ICollection<Promocode> GetActivePromocodes();
    void AddPromocode(Promocode promocode);
    void UpdatePromocode(Promocode promocode);
    void RemovePromocode(string code);

    Northlands GetNorthLands(DateOnly date);

    string GetLink(string id);
    BinaryData GetCalendar();

    void Log(string level, string message, object? argumentObject = null, Exception? exception = null);
    void Cleanup();
}
