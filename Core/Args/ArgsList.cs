using System.Collections;

namespace DioRed.Murka.Core.Args;

internal class ArgsList(Arg?[] args) : IReadOnlyList<Arg?>
{
    public ArgsList(params string[] args)
        : this(args.Select(Arg.Parse).ToArray())
    {
    }

    public Arg? this[int index] => index < args.Length ? args[index] : null;

    public int Count => args.Length;

    public ArgsList ExpandTo(int count)
    {
        if (count < Count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Argument list can't be expanded to lesser item count it has");
        }

        Arg?[] expandedArgs = [.. args, .. Enumerable.Repeat((Arg?)null, count - Count)];

        return new ArgsList(expandedArgs);
    }

    public static ArgsList Parse(string argsString)
    {
        string[] args = string.IsNullOrEmpty(argsString)
            ? []
            : argsString.Split('|', StringSplitOptions.TrimEntries);

        return new ArgsList(args);
    }

    public IEnumerator<Arg?> GetEnumerator()
    {
        return ((IEnumerable<Arg?>)args).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return args.GetEnumerator();
    }
}