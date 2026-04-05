namespace DioRed.Murka.Core.Entities;

public static class CommonValues
{
    public const string DateFormat = "yyyy-MM-dd";
    public const string TimeFormat = "HH:mm";
    public const string DateTimeFormat = $"{DateFormat} {TimeFormat}";

    public static readonly TimeZoneInfo ServerTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
}