namespace DioRed.Murka.Core;

public static class ServerTime
{
    public static DateTime GetCurrent()
    {
        return DateTime.UtcNow.AddHours(3); // Moscow time zone UTC+3
    }
}
