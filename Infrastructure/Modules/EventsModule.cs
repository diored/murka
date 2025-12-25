using DioRed.Murka.Core.Entities;
using DioRed.Murka.Core.Modules;
using DioRed.Murka.Infrastructure.AzureStorage;

using Microsoft.Extensions.Logging;

namespace DioRed.Murka.Infrastructure.Modules;

public class EventsModule(EventsStorage storage, ILogger<EventsModule> logger) : IEventsModule
{
    public Event[] GetActiveEvents()
    {
        return storage.GetActive(ServerDateTime.GetCurrent());
    }

    public void AddEvent(Event newEvent)
    {
        storage.AddNew(newEvent);
    }

    public void Cleanup()
    {
        string[] removed = storage.RemoveOutdated();

        if (removed.Length != 0)
        {
            logger.LogInformation(EventIDs.MurkaCleanup, "Outdated events [{Events}] were removed", string.Join(", ", removed));
        }
    }
}