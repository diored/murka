using DioRed.Murka.Core.Entities;
using DioRed.Vermilion;

namespace DioRed.Murka.Core;

public interface ILogic
{
    string GetRandomGreeting();

    void AddChat(ChatId chatInfo, string title);
    ICollection<ChatId> GetChats();
    void RemoveChat(ChatId chatInfo);

    Daily GetDaily(DateOnly date);
    void SetDaily(int monthNumber, string dailies);

    ICollection<DayEvent> GetDayEvents(DateOnly date, ChatId chatId);
    void AddDayEvent(string name, string occurrence, TimeOnly time, ChatId? chatId);

    ICollection<Event> GetActiveEvents();
    void AddEvent(Event newEvent);

    ICollection<Promocode> GetActivePromocodes();
    void AddPromocode(Promocode promocode);
    void UpdatePromocode(Promocode promocode);
    void RemovePromocode(string code);

    Northlands GetNorthLands(DateOnly date);

    string GetLink(string id);
    BinaryData GetCalendar();

    void Cleanup();
}