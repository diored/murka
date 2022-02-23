using DioRed.Murka.Core.Entities;

namespace DioRed.Murka.Storage.Contracts;

public interface IDayEventsStorage
{
    void AddNew(DayEvent dayEvent);
    DayEvent[] Get(DateOnly date, string chatId);
}
