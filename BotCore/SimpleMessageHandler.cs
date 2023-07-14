using System.Text;

using DioRed.Murka.Core;
using DioRed.Murka.Core.Entities;
using DioRed.Murka.Graphics;
using DioRed.Vermilion;
using DioRed.Vermilion.Handlers;

using Microsoft.Extensions.Logging;

namespace DioRed.Murka.BotCore;

public partial class SimpleMessageHandler : MessageHandlerBase
{
    private readonly ILogic _logic;
    private readonly ILogger _logger;

    public SimpleMessageHandler(MessageContext messageContext, ILogic logic, ILoggerFactory logger)
        : base(messageContext)
    {
        _logic = logic;
        _logger = logger.CreateLogger("MessageHandler");
    }

    private TimeSpan GreetingInterval { get; } = TimeSpan.FromMinutes(40);

    protected override async Task HandleMessageAsync(string message)
    {
        string[] parts = message.Split(new[] { ' ' }, 2, StringSplitOptions.TrimEntries);
        string command = parts[0];

        Args args = parts.Length > 1 ? Args.Parse(parts[1]) : new Args();

        Task? task = null;

        if (MessageContext.UserRole.HasFlag(UserRole.SuperAdmin))
        {
            task = (command, args) switch
            {
                ("/addDayEvent!", { Types: [not ArgType.Empty, ArgType.Time, not ArgType.Empty] }) => AddDayEvent(args[0], args[1].TimeValue, args[2], null),
                ("/addEvent", { Types: [not ArgType.Empty] or [not ArgType.Empty, ArgType.DateTime or ArgType.Empty] or [not ArgType.Empty, ArgType.DateTime or ArgType.Empty, ArgType.DateTime or ArgType.Empty] }) => AddEvent(args[0], args.DateTimeOrDefault(1), args.DateTimeOrDefault(2)),
                ("/addPromocode", { Types: [not ArgType.Empty, ArgType.DateTime or ArgType.Empty, not ArgType.Empty] }) => AddPromocode(args[0], args.DateTimeOrDefault(1), args[2]),
                ("/cleanup", _) => Cleanup(),
                ("/ga", { Count: > 0 }) => GlobalAnnounce(parts[1]),
                ("/ga?", { Count: > 0 }) => NoSoGlobalAnnounce(parts[1]),
                ("/setDaily", { Types: [ArgType.Int, not ArgType.Empty] }) => SetDaily(args[0].IntValue, args[1]),
                ("/log", { Count: > 0 }) => LogMessage(parts[1]),
                _ => null
            };
        }

        if (MessageContext.UserRole.HasFlag(UserRole.ChatAdmin))
        {
            task ??= (command, args) switch
            {
                ("/addDayEvent", { Types: [not ArgType.Empty, ArgType.Time, not ArgType.Empty] }) => AddDayEvent(args[0], args[1].TimeValue, args[2], MessageContext.ChatId),
                _ => null
            };
        }

        task ??= (command, args) switch
        {
            ("/daily" or "ежа", { Types: [] or [ArgType.Int] }) => ShowDaily(args.IntOrDefault(0) ?? 5),
            ("/promo" or "промокоды", _) => ShowPromocodes(),
            ("/addDayEvent", _) => AddDayEventHelp(),
            ("/sea" or "море", _) => ShowSeaAsync(),
            ("/north" or "север", _) => ShowNorthAsync(),
            ("/agenda" or "сводка", _) => ShowAgenda(ServerDateTime.GetCurrent().Date),
            ("/tomorrow" or "завтра", _) => ShowAgenda(ServerDateTime.GetCurrent().Date.AddDays(1)),
            ("/events" or "ивенты", _) => ShowEventsAsync(),
            ("/calendar" or "календарь", _) => ShowCalendarAsync(),
            _ => null
        };

        // if incoming message contains any command, log it
        if (task is not null)
        {
            _logger.LogInformation(EventIDs.MessageHandled, "Message \"{Message}\" handled as a command \"{Command}\" in {System} {Type} chat #{ChatId}", message, command, MessageContext.ChatId.System, MessageContext.ChatId.Type, MessageContext.ChatId.Id);
            await task;
        }
        else
        {
            // greetings are not commands, they shouldn't be logged
            if (_greetingsToReply.Any(greeting => message.Contains(greeting, StringComparison.InvariantCultureIgnoreCase)) &&
                !(MessageContext.ChatClient["LatestGreeting"] is DateTime latestGreeting &&
                DateTime.UtcNow - latestGreeting < GreetingInterval))
            {
                await SayMurrAsync();
            }
        }
    }

