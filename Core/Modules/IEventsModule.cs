using DioRed.Murka.Core.Entities;

namespace DioRed.Murka.Core.Modules;

public interface IEventsModule
{
    void AddEvent(Event newEvent);
    void Cleanup();
    Event[] GetActiveEvents();
}