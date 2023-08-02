using System.Collections;

using DioRed.Murka.Core.Entities;

namespace DioRed.Murka.Core;

internal class Args : IReadOnlyList<Arg>
{
    private readonly Arg[] _args;

    public Args(params string[] args)
    {
        _args = args.Select(arg => new Arg(arg)).ToArray();
        Types = _args.Select(arg => arg.Type).ToArray().AsReadOnly();
    }

    public Arg this[int index] => _args[index];

    public int Count => _args.Length;
    public IList<ArgType> Types { get; }

    public int? IntOrDefault(int index) => _args.Length > index && _args[index].Type == ArgType.Int ? _args[index].IntValue : null;
    public long? LongOrDefault(int index) => _args.Length > index && _args[index].Type == ArgType.Long ? _args[index].LongValue : null;
    public TimeOnly? TimeOrDefault(int index) => _args.Length > index && _args[index].Type == ArgType.Time ? _args[index].TimeValue : null;
    public ServerDateTime? DateTimeOrDefault(int index) => _args.Length > index && _args[index].Type == ArgType.DateTime ? _args[index].DateTimeValue : null;
    public string? StringOrDefault(int index) => _args.Length > index && _args[index].Type != ArgType.Empty ? (string)_args[index] : null;

    public static Args Parse(string s)
    {
        string[] args = string.IsNullOrEmpty(s)
            ? Array.Empty<string>()
            : s.Split('|', StringSplitOptions.TrimEntries);

        return new Args(args);
    }

    public IEnumerator<Arg> GetEnumerator()
    {
        return ((IEnumerable<Arg>)_args).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _args.GetEnumerator();
    }
}