    private async Task ShowDaily(int days)
    {
        ServerDateTime serverTime = ServerDateTime.GetCurrent();

        ScheduleItem[] schedule = Enumerable.Range(0, days)
            .Select(i =>
            {
                DateOnly date = serverTime.Date.AddDays(i);
                return new ScheduleItem(date, _logic.GetDaily(date).Id);
            })
            .ToArray();

        const string folder = "forecasts";
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        string fileName = $"{folder}\\{serverTime.Date:yyyy_MM_dd}_{days}.png";

        if (!File.Exists(fileName))
        {
            File.WriteAllBytes(fileName, ForecastBuilder.BuildImage(schedule).ToArray());
        }

        using Stream fileStream = File.OpenRead(fileName);

        await ChatWriter.SendPhotoAsync(fileStream);
    }

    private async Task ShowPromocodes()
    {
        ICollection<Promocode> activePromocodes = _logic.GetActivePromocodes();

        if (!activePromocodes.Any())
        {
            await ChatWriter.SendTextAsync("Активных промокодов нет");
            return;
        }

        StringBuilder builder = new();
        builder
            .Append("<b>Активные промокоды:</b>");

        foreach (Promocode promocode in activePromocodes)
        {
            builder
                .AppendLine()
                .AppendLine()
                .AppendFormat("<code>{0}</code>", promocode.Code);

            if (promocode.ValidTo.HasValue)
            {
                builder.AppendFormat(" (до {0})", promocode.ValidTo.Value);
            }

            builder
                .AppendLine()
                .Append(promocode.Content);
        }

        await ChatWriter.SendHtmlAsync(builder.ToString());
    }

    private async Task AddDayEventHelp()
    {
        if (!MessageContext.UserRole.HasFlag(UserRole.ChatAdmin))
        {
            await ChatWriter.SendTextAsync("Добавлять ивенты может только администратор чата");
            return;
        }

        string html = """
            Синтаксис:
            <code>/addDayEvent {наименование}|{время}|{повторение}</code>
            
            Повторение:
            <code>daily</code> — ежедневно
            <code>1</code> — по понедельникам
            <code>2</code> — по вторникам
            ...
            <code>7</code> — по воскресеньям
            
            Пример:
            <code>/addDayEvent Клан-холл|17:30|6</code>
            
            <b>Примечание</b>
            Удаление ранее добавленных событий будет реализовано в одной из последующих версий.
            """;

        await ChatWriter.SendHtmlAsync(html);
    }

    private async Task AddDayEvent(string name, TimeOnly time, string occurrence, ChatId? chatId)
    {
        occurrence = occurrence.ToLowerInvariant() switch
        {
            "daily" => "daily",
            "mo" or "monday" or "1" => "weekly:1",
            "tu" or "tuesday" or "2" => "weekly:2",
            "we" or "wednesday" or "3" => "weekly:3",
            "th" or "thursday" or "4" => "weekly:4",
            "fr" or "friday" or "5" => "weekly:5",
            "sa" or "saturday" or "6" => "weekly:6",
            "su" or "sunday" or "7" or "0" => "weekly:0",
            _ => throw new ArgumentOutOfRangeException(nameof(occurrence), $"Unrecognized occurrence qualifier: {occurrence}. See /addDayEvent for possible qualifiers list.")
        };

        _logic.AddDayEvent(name, occurrence, time, chatId);

        await ChatWriter.SendTextAsync($"Day event added. Details: {(name, occurrence, time)}.");
    }

