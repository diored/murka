namespace DioRed.Murka.Core.Entities;

public abstract class Occurrence
{
    public TimeOnly Time { get; init; }

    public static Occurrence Daily(TimeOnly time) => new DailyOccurrence
    {
        Time = time
    };

    public static Occurrence Weekly(DayOfWeek dayOfWeek, TimeOnly time) => new WeeklyOccurrence
    {
        DayOfWeek = dayOfWeek,
        Time = time
    };
}

public class DailyOccurrence : Occurrence
{
}

public class WeeklyOccurrence : Occurrence
{
    public DayOfWeek DayOfWeek { get; init; }
}