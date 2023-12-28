using DioRed.Murka.Core.Entities;

namespace DioRed.Murka.Core.Args;

internal abstract class Arg
{
    public required string Text { get; init; }

    public static Arg? Parse(string? s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return null;
        }

        if (int.TryParse(s, out int intValue))
        {
            return new IntArg(intValue) { Text = s };
        }

        if (TimeOnly.TryParseExact(s, CommonValues.TimeFormat, out TimeOnly timeValue))
        {
            return new TimeArg(timeValue) { Text = s };
        }

        if (ServerDateTime.TryParse(s, out var dateTimeValue))
        {
            return new DateTimeArg(dateTimeValue.Value) { Text = s };
        }

        return new StringArg(s) { Text = s };
    }

    public override string ToString()
    {
        return Text;
    }

    public static implicit operator string(Arg arg) => arg.Text;
}