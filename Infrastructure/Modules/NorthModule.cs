using DioRed.Murka.Core.Entities;
using DioRed.Murka.Core.Modules;

namespace DioRed.Murka.Infrastructure.Modules;

public class NorthModule : INorthModule
{
    public Northlands GetNorth(string date)
    {
        DateOnly dateOnly = DateOnly.Parse(date);

        DayOfWeek dayOfWeek = dateOnly.DayOfWeek;

        if (dayOfWeek == DayOfWeek.Tuesday)
        {
            return new Northlands("Ледяной штурм", "Ледяной штурм");
        }

        int[] godsEvents = dayOfWeek switch
        {
            DayOfWeek.Monday or DayOfWeek.Friday => [18, 19, 20, 21],
            DayOfWeek.Wednesday or DayOfWeek.Thursday => [18, 19, 21, 22],
            DayOfWeek.Saturday or DayOfWeek.Sunday => [18, 19],
            _ => []
        };

        int[] northEvents = Enumerable.Range(16, 8) // 16..23
            .Except(godsEvents)
            .ToArray();

        return new Northlands(
            string.Join(", ", northEvents.Select(e => $"{e}:00")),
            string.Join(", ", godsEvents.Select(e => $"{e}:00")));
    }
}