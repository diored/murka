using System.Text;

using DioRed.Murka.Core.Entities;
using DioRed.Murka.Core.Modules;
using DioRed.Vermilion;

using Microsoft.Extensions.Logging;

namespace DioRed.Murka.Core;

public interface ILogic
{
    Task<string> GetRandomGreetingAsync();

    Task<Daily> GetDailyAsync(DateOnly date);

    Task<ICollection<DayEvent>> GetDayEventsAsync(DateOnly date, ChatId chatId);
    Task AddDayEventAsync(string name, Occurrence occurrence, ChatId? chatId);

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

public class Logic(
    IDailyModule dailyModule,
    IDayEventsModule dayEventsModule,
    IEventsModule eventsModule,
    IGreetingModule greetingModule,
    ILinksModule linksModule,
    INorthModule northModule,
    IPromocodesModule promocodesModule,
    ILoggerFactory logger
) : ILogic
{
    private readonly ILogger _logger = logger.CreateLogger("Logic");

    public async Task CleanupAsync()
    {
        _logger.LogInformation(MurkaEvents.CleanupStarted, "Storage cleanup started");
        try
        {
            promocodesModule.Cleanup();
            eventsModule.Cleanup();

            _logger.LogInformation(MurkaEvents.CleanupFinished, "Storage cleanup finished");
        }
        catch (Exception ex)
        {
            _logger.LogError(MurkaEvents.CleanupFailed, ex, "Storage cleanup failed");
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
        return eventsModule.GetActiveEvents();
    }

    public async Task<ICollection<Promocode>> GetActivePromocodesAsync()
    {
        return promocodesModule.GetActive();
    }

    public async Task<Daily> GetDailyAsync(DateOnly date)
    {
        return dailyModule.Get(date.ToString(CommonValues.DateFormat));
    }

    public async Task<ICollection<DayEvent>> GetDayEventsAsync(DateOnly date, ChatId chatId)
    {
        return dayEventsModule.Get(date.ToString(CommonValues.DateFormat), chatId);
    }

    public async Task<Northlands> GetNorthLandsAsync(DateOnly date)
    {
        return northModule.GetNorth(date.ToString(CommonValues.DateFormat));
    }

    public async Task<string> GetRandomGreetingAsync()
    {
        return greetingModule.GetRandomGreeting();
    }

    public async Task AddEventAsync(Event newEvent)
    {
        eventsModule.AddEvent(newEvent);
    }

    public async Task AddPromocodeAsync(Promocode promocode)
    {
        promocodesModule.Add(promocode);
    }

    public async Task AddDayEventAsync(string name, Occurrence occurrence, ChatId? chatId)
    {
        string occurrenceString = occurrence switch
        {
            DailyOccurrence => "daily",
            WeeklyOccurrence wo => $"weekly:{(int)wo.DayOfWeek}",
            _ => throw new InvalidOperationException("Unexpected occurrence value")
        };

        dayEventsModule.Add(new DayEvent(
            name,
            chatId,
            occurrence.Time,
            occurrenceString
        ));
    }

    public async Task UpdatePromocodeAsync(Promocode promocode)
    {
        promocodesModule.Update(promocode);
    }

    public async Task RemovePromocodeAsync(string code)
    {
        promocodesModule.Remove(code);
    }

    public async Task<string> GetLinkAsync(string id)
    {
        return linksModule.Get(id) ?? throw new KeyNotFoundException("Link not found");
    }
}