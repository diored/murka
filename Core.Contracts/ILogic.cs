using DioRed.Murka.Common;
using DioRed.Murka.Core.Entities;

namespace DioRed.Murka.Core.Contracts;

public interface ILogic
{
    string GetRandomGreeting();

    ICollection<ChatInfo> GetChats();
    void AddChat(ChatInfo chatInfo);
    void RemoveChat(ChatInfo chatInfo);

    Daily GetDaily(DateOnly date);
    void SetDaily(int monthNumber, string dailies);

    ICollection<DayEvent> GetDayEvents(DateOnly date, string chatId);
    void AddDayEvent(DayEvent dayEvent);

    ICollection<Event> GetActiveEvents(ServerTime serverTime);
    void AddEvent(Event newEvent);

    ICollection<Promocode> GetActivePromocodes(ServerTime serverTime);
    void AddPromocode(Promocode newPromocode);

    Northlands GetNorthLands(DateOnly date);

    BinaryData GetCalendar();

    void Log(string level, string message, object? argumentObject = null, Exception? exception = null);
    void Cleanup();
}
