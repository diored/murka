using DioRed.Murka.Core.Entities;

namespace DioRed.Murka.Storage.Contracts;

public interface IEventsStorage : ITimeLimitedStorage<Event>
{
    void AddNew(Event newEvent);
}
