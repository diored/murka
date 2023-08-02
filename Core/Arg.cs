using DioRed.Murka.Core.Entities;

namespace DioRed.Murka.Core;

internal class Arg
{
    private readonly string _stringValue;
    private readonly int? _intValue;
    private readonly long? _longValue;
    private readonly TimeOnly? _timeValue;
    private readonly ServerDateTime? _dateTimeValue;

    public Arg(string? value)
    {
        if (value is null)
        {
            Value = string.Empty;
            Type = ArgType.Empty;
            _stringValue = string.Empty;
            return;
        }

        _stringValue = value.ToString();


        if (int.TryParse(value, out int intValue))
        {
            _intValue = intValue;
            _longValue = intValue;
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

        if (ServerDateTime.TryParse(value, out var dateTimeValue))
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

#pragma warning disable CS8629 // Nullable value type may be null.
    public int IntValue => _intValue.Value;
    public long LongValue => _longValue.Value;
    public TimeOnly TimeValue => _timeValue.Value;
    public ServerDateTime DateTimeValue => _dateTimeValue.Value;
#pragma warning restore CS8629 // Nullable value type may be null.

    public string StringValue => _stringValue;

    public static implicit operator string(Arg arg)
    {
        return arg._stringValue;
    }
}