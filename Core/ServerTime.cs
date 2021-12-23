namespace DioRed.Murka.Core;

public static class ServerTime
{
    public static DateTime GetCurrent()
    {
        return GetServerTime(DateTime.UtcNow);
    }

    public static DateTime GetServerTime(DateTime dateTime)
    {
        return dateTime.ToUniversalTime().AddHours(3); // Moscow time zone UTC+3
    }
}
