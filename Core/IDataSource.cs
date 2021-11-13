using DioRed.Murka.Core.Entities;

using System.Text.RegularExpressions;

namespace DioRed.Murka.Core;

public interface IDataSource
{
    DateTime ServerTime { get; }
    Regex[] MurrTriggers { get; }

    Event[] GetActiveEvents(DateTime dateTime);
    Promocode[] GetActivePromocodes(DateTime dateTime);
    Daily? GetDaily(DateTime dateTime);
    IEnumerable<string> GetDayEvents(DateTime dateTime);
    string GetNorth(DateTime dateTime, NorthArmy army);
    string GetRandomGreeting();
}
