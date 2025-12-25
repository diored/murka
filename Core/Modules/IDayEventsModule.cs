using DioRed.Murka.Core.Entities;
using DioRed.Vermilion;

namespace DioRed.Murka.Core.Modules;

public interface IDayEventsModule
{
    void Add(DayEvent newDayEvent);
    DayEvent[] Get(string date, ChatId chatId);
}