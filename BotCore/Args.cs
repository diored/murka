using DioRed.Murka.Core.Entities;

namespace DioRed.Murka.BotCore;

internal class Args
{
    private readonly string[] _args;

    public Args(params string[] args)
    {
        _args = args;
    }

    public string this[int index] => _args[index];

    public int Count => _args.Length;

    public int Int(int index) => int.Parse(_args[index]);
    public int? IntOrDefault(int index) => string.IsNullOrEmpty(_args[index]) ? null : Int(index);
    public string? StringOrDefault(int index) => string.IsNullOrEmpty(_args[index]) ? null : _args[index];
    public TimeOnly Time(int index) => TimeOnly.ParseExact(_args[index], CommonValues.TimeFormat);
    public TimeOnly? TimeOrDefault(int index) => string.IsNullOrEmpty(_args[index]) ? null : Time(index);
    public ServerDateTime DateTime(int index) => ServerDateTime.Parse(_args[index]);
    public ServerDateTime? DateTimeOrDefault(int index) => string.IsNullOrEmpty(_args[index]) ? null : DateTime(index);

    public static Args Parse(string s)
    {
        return new Args(s.Split('|', StringSplitOptions.TrimEntries));
    }
}