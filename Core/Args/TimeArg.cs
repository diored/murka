using DioRed.Murka.Core.Entities;

namespace DioRed.Murka.Core.Args;

internal class TimeArg(TimeOnly time) : Arg
{
    public TimeOnly Value { get; } = time;

    public override string ToString()
    {
        return Value.ToString(CommonValues.TimeFormat);
    }

    public static implicit operator TimeOnly(TimeArg arg) => arg.Value;
}