    private async Task SayMurrAsync()
    {
        MessageContext.ChatClient["LatestGreeting"] = DateTime.UtcNow;
        await ChatWriter.SendTextAsync(_logic.GetRandomGreeting());
    }

    private async Task ShowSeaAsync()
    {
        string link = _logic.GetLink("sea");

        await ChatWriter.SendPhotoAsync(link);
    }

    private async Task ShowNorthAsync()
    {
        Northlands northlands = _logic.GetNorthLands(ServerDateTime.GetCurrent().Date);

        string text = $"""
            Расписание ивентов в СЗ:
            — войско богов: {northlands.Gods}
            — армия севера: {northlands.North}
            """;

        await ChatWriter.SendTextAsync(text);
    }

    private async Task ShowAgenda(DateOnly date)
    {
        StringBuilder builder = new();

        // Greeting
        builder
            .AppendLine(GetDaytimeGreeting(ServerDateTime.GetCurrent().Time!.Value));

        // Daily
        var daily = _logic.GetDaily(date);
        if (daily?.Definition != null)
        {
            builder
                .AppendFormat("Ежа: <b>{0}</b> — {1}.", daily.Quest, daily.Definition)
                .AppendLine();
        }

        // Northlands
        Northlands northlands = _logic.GetNorthLands(date);

        builder
            .AppendLine("Северные земли:")
            .AppendFormat("— войско богов: <b>{0}</b>.", northlands.Gods)
            .AppendLine()
            .AppendFormat("— армия севера: <b>{0}</b>.", northlands.North)
            .AppendLine();

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

        ICollection<DayEvent> dayEvents = _logic.GetDayEvents(date, MessageContext.ChatId);

        if (!dayEvents.Any())
        {
            builder.Append(" <b>отсутствуют</b>");
        }
        else
        {
            builder.Append(':');

            foreach (DayEvent dayEvent in dayEvents)
            {
                builder
                    .AppendLine()
                    .AppendFormat("— <b>{0}</b> — {1}", dayEvent.Time, dayEvent.Name);
            }
        }

        List<Promocode> expiringPromocodes = _logic.GetActivePromocodes()
            .Where(p => p.ValidTo?.Date == date)
            .ToList();

        if (expiringPromocodes.Any())
        {
            builder
                .AppendLine()
                .AppendLine()
                .Append("Последний день активации промокод")
                .Append(expiringPromocodes.Count > 1 ? "ов" : "а")
                .Append(' ')
                .AppendFormat("<code>{0}</code>", expiringPromocodes[0].Code);

            foreach (Promocode promocode in expiringPromocodes.Skip(1))
            {
                builder
                    .Append(", ")
                    .AppendFormat("<code>{0}</code>", promocode.Code);
            }

            builder.Append('.');
        }

        await ChatWriter.SendHtmlAsync(builder.ToString());
    }

    private static string GetDaytimeGreeting(TimeOnly time)
    {
        return time.Hour switch
        {
            < 5 => "Доброй ночи! =^.^=",
            < 12 => "Доброе утро! =^.^=",
            < 18 => "Добрый день! =^.^=",
            >= 18 => "Добрый вечер! =^.^="
        };
    }

