using System.Text;

using DioRed.Murka.Core.Entities;
using DioRed.Vermilion;

using Microsoft.Extensions.Logging;

namespace DioRed.Murka.Core;

public interface ILogic
{
    Task<string> GetRandomGreetingAsync();

    Task<Daily> GetDailyAsync(DateOnly date);
    Task SetDailyAsync(int monthNumber, string dailies);

    Task<ICollection<DayEvent>> GetDayEventsAsync(DateOnly date, ChatId chatId);
    Task AddDayEventAsync(string name, string occurrence, TimeOnly time, ChatId? chatId);

    Task<ICollection<Event>> GetActiveEventsAsync();
    Task AddEventAsync(Event newEvent);

    Task<ICollection<Promocode>> GetActivePromocodesAsync();
    Task AddPromocodeAsync(Promocode promocode);
    Task UpdatePromocodeAsync(Promocode promocode);
    Task RemovePromocodeAsync(string code);

    Task<Northlands> GetNorthLandsAsync(DateOnly date);

    Task<string> GetLinkAsync(string id);

    Task CleanupAsync();

    Task<string> BuildAgendaAsync(ChatId chatId, DateOnly date);
    string GetDaytimeGreeting(TimeOnly time);
}

internal class Logic(
    IApiFacade api,
    ILoggerFactory logger
) : ILogic
{
    private readonly ILogger _logger = logger.CreateLogger("Logic");

    public async Task CleanupAsync()
    {
        _logger.LogInformation(Events.CleanupStarted, "Storage cleanup started");
        try
        {
            await api.CleanupPromocodes();
            await api.CleanupEvents();

            _logger.LogInformation(Events.CleanupFinished, "Storage cleanup finished");
        }
        catch (Exception ex)
        {
            _logger.LogError(Events.CleanupFailed, ex, "Storage cleanup failed");
        }
    }

    public async Task<string> BuildAgendaAsync(
        ChatId chatId,
        DateOnly date)
    {
        StringBuilder builder = new();

        // Greeting
        builder.AppendLine(
            GetDaytimeGreeting(
                ServerDateTime.GetCurrent().Time!.Value
            )
        );

        // Daily
        var daily = await GetDailyAsync(date);
        if (daily?.Definition != null)
        {
            builder.AppendLine(
                $"Ежа: <b>{daily.Quest}</b> — {daily.Definition}."
            );
        }

        // Northlands
        Northlands northlands = await GetNorthLandsAsync(date);

        builder.AppendLine($"""
            Северные земли:
            — войско богов: <b>{northlands.Gods}</b>.
            — армия севера: <b>{northlands.North}</b>.
            """
        );

        // DayEvents
        builder
            .Append("Ивенты ")
            .Append(date.DayOfWeek switch
            {
                DayOfWeek.Monday => "в <i>понедельник</i>",
                DayOfWeek.Tuesday => "во <i>вторник</i>",
                DayOfWeek.Wednesday => "в <i>среду</i>",
                DayOfWeek.Thursday => "в <i>четверг</i>",
                DayOfWeek.Friday => "в <i>пятницу</i>",
                DayOfWeek.Saturday => "в <i>субботу</i>",
                _ => "в <i>воскресенье</i>"
            });

        ICollection<DayEvent> dayEvents = await GetDayEventsAsync(date, chatId);

        if (dayEvents.Count == 0)
        {
            builder.Append(" <b>отсутствуют</b>");
        }
        else
        {
            builder.Append(':');

            foreach (DayEvent dayEvent in dayEvents)
            {
                builder.Append($"""

                    — <b>{dayEvent.Time:HH:mm}</b> — {dayEvent.Name}
                    """
                );
            }
        }

        ICollection<Promocode> activePromocodes = await GetActivePromocodesAsync();

        ICollection<Promocode> expiringPromocodes =
        [
            ..activePromocodes.Where(p => p.ValidTo?.Date == date)
        ];

        if (expiringPromocodes.Count != 0)
        {
            builder.AppendLine($"""


                Последний день активации промокод{(expiringPromocodes.Count > 1 ? "ов" : "а")} {string.Join(
                    ", ",
                    expiringPromocodes.Select(ep => $"<code>{ep.Code}</code>")
                )}.
                """);
        }

        return builder.ToString();
    }

    public string GetDaytimeGreeting(TimeOnly time)
    {
        return time.Hour switch
        {
            < 5 => "Доброй ночи! =^.^=",
            < 12 => "Доброе утро! =^.^=",
            < 18 => "Добрый день! =^.^=",
            >= 18 => "Добрый вечер! =^.^="
        };
    }

    public async Task<ICollection<Event>> GetActiveEventsAsync()
    {
        return await api.GetActiveEvents();
    }

    public async Task<ICollection<Promocode>> GetActivePromocodesAsync()
    {
        return await api.GetActivePromocodes();
    }

    public async Task<Daily> GetDailyAsync(DateOnly date)
    {
        return await api.GetDaily(date.ToString(CommonValues.DateFormat));
    }

    public async Task<ICollection<DayEvent>> GetDayEventsAsync(DateOnly date, ChatId chatId)
    {
        return await api.GetDayEvents(date.ToString(CommonValues.DateFormat), chatId);
    }

    public async Task<Northlands> GetNorthLandsAsync(DateOnly date)
    {
        return await api.GetNorthlands(date.ToString(CommonValues.DateFormat));
    }

    public async Task<string> GetRandomGreetingAsync()
    {
        return await api.GetRandomGreeting();
    }

    public async Task AddEventAsync(
        Event newEvent
    )
    {
        await api.AddEvent(
            newEvent.Name,
            newEvent.ValidFrom?.ToString(),
            newEvent.ValidTo?.ToString()
        );
    }

    public async Task AddPromocodeAsync(Promocode promocode)
    {
        await api.AddPromocode(
            promocode.Code,
            promocode.ValidFrom?.ToString(),
            promocode.ValidTo?.ToString(),
            promocode.Content
        );
    }

    public async Task AddDayEventAsync(string name, string occurrence, TimeOnly time, ChatId? chatId)
    {
        await api.AddDayEvent(
            name,
            occurrence,
            time.ToString(CommonValues.TimeFormat),
            chatId
        );
    }

    public async Task SetDailyAsync(int month, string dailies)
    {
        await api.SetDailyMonth(month, dailies);
    }

    public async Task UpdatePromocodeAsync(Promocode promocode)
    {
        await api.UpdatePromocode(
            promocode.Code,
            promocode.ValidFrom?.ToString(),
            promocode.ValidTo?.ToString(),
            promocode.Content
        );
    }

    public async Task RemovePromocodeAsync(string code)
    {
        await api.RemovePromocode(code);
    }

    public async Task<string> GetLinkAsync(string id)
    {
        return await api.GetLink(id);
    }
}