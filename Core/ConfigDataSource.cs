using DioRed.Murka.Core.Entities;

using Microsoft.Extensions.Configuration;

namespace DioRed.Murka.Core;

public class ConfigDataSource : IDataSource
{
    private static IConfiguration _configuration = default!;

    public ConfigDataSource(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Event[] GetActiveEvents(DateTime dateTime)
    {
        return _configuration.GetSection("events").GetChildren()
            .Select(x => new Event(x["Name"], DateTime.Parse(x["Ends"])))
            .Where(e => e.Ends >= dateTime)
            .OrderBy(e => e.Ends)
            .ToArray();
    }

    public Promocode[] GetActivePromocodes(DateTime dateTime)
    {
        var today = DateOnly.FromDateTime(dateTime);

        return _configuration.GetSection("promo").GetChildren()
            .Select(x => new Promocode(DateOnly.Parse(x["ValidTo"]), x.Key, x["Content"]))
            .Where(p => p.ValidTo >= today)
            .OrderBy(p => p.ValidTo)
            .ToArray();
    }

    public Daily? GetDaily(DateTime dateTime)
    {
        string key = $"daily:{dateTime:MM:dd}";
        string dailyKey = _configuration[key];
        return dailyKey != null ? _dailies[dailyKey] : null;
    }

    public IEnumerable<string> GetDayEvents(DateTime dateTime)
    {
        DayOfWeek dow = dateTime.DayOfWeek;

        if (dow == DayOfWeek.Monday)
        {
            yield return "Битва династий (внутрисерверная)";
        }

        if (dow == DayOfWeek.Friday)
        {
            yield return "Битва династий (межсерверная)";
        }

        if (dow == DayOfWeek.Tuesday)
        {
            yield return "Ледяной штурм (СЗ)";
        }

        if (dow is DayOfWeek.Monday or DayOfWeek.Friday)
        {
            yield return "Битва за ледник (войско богов)";
        }

        if (dow is DayOfWeek.Wednesday or DayOfWeek.Thursday or DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            yield return "Битва за ледник (армия севера)";
        }

        if (dow == DayOfWeek.Wednesday)
        {
            yield return "Битва за ресурсы (БЗР)";
        }

        if (dow == DayOfWeek.Thursday)
        {
            yield return "Конкурс ремесленников";
        }

        if (dow == DayOfWeek.Saturday)
        {
            yield return "Клан-холл";
        }
    }

    public string GetNorth(DateTime dateTime, NorthArmy army)
    {
        DayOfWeek dayOfWeek = dateTime.DayOfWeek;
        if (dayOfWeek == DayOfWeek.Tuesday)
        {
            return "Ледяной штурм";
        }

        var godsEvents = GetNorthEventForGods(dayOfWeek);
        var events = (army == NorthArmy.Gods)
            ? godsEvents
            : Enumerable.Range(16, 8).Except(godsEvents).ToArray();

        return string.Join(", ", events.Select(e => $"{e}:00"));
    }

    public string GetRandomGreeting()
    {
        return GetRandomItem(_greetings);
    }

    private static int[] GetNorthEventForGods(DayOfWeek dayOfWeek)
    {
        return dayOfWeek switch
        {
            DayOfWeek.Monday or DayOfWeek.Friday => new[] { 18, 19, 20, 21 },
            DayOfWeek.Wednesday or DayOfWeek.Thursday => new[] { 18, 19, 21, 22 },
            DayOfWeek.Saturday or DayOfWeek.Sunday => new[] { 18, 19 },
            _ => Array.Empty<int>()
        };
    }

    private static T GetRandomItem<T>(IList<T> items)
    {
        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        if (items.Count == 0)
        {
            throw new ArgumentException(nameof(items) + " should contain at least one item");
        }

        return items[Random.Shared.Next(items.Count)];
    }

    private static readonly IDictionary<string, Daily> _dailies = new Dictionary<string, Daily>
    {
        ["weapon"] = new("weapon", "Оружие (ПВ2 / ПП / МИ / ГШ)"),
        ["armor"] = new("armor", "Доспех (ПВ1 / СЦ / ХХ 4-1 / ХХ 4-2)"),
        ["relic"] = new("relic", "Реликвия (ХС / ЛА / ДР)")
    };

    private static readonly string[] _greetings =
    {
        "Привет! =)",
        "Мяу :-)",
        ",,,==(^.^)==,,,",
        "Рада видеть ;)",
        "И вам здравствуйте!",
        "Мурр"
    };
}
