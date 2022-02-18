﻿using DioRed.Murka.Common;
using DioRed.Murka.Core.Contracts;
using DioRed.Murka.Core.Entities;

namespace DioRed.Murka.Core;

public class Logic : ILogic
{
    private readonly IStorageEndpoint _storageEndpoint;

    public Logic(IStorageEndpoint storageEndpoint)
    {
        _storageEndpoint = storageEndpoint;
    }

    public void Cleanup()
    {
        const string level = "cleanup";

        Log(level, "Storage cleanup started");

        var removed = _storageEndpoint.Promocodes.RemoveOutdated();
        if (removed.Any())
        {
            Log(level, $"Outdated promocodes removed", removed);
        }

        removed = _storageEndpoint.Events.RemoveOutdated();
        if (removed.Any())
        {
            Log(level, $"Outdated events removed", removed);
        }

        Log(level, "Storage cleanup finished");
    }

    public ICollection<Event> GetActiveEvents(ServerTime serverTime)
    {
        return _storageEndpoint.Events.GetActive(serverTime);
    }

    public ICollection<Promocode> GetActivePromocodes(ServerTime serverTime)
    {
        return _storageEndpoint.Promocodes.GetActive(serverTime);
    }

    public Daily GetDaily(DateOnly date)
    {
        return _storageEndpoint.Dailies.Get(date);
    }

    public ICollection<DayEvent> GetDayEvents(DateOnly date)
    {
        List<DayEvent> dayEvents = new();

        DayOfWeek dow = date.DayOfWeek;

        if (dow == DayOfWeek.Monday)
        {
            dayEvents.Add(new DayEvent(new TimeOnly(20, 20), "Битва династий (внутрисерверная)"));
        }

        if (dow == DayOfWeek.Friday)
        {
            dayEvents.Add(new DayEvent(new TimeOnly(20, 20), "Битва династий (межсерверная)"));
        }

        // Северные земли
        dayEvents.Add(dow switch
        {
            DayOfWeek.Tuesday => new DayEvent(new TimeOnly(20, 0), "Ледяной штурм (СЗ)"),
            DayOfWeek.Monday or DayOfWeek.Friday => new DayEvent(new TimeOnly(20, 20), "Битва за ледник (войско богов)"),
            _ => new DayEvent(new TimeOnly(20, 20), "Битва за ледник (армия севера)")
        });

        if (dow == DayOfWeek.Wednesday)
        {
            dayEvents.Add(new DayEvent(new TimeOnly(20, 0), "Битва за ресурсы (БЗР)"));
        }

        if (dow == DayOfWeek.Thursday)
        {
            dayEvents.Add(new DayEvent(new TimeOnly(20, 0), "Конкурс ремесленников"));
        }

        if (dow == DayOfWeek.Saturday)
        {
            dayEvents.Add(new DayEvent(new TimeOnly(17, 30), "Клан-холл"));
        }

        dayEvents = dayEvents
            .OrderBy(de => de.Time)
            .ToList();

        return dayEvents;
    }

    public Northlands GetNorthLands(DateOnly date)
    {
        DayOfWeek dayOfWeek = date.DayOfWeek;

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
        const string level = "chat";

        try
        {
            _storageEndpoint.Chats.Add(chatInfo);

            Log(level, "Chat added", chatInfo);
        }
        catch (Exception ex)
        {
            Log(level, "Chat adding failed", chatInfo, ex);

            throw;
        }
    }

    public void RemoveChat(ChatInfo chatInfo)
    {
        const string level = "chat";

        try
        {
            _storageEndpoint.Chats.Remove(chatInfo);

            Log(level, "Chat removed", chatInfo);
        }
        catch (Exception ex)
        {
            Log(level, "Chat removing failed", chatInfo, ex);

            throw;
        }
    }

    public void Log(string level, string message, object? argumentObject = null, Exception? exception = null)
    {
        _storageEndpoint.Log.Log(level, message, argumentObject, exception);
    }

    public BinaryData GetCalendar()
    {
        return _storageEndpoint.Images.Get("daily.png");
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

    public void AddEvent(Event newEvent)
    {
        _storageEndpoint.Events.AddNew(newEvent);
    }

    public void AddPromocode(Promocode newPromocode)
    {
        _storageEndpoint.Promocodes.AddNew(newPromocode);
    }

    private static readonly string[] _greetings =
    {
        "Привет! =)",
        "Мяу :-)",
        ",,,==(^.^)==,,,",
        "Рада видеть ;)",
        "И вам здравствуйте!",
        "Мурр",
        "Чао )",
        "Фрррр"
    };
}
