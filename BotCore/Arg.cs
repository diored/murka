using DioRed.Murka.Core.Entities;

namespace DioRed.Murka.BotCore;

internal class Arg
{
    private readonly string _stringValue;
    private readonly int? _intValue;
    private readonly long? _longValue;
    private readonly TimeOnly? _timeValue;
    private readonly ServerDateTime? _dateTimeValue;

    public Arg(string? value)
    {
        _stringValue = value?.ToString() ?? string.Empty;

        if (value is null)
        {
            Value = string.Empty;
            Type = ArgType.Empty;
            return;
        }

        if (int.TryParse(value, out int intValue))
        {
            _intValue = intValue;
            Value = intValue;
            Type = ArgType.Int;
            return;
        }

        if (long.TryParse(value, out long longValue))
        {
            _longValue = longValue;
            Value = longValue;
            Type = ArgType.Long;
            return;
        }

        if (TimeOnly.TryParseExact(value, CommonValues.TimeFormat, out TimeOnly timeValue))
        {
            _timeValue = timeValue;
            Value = timeValue;
            Type = ArgType.Time;
            return;
        }

        if (ServerDateTime.ParseOrDefault(value) is { } dateTimeValue)
        {
            _dateTimeValue = dateTimeValue;
            Value = dateTimeValue;
            Type = ArgType.DateTime;
            return;
        }

        Value = value;
        Type = ArgType.String;
    }

    public object Value { get; }
    public ArgType Type { get; }

    public int IntValue => (int)Value;
    public long LongValue => (long)Value;
    public TimeOnly TimeValue => (TimeOnly)Value;
    public ServerDateTime DateTimeValue => (ServerDateTime)Value;
    public string StringValue => _stringValue;

    public static implicit operator string(Arg arg)
    {
        return arg._stringValue;
    }
}