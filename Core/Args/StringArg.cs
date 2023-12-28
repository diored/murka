namespace DioRed.Murka.Core.Args;

internal class StringArg(string value) : Arg
{
    public string Value { get; } = value;

    public override string ToString()
    {
        return Value;
    }

    public static implicit operator string(StringArg arg) => arg.Value;
}