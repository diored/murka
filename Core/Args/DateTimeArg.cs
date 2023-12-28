using DioRed.Murka.Core.Entities;

namespace DioRed.Murka.Core.Args;

internal class DateTimeArg(ServerDateTime dateTime) : Arg
{
    public ServerDateTime Value { get; } = dateTime;

    public static implicit operator ServerDateTime(DateTimeArg arg) => arg.Value;
}