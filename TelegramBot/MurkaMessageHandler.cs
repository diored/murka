using System.Text;

using DioRed.Murka.Core.Entities;
using DioRed.Murkal.ForecastBuilder;
using DioRed.Vermilion;
using DioRed.Vermilion.Attributes;

using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace DioRed.Murka.TelegramBot;

public partial class MurkaMessageHandler : MessageHandler
{
    private TimeSpan GreetingInterval { get; } = TimeSpan.FromMinutes(40);

    public MurkaMessageHandler(MessageContext messageContext)
        : base(messageContext)
    {
        MurkaChat = (MurkaChatClient)messageContext.ChatClient;

        ChatWriter.OnException += ChatWriter_OnException;
    }

    public MurkaChatClient MurkaChat { get; }

    [BotCommand("/daily")]
    [BotCommand("ежа", BotCommandOptions.CaseInsensitive)]
    public async Task ShowDailyTextAsync()
    {
        await ShowDailyInternalAsync(5);
    }

    [BotCommand("/daily")]
    [BotCommand("ежа", BotCommandOptions.CaseInsensitive)]
    public async Task ShowDailyAsync(string days)
    {
        if (days is null || !int.TryParse(days, out int nDays))
        {
            nDays = 5;
        }

        await ShowDailyInternalAsync(nDays);
    }

    private async Task ShowDailyInternalAsync(int days)
    {
        ServerDateTime serverTime = ServerDateTime.GetCurrent();

        ScheduleItem[] schedule = Enumerable.Range(0, days)
            .Select(i =>
            {
                DateOnly date = serverTime.Date.AddDays(i);
                return new ScheduleItem(date, MurkaChat.Logic.GetDaily(date).Id);
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

    [BotCommand("/promo")]
    [BotCommand("промокоды", BotCommandOptions.CaseInsensitive)]
    public async Task ShowPromocodesAsync()
    {
        ICollection<Promocode> activePromocodes = MurkaChat.Logic.GetActivePromocodes();

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

    [BotCommand("/addDayEvent", BotCommandOptions.CaseInsensitive)]
    public async void AddDayEventHelp()
    {
        if (MessageContext.Role == UserRole.AnyUser)
        {
            await ChatWriter.SendTextAsync("Добавлять ивенты может только администратор чата");
            return;
        }

        string html = @"Синтаксис:
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
Удаление ранее добавленных событий будет реализовано в одной из последующих версий.";
        await ChatWriter.SendHtmlAsync(html);
    }

    [BotCommand(UserRole.ChatAdmin, @"/addDayEvent", BotCommandOptions.CaseInsensitive)]
    public void AddDayEvent(string name, string time, string repeat)
    {
        AddDayEventInternal(name, repeat, time, MessageContext.ChatClient.Chat.ToChatInfo().ChatId);
    }

    [BotCommand("привет|доброе утро|добрый день|добрый вечер", BotCommandOptions.Regex | BotCommandOptions.CaseInsensitive)]
    public async Task SayMurrAsync()
    {
        if (!(DateTime.UtcNow - MurkaChat.LatestGreeting < GreetingInterval))
        {
            MurkaChat.LatestGreeting = DateTime.UtcNow;
            await ChatWriter.SendTextAsync(MurkaChat.Logic.GetRandomGreeting());
        }
    }

    [BotCommand("/sea")]
    [BotCommand("море", BotCommandOptions.CaseInsensitive)]
    public async Task ShowSeaAsync()
    {
        string link = MurkaChat.Logic.GetLink("sea");

        await ChatWriter.SendPhotoAsync(link);
    }

    [BotCommand("/north")]
    [BotCommand("север", BotCommandOptions.CaseInsensitive)]
    public async Task ShowNorthAsync()
    {
        Northlands northlands = MurkaChat.Logic.GetNorthLands(ServerDateTime.GetCurrent().Date);

        await ChatWriter.SendTextAsync($"Расписание ивентов в СЗ:\n— войско богов: {northlands.Gods}\n— армия севера: {northlands.North}");
    }

    [BotCommand("/agenda")]
    [BotCommand("сводка", BotCommandOptions.CaseInsensitive)]
    public async Task ShowAgendaAsync()
    {
        await ShowAgendaInternalAsync(ServerDateTime.GetCurrent().Date);
    }

    [BotCommand("/tomorrow")]
    [BotCommand("завтра", BotCommandOptions.CaseInsensitive)]
    public async Task ShowAgendaTomorrowAsync()
    {
        await ShowAgendaInternalAsync(ServerDateTime.GetCurrent().Date.AddDays(1));
    }

    [BotCommand("/events")]
    [BotCommand("ивенты", BotCommandOptions.CaseInsensitive)]
    public async Task ShowEventsAsync()
    {
        ICollection<Event> events = MurkaChat.Logic.GetActiveEvents();

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

    [BotCommand("/calendar")]
    [BotCommand("календарь", BotCommandOptions.CaseInsensitive)]
    public async Task ShowCalendarAsync()
    {
        string link = MurkaChat.Logic.GetLink("daily");

        await ChatWriter.SendPhotoAsync(link);
    }

    [BotCommand("/murka")]
    [BotCommand("мурка", BotCommandOptions.CaseInsensitive)]
    public async Task StartDialogAsync()
    {
        IReplyMarkup replyMarkup = new InlineKeyboardMarkup(new[]
        {
            new InlineKeyboardButton("ежа") { CallbackData = "ежа" },
            new InlineKeyboardButton("север") { CallbackData = "север" }
        });

        await MessageContext.BotClient.SendTextMessageAsync(MurkaChat.Chat.Id, "Чем помочь?", replyMarkup: replyMarkup);
    }

    private void AddDayEventInternal(string name, string occurrence, string time, string chatId)
    {
        name = name.Trim();
        occurrence = occurrence.Trim().ToLowerInvariant() switch
        {
            "daily" => "daily",
            "mo" or "monday" or "1" => "weekly:1",
            "tu" or "tuesday" or "2" => "weekly:2",
            "we" or "wednesday" or "3" => "weekly:3",
            "th" or "thursday" or "4" => "weekly:4",
            "fr" or "friday" or "5" => "weekly:5",
            "sa" or "saturday" or "6" => "weekly:6",
            "su" or "sunday" or "7" or "0" => "weekly:0",
            _ => throw new ArgumentOutOfRangeException(nameof(occurrence), $"Unrecognized repeat qualifier: {occurrence}. See /addDayEvent for possible qualifiers list.")
        };

        TimeOnly timeOnly = TimeOnly.ParseExact(time, CommonValues.TimeFormat);

        MurkaChat.Logic.AddDayEvent(name, occurrence, timeOnly, chatId);
    }

    private async Task ShowAgendaInternalAsync(DateOnly date)
    {
        StringBuilder builder = new();

        Greeting();
        Daily();
        Northlands();
        DayEvents();

        await ChatWriter.SendHtmlAsync(builder.ToString());


        void Greeting()
        {
            builder.AppendLine(GetDaytimeGreeting(ServerDateTime.GetCurrent().Time!.Value));
        }

        void Daily()
        {
            var daily = MurkaChat.Logic.GetDaily(date);
            if (daily?.Definition != null)
            {
                builder
                    .AppendFormat("Ежа: <b>{0}</b> — {1}.", daily.Quest, daily.Definition)
                    .AppendLine();
            }
        }

        void Northlands()
        {
            Northlands northlands = MurkaChat.Logic.GetNorthLands(date);

            builder
                .AppendLine("Северные земли:")
                .AppendFormat("— войско богов: <b>{0}</b>.", northlands.Gods)
                .AppendLine()
                .AppendFormat("— армия севера: <b>{0}</b>.", northlands.North)
                .AppendLine();
        }

        void DayEvents()
        {
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

            string chatId = MessageContext.ChatClient.Chat.ToChatInfo().ChatId;
            var dayEvents = MurkaChat.Logic.GetDayEvents(date, chatId);

            if (!dayEvents.Any())
            {
                builder.Append(" <b>отсутствуют</b>");
                return;
            }

            builder.Append(':');

            foreach (DayEvent dayEvent in dayEvents)
            {
                builder
                    .AppendLine()
                    .AppendFormat("— <b>{0}</b> — {1}", dayEvent.Time, dayEvent.Name);
            }
        }
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

    private void ChatWriter_OnException(Exception ex)
    {
        ChatInfo chat = MurkaChat.Chat.ToChatInfo();

        MurkaChat.Logic.Log("error", "Error occurred in chat", chat, ex);

        if (ex.Message.Contains("kicked") || ex.Message.Contains("blocked"))
        {
            MurkaChat.Logic.RemoveChat(chat);
        }
    }

    public override async Task OnExceptionAsync(Exception ex)
    {
        string message = "Error occurred"
#if DEBUG
            + ": " + (ex.InnerException?.Message ?? ex.Message)
#endif
        ;

        await ChatWriter.SendTextAsync(message);
    }
}