    private async Task ShowEventsAsync()
    {
        ICollection<Event> events = _logic.GetActiveEvents();

        if (!events.Any())
        {
            await ChatWriter.SendTextAsync("На данный момент ивентов нет.");
            return;
        }

        DateOnly today = ServerDateTime.GetCurrent().Date;

        var builder = new StringBuilder()
            .AppendFormat("<b>Текущие ивенты</b> (на {0:yyyy-MM-dd}) <b>:</b>", today);

        foreach (var evt in events)
        {
            builder
                .AppendLine()
                .Append("— ")
                .Append(evt.Name);

            if (evt.ValidTo.HasValue)
            {
                builder.AppendFormat(" — <i>");

                if (evt.ValidTo.Value.Date == today)
                {
                    builder.Append("сегодня последний день");
                }
                else if (evt.ValidTo.Value.Date == today.AddDays(1))
                {
                    builder.Append("до завтра");
                }
                else
                {
                    builder.AppendFormat("до {0}", evt.ValidTo.Value);
                }

                builder.AppendFormat("</i>");
            }
        }

        await ChatWriter.SendHtmlAsync(builder.ToString());
    }

    private async Task ShowCalendarAsync()
    {
        string link = _logic.GetLink("daily");

        char prefix = link.Contains('?') ? '&' : '?';
        string url = $"{link}{prefix}month={DateTime.Now:yyMM}";

        await ChatWriter.SendPhotoAsync(url);
    }

    private async Task AddEvent(string eventName, ServerDateTime? starts, ServerDateTime? ends)
    {
        Event newEvent = new(eventName, starts, ends);
        _logic.AddEvent(newEvent);

        await ChatWriter.SendTextAsync($"Event added. Details: {(eventName, starts, ends)}");
    }

    private async Task AddPromocode(string code, ServerDateTime? validTo, string description)
    {
        Promocode newPromocode = new(code, description, null, validTo);
        _logic.AddPromocode(newPromocode);

        await ChatWriter.SendTextAsync($"Promocode added. Details: {(code, validTo, description)}");
    }

    private async Task Cleanup()
    {
        _logic.Cleanup();

        await ChatWriter.SendTextAsync("Cleanup done");
    }

    private async Task GlobalAnnounce(string message)
    {
        await MessageContext.Bot.Manager.Broadcast(chat => chat.SendTextAsync(message));
    }

    private async Task NoSoGlobalAnnounce(string message)
    {
        await ChatWriter.SendTextAsync("This will be announced:" + Environment.NewLine + Environment.NewLine + message);
    }

    private async Task SetDaily(int month, string dailies)
    {
        if (month is < 1 or > 12)
        {
            await ChatWriter.SendTextAsync($"Wrong month number. Expected: 1 to 12, actual: {month}.");
            return;
        }

        int daysExpected = DateTime.DaysInMonth(DateTime.Now.Year, month);
        if (daysExpected != dailies.Length)
        {
            await ChatWriter.SendTextAsync($"Wrong month length. Expected: {daysExpected}, actual: {dailies.Length}.");
            return;
        }

        _logic.SetDaily(month, dailies);

        await ChatWriter.SendTextAsync($"Dailies for month {month} set");
    }

    private Task LogMessage(string message)
    {
        _logger.LogInformation(message);
        return Task.CompletedTask;
    }

    //[BotCommand("/murka")]
    //[BotCommand("мурка", BotCommandOptions.CaseInsensitive)]
    //public async Task StartDialogAsync()
    //{
    //    IReplyMarkup replyMarkup = new InlineKeyboardMarkup(new[]
    //    {
    //        new InlineKeyboardButton("ежа") { CallbackData = "ежа" },
    //        new InlineKeyboardButton("север") { CallbackData = "север" }
    //    });

    //    await ((TelegramChatWriter)ChatWriter).SendTextAsync("Чем помочь?", replyMarkup);
    //}

    protected override async Task OnExceptionAsync(Exception ex)
    {
        _logger.LogError(EventIDs.MessageHandleException, ex, "Error occurred in chat {ChatId}", MessageContext.ChatId);
        await base.OnExceptionAsync(ex);
    }

    private static readonly string[] _greetingsToReply = new[]
    {
        "привет",
        "доброе утро",
        "добрый день",
        "добрый вечер",
        "здравствуйте"
    };
}