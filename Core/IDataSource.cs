using DioRed.Murka.Core.Entities;

namespace DioRed.Murka.Core;

public interface IDataSource
{
    Event[] GetActiveEvents(DateTime dateTime);
    Promocode[] GetActivePromocodes(DateTime dateTime);
    Daily GetDaily(DateTime dateTime);
    IEnumerable<string> GetDayEvents(DateTime dateTime);
    string GetNorth(DateTime dateTime, NorthArmy army);
    string GetRandomGreeting();
}
