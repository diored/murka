using DioRed.Murka.Common;
using DioRed.Murka.Core.Entities;

namespace DioRed.Murka.Core.Contracts;

public interface ILogic
{
    void Cleanup();
    ICollection<Event> GetActiveEvents(ServerTime serverTime);
    ICollection<Promocode> GetActivePromocodes(ServerTime serverTime);
    Daily GetDaily(DateOnly date);
    ICollection<DayEvent> GetDayEvents(DateOnly date);
    Northlands GetNorthLands(DateOnly date);
    string GetRandomGreeting();
    ICollection<ChatInfo> GetChats();
    void AddChat(ChatInfo chatInfo);
    void RemoveChat(ChatInfo chatInfo);
    void Log(string level, string message, object? argumentObject = null, Exception? exception = null);
    BinaryData GetCalendar();
    void AddEvent(Event newEvent);
    void AddPromocode(Promocode newPromocode);
}
