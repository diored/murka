namespace DioRed.Murka.Core.Args;

internal class IntArg(int value) : Arg
{
    public int Value { get; } = value;

    public static implicit operator int(IntArg arg) => arg.Value;
}