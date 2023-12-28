namespace DioRed.Murka.Graphics;

public record struct ScheduleItem(string Day, string Month, string Quest)
{
    public ScheduleItem(DateOnly day, string quest)
        : this(day.Day.ToString("D2"), GetMonthName(day.Month), quest)
    {
    }

    private static string GetMonthName(int month)
    {
        return month switch
        {
            1 => "янв.",
            2 => "фев.",
            3 => "мар.",
            4 => "апр.",
            5 => "мая",
            6 => "июня",
            7 => "июля",
            8 => "авг.",
            9 => "сен.",
            10 => "окт.",
            11 => "ноя.",
            12 => "дек.",
            _ => string.Empty
        };
    }
}