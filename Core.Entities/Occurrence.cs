namespace DioRed.Murka.Core.Entities;

public abstract class Occurrence
{
    public TimeOnly Time { get; set; }

    public abstract bool Contains(DateOnly date);

    public static DailyOccurrence Daily(TimeOnly time)
        => new() { Time = time };

    public static WeeklyOccurrence Weekly(DayOfWeek dayOfWeek, TimeOnly time)
        => new() { DayOfWeek = dayOfWeek, Time = time };
}

public class DailyOccurrence : Occurrence
{
    public override bool Contains(DateOnly date) => true;
}

public class WeeklyOccurrence : Occurrence
{
    public DayOfWeek DayOfWeek { get; set; }

    public override bool Contains(DateOnly date) => DayOfWeek == date.DayOfWeek;
}