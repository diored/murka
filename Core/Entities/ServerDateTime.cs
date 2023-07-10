using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace DioRed.Murka.Core.Entities;

public record struct ServerDateTime(DateOnly Date, TimeOnly? Time = null)
{
    public ServerDateTime(DateTime dateTime)
        : this(DateOnly.FromDateTime(dateTime), TimeOnly.FromDateTime(dateTime))
    {
    }

    public static ServerDateTime FromDateTime(DateTime dateTime)
    {
        var dt = dateTime.ToUniversalTime() + CommonValues.ServerTimeZoneShift;

        return new ServerDateTime(dt);
    }

    public static ServerDateTime GetCurrent()
    {
        return FromDateTime(DateTime.UtcNow);
    }

    public static ServerDateTime Parse(string s)
    {
        if (DateTime.TryParseExact(s, CommonValues.DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
        {
            return new ServerDateTime(dateTime);
        }

        DateOnly date = DateOnly.ParseExact(s, CommonValues.DateFormat);

        return new ServerDateTime(date);
    }

    public static bool TryParse(string s, [NotNullWhen(true)] out ServerDateTime? value)
    {
        if (DateTime.TryParseExact(s, CommonValues.DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
        {
            value = new ServerDateTime(dateTime);
            return true;
        }

        if (DateOnly.TryParseExact(s, CommonValues.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly dateOnly))
        {
            value = new ServerDateTime(dateOnly);
            return true;
        }

        value = default;
        return false;
    }

    public static ServerDateTime? ParseOrDefault(string? s)
    {
        return string.IsNullOrEmpty(s) ? null : Parse(s);
    }

    public readonly DateTime ToDateTime()
    {
        if (!Time.HasValue)
        {
            throw new InvalidOperationException($"{nameof(Time)} should be set");
        }

        return Date.ToDateTime(Time.Value);
    }

    public readonly DateTime ToUtc()
    {
        return DateTime.SpecifyKind(ToDateTime() - CommonValues.ServerTimeZoneShift, DateTimeKind.Utc);
    }

    public override readonly string ToString()
    {
        return Time.HasValue
            ? Date.ToDateTime(Time.Value).ToString(CommonValues.DateTimeFormat)
            : Date.ToString(CommonValues.DateFormat);
    }

    public static ServerDateTime operator +(ServerDateTime serverTime, TimeSpan timeSpan)
    {
        return new ServerDateTime(serverTime.ToDateTime() + timeSpan);
    }

    public static ServerDateTime operator -(ServerDateTime serverTime, TimeSpan timeSpan)
    {
        return serverTime + -timeSpan;
    }

    public static bool operator >(ServerDateTime serverTime1, ServerDateTime serverTime2)
    {
        if (serverTime1.Date != serverTime2.Date)
        {
            return serverTime1.Date > serverTime2.Date;
        }

        if (serverTime1.Time.HasValue && serverTime2.Time.HasValue)
        {
            return serverTime1.Time.Value > serverTime2.Time.Value;
        }

        return false;
    }

    public static bool operator <(ServerDateTime serverTime1, ServerDateTime serverTime2)
    {
        if (serverTime1.Date != serverTime2.Date)
        {
            return serverTime1.Date < serverTime2.Date;
        }

        if (serverTime1.Time.HasValue && serverTime2.Time.HasValue)
        {
            return serverTime1.Time.Value < serverTime2.Time.Value;
        }

        return false;
    }

    public static bool operator >=(ServerDateTime serverTime1, ServerDateTime serverTime2)
    {
        return serverTime1 == serverTime2 || serverTime1 > serverTime2;
    }

    public static bool operator <=(ServerDateTime serverTime1, ServerDateTime serverTime2)
    {
        return serverTime1 == serverTime2 || serverTime1 < serverTime2;
    }
}
