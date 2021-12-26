using DioRed.Murka.Core.Contracts;
using DioRed.Murka.Core.Entities;
using DioRed.Murka.Storage.AzureTables;
using DioRed.Murka.Storage.Contracts;

using Microsoft.Extensions.Configuration;

namespace DioRed.Murka.Core;

public class Logic : ILogic
{
    private readonly IStorageEndpoint _storageEndpoint;

    public Logic(IConfiguration configuration)
    {
        _storageEndpoint = new StorageEndpoint(configuration["data:account"], configuration["data:key"]);
    }

    public void Cleanup()
    {
        _storageEndpoint.Promocodes.RemoveOutdated();
        _storageEndpoint.Events.RemoveOutdated();
    }

    public ICollection<Event> GetActiveEvents(DateTime dateTime)
    {
        return _storageEndpoint.Events.GetActive(dateTime);
    }

    public ICollection<Promocode> GetActivePromocodes(DateTime dateTime)
    {
        return _storageEndpoint.Promocodes.GetActive(dateTime);
    }

    public Daily GetDaily(DateTime dateTime)
    {
        return _storageEndpoint.Dailies.Get(dateTime);
    }

    public ICollection<string> GetDayEvents(DateTime dateTime)
    {
        List<string> dayEvents = new();

        DayOfWeek dow = dateTime.DayOfWeek;

        if (dow == DayOfWeek.Monday)
        {
            dayEvents.Add("20:20 — Битва династий (внутрисерверная)");
        }

        if (dow == DayOfWeek.Friday)
        {
            dayEvents.Add("20:20 — Битва династий (межсерверная)");
        }

        // Северные земли
        dayEvents.Add(dow switch
        {
            DayOfWeek.Tuesday => "20:00 — Ледяной штурм (СЗ)",
            DayOfWeek.Monday or DayOfWeek.Friday => "20:20 — Битва за ледник (войско богов)",
            _ => "20:20 — Битва за ледник (армия севера)"
        });

        if (dow == DayOfWeek.Wednesday)
        {
            dayEvents.Add("20:00 — Битва за ресурсы (БЗР)");
        }

        if (dow == DayOfWeek.Thursday)
        {
            dayEvents.Add("20:00 — Конкурс ремесленников");
        }

        if (dow == DayOfWeek.Saturday)
        {
            dayEvents.Add("17:30 — Клан-холл");
        }

        dayEvents.Sort();

        return dayEvents;
    }

    public Northlands GetNorthLands(DateTime dateTime)
    {
        DayOfWeek dayOfWeek = dateTime.DayOfWeek;

        if (dayOfWeek == DayOfWeek.Tuesday)
        {
            return new Northlands("Ледяной штурм", "Ледяной штурм");
        }

        int[] godsEvents = dayOfWeek switch
        {
            DayOfWeek.Monday or DayOfWeek.Friday => new[] { 18, 19, 20, 21 },
            DayOfWeek.Wednesday or DayOfWeek.Thursday => new[] { 18, 19, 21, 22 },
            DayOfWeek.Saturday or DayOfWeek.Sunday => new[] { 18, 19 },
            _ => Array.Empty<int>()
        };

        int[] northEvents = Enumerable.Range(16, 8) // 16..23
            .Except(godsEvents)
            .ToArray();

        return new Northlands(
            string.Join(", ", northEvents.Select(e => $"{e}:00")),
            string.Join(", ", godsEvents.Select(e => $"{e}:00")));
    }

    public string GetRandomGreeting()
    {
        return GetRandomItem(_greetings);
    }

    public ICollection<ChatInfo> GetChats()
    {
        return _storageEndpoint.Chats.GetTelegramChats();
    }

    public void AddChat(ChatInfo chatInfo)
    {
        _storageEndpoint.Chats.Add(chatInfo);
    }

    public void RemoveChat(ChatInfo chatInfo)
    {
        _storageEndpoint.Chats.Remove(chatInfo);
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
