using System.Globalization;

namespace DioRed.Murka.Common;

public struct ServerTime
{
    public const string DateFormat = "yyyy-MM-dd";
    public const string TimeFormat = "HH:mm";
    public const string DateTimeFormat = $"{DateFormat} {TimeFormat}";

    private static readonly TimeSpan MoscowTimeZone = TimeSpan.FromHours(3);

    public ServerTime(DateTime dt)
    {
        Date = DateOnly.FromDateTime(dt);
        Time = TimeOnly.FromDateTime(dt);
    }

    public ServerTime(DateOnly date, TimeOnly? time = default)
    {
        Date = date;
        Time = time;
    }

    public static ServerTime GetCurrent() => FromDateTime(DateTime.UtcNow);

    public static ServerTime FromDateTime(DateTime dateTime)
    {
        var dt = dateTime.ToUniversalTime() + MoscowTimeZone;
        return new ServerTime(dt);
    }

    public DateOnly Date { get; set; }
    public TimeOnly? Time { get; set; }

    public DateTime ToDateTime()
    {
        if (!Time.HasValue)
        {
            throw TimeShouldBeSetException();
        }

        return Date.ToDateTime(Time.Value);
    }

    public DateTime ToUtc()
    {
        return DateTime.SpecifyKind(ToDateTime() - MoscowTimeZone, DateTimeKind.Utc);
    }

    public override string ToString()
    {
        return Time.HasValue
            ? Date.ToDateTime(Time.Value).ToString(DateTimeFormat)
            : Date.ToString(DateFormat);
    }

    public static ServerTime Parse(string s)
    {
        DateTime dateTime;
        if (DateTime.TryParseExact(s, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
        {
            return new ServerTime(dateTime);
        }

        DateOnly date = DateOnly.ParseExact(s, DateFormat);
        return new ServerTime(date);
    }

    public static ServerTime? SafeParse(string? s)
    {
        return string.IsNullOrEmpty(s) ? null : Parse(s);
    }

    public static ServerTime operator +(ServerTime serverTime, TimeSpan timeSpan)
    {
        return new ServerTime(serverTime.ToDateTime() + timeSpan);
    }

    public static ServerTime operator -(ServerTime serverTime, TimeSpan timeSpan)
    {
        return serverTime + -timeSpan;
    }

    public static bool operator ==(ServerTime serverTime1, ServerTime serverTime2)
    {
        return serverTime1.Date == serverTime2.Date
            && serverTime1.Time == serverTime2.Time;
    }

    public static bool operator !=(ServerTime serverTime1, ServerTime serverTime2)
    {
        return !(serverTime1 == serverTime2);
    }

    public static bool operator >(ServerTime serverTime1, ServerTime serverTime2)
    {
        if (serverTime1.Date != serverTime2.Date)
        {
            return serverTime1.Date > serverTime1.Date;
        }

        if (serverTime1.Time.HasValue && serverTime2.Time.HasValue)
        {
            return serverTime1.Time.Value > serverTime2.Time.Value;
        }

        return false;
    }

    public static bool operator <(ServerTime serverTime1, ServerTime serverTime2)
    {
        if (serverTime1.Date != serverTime2.Date)
        {
            return serverTime1.Date < serverTime1.Date;
        }

        if (serverTime1.Time.HasValue && serverTime2.Time.HasValue)
        {
            return serverTime1.Time.Value < serverTime2.Time.Value;
        }

        return false;
    }

    public static bool operator >=(ServerTime serverTime1, ServerTime serverTime2)
    {
        return serverTime1 == serverTime2 || serverTime1 > serverTime2;
    }

    public static bool operator <=(ServerTime serverTime1, ServerTime serverTime2)
    {
        return serverTime1 == serverTime2 || serverTime1 < serverTime2;
    }

    private static Exception TimeShouldBeSetException() => new InvalidOperationException($"{nameof(Time)} should be set");

    public override bool Equals(object? obj) => obj is ServerTime st && st == this;

    public override int GetHashCode()
    {
        int hash = Date.GetHashCode();
        if (Time.HasValue)
        {
            hash ^= Time.Value.GetHashCode();
        }

        return hash;
    }
}
