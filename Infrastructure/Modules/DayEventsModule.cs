using DioRed.Murka.Core.Entities;
using DioRed.Murka.Core.Modules;
using DioRed.Murka.Infrastructure.AzureStorage;
using DioRed.Vermilion;

namespace DioRed.Murka.Infrastructure.Modules;

public class DayEventsModule(DayEventsStorage storage) : IDayEventsModule
{
    public void Add(DayEvent newDayEvent)
    {
        storage.AddNew(newDayEvent);
    }

    public DayEvent[] Get(DateOnly date, ChatId chatId)
    {
        return storage.Get(date, chatId);
    }
}