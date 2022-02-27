using System.Text;

using DioRed.Murka.Common;
using DioRed.Murka.Core.Entities;
using DioRed.Vermilion;
using DioRed.Vermilion.Attributes;

using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace DioRed.Murka.TelegramBot;

public partial class MurkaMessageHandler : MessageHandler
{
    public MurkaMessageHandler(MessageContext messageContext)
        : base(messageContext)
    {
        MurkaChat = (MurkaChatClient)messageContext.ChatClient;

        ChatWriter.OnException += ChatWriter_OnException;
    }

    public MurkaChatClient MurkaChat { get; }

    [BotCommand("/daily")]
    [BotCommand("ежа", BotCommandOptions.CaseInsensitive)]
    public async Task ShowDailyAsync()
    {
        ServerTime serverTime = ServerTime.GetCurrent();
        Daily today = MurkaChat.Logic.GetDaily(serverTime.Date);
        Daily tomorrow = MurkaChat.Logic.GetDaily(serverTime.Date.AddDays(1));

        if (string.IsNullOrEmpty(today.Code))
        {
            await ChatWriter.SendTextAsync("Пока хз, что по еже.");
        }
        else if (string.IsNullOrEmpty(tomorrow.Code))
        {
            await ChatWriter.SendHtmlAsync($"Сегодня ежа в {today.Definition} (<b>{today.Code}</b>).");
        }
        else
        {
            await ChatWriter.SendHtmlAsync($"Сегодня ежа в {today.Definition} (<b>{today.Code}</b>), а завтра в {tomorrow.Definition} (<b>{tomorrow.Code}</b>).");
        }
    }

    [BotCommand("/promo")]
    [BotCommand("промокоды", BotCommandOptions.CaseInsensitive)]
    public async Task ShowPromocodesAsync()
    {
        ServerTime serverTime = ServerTime.GetCurrent();

        ICollection<Promocode> activePromocodes = MurkaChat.Logic.GetActivePromocodes(serverTime);

        if (!activePromocodes.Any())
        {
            await ChatWriter.SendTextAsync("Активных промокодов нет");
            return;
        }

        StringBuilder builder = new();
        builder.Append("<b>Активные промокоды:</b>");
        foreach (Promocode promocode in activePromocodes)
        {
            builder
                .AppendLine()
                .AppendFormat("<code>{0}</code>", promocode.Code);

            if (promocode.Valid.To.HasValue)
            {
                builder.AppendFormat(" (до {0})", promocode.Valid.To.ToString());
            }

            builder.AppendFormat(" — {0}", promocode.Content);
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
        AddDayEventInternal(MessageContext.ChatClient.Chat.ToChatInfo().ToString(), name, time, repeat);
    }

    [BotCommand("привет|доброе утро|добрый день|добрый вечер", BotCommandOptions.Regex | BotCommandOptions.CaseInsensitive)]
    public async Task SayMurrAsync()
    {
        if (!(DateTime.UtcNow - MurkaChat.LatestGreeting < TimeSpan.FromMinutes(5)))
        {
            MurkaChat.LatestGreeting = DateTime.UtcNow;
            await ChatWriter.SendTextAsync(MurkaChat.Logic.GetRandomGreeting());
        }
    }

    [BotCommand("/sea")]
    [BotCommand("море", BotCommandOptions.CaseInsensitive)]
    public async Task ShowSeaAsync()
    {
        await ChatWriter.SendPhotoAsync("https://operator.cdn.gmru.net/ms/05141cf319c4d5788eb1470cebd9a28c.jpg");
    }

    [BotCommand("/north")]
    [BotCommand("север", BotCommandOptions.CaseInsensitive)]
    public async Task ShowNorthAsync()
    {
        Northlands northlands = MurkaChat.Logic.GetNorthLands(ServerTime.GetCurrent().Date);

        await ChatWriter.SendTextAsync($"Расписание ивентов в СЗ:\n— войско богов: {northlands.Gods}\n— армия севера: {northlands.North}");
    }

    [BotCommand("/agenda")]
    [BotCommand("сводка", BotCommandOptions.CaseInsensitive)]
    public async Task ShowAgendaAsync()
    {
        await ShowAgendaInternalAsync(ServerTime.GetCurrent().Date);
    }

    [BotCommand("/tomorrow")]
    [BotCommand("завтра", BotCommandOptions.CaseInsensitive)]
    public async Task ShowAgendaTomorrowAsync()
    {
        await ShowAgendaInternalAsync(ServerTime.GetCurrent().Date.AddDays(1));
    }

    [BotCommand("/events")]
    [BotCommand("ивенты", BotCommandOptions.CaseInsensitive)]
    public async Task ShowEventsAsync()
    {
        ServerTime serverTime = ServerTime.GetCurrent();

        ICollection<Event> events = MurkaChat.Logic.GetActiveEvents(serverTime);

        if (!events.Any())
        {
            await ChatWriter.SendTextAsync("На данный момент ивентов нет.");
            return;
        }

        var builder = new StringBuilder()
            .AppendFormat("<b>Текущие ивенты</b> (на {0:yyyy-MM-dd}) <b>:</b>", serverTime.Date);

        foreach (var evt in events)
        {
            builder
                .AppendLine()
                .Append("— ")
                .Append(evt.Name);

            if (evt.Valid.To.HasValue)
            {
                builder.AppendFormat(" — <i>");

                if (evt.Valid.To.Value.Date == serverTime.Date)
                {
                    builder.Append("сегодня последний день");
                }
                else if (evt.Valid.To.Value.Date == serverTime.Date.AddDays(1))
                {
                    builder.Append("до завтра");
                }
                else
                {
                    builder.AppendFormat("до {0}", evt.Valid.To.Value);
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
        BinaryData photo = MurkaChat.Logic.GetCalendar();
        await ChatWriter.SendPhotoAsync(photo.ToStream());
    }

    //[BotCommand("/remind (.+)|(.+)", BotCommandOptions.Regex)]
    //public async Task AddReminder(string time, string message)
    //{
    //    TimeSpan timeSpan = ServerTime.Parse(time.Trim()).ToUtc().TimeOfDay;
    //    message = message.Trim();

    //    Job.SetupOneTime(MessageContext.Role)
    //}

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

    private void AddDayEventInternal(string chatId, string name, string time, string repeat)
    {
        name = name.Trim();
        time = time.Trim();
        repeat = repeat.Trim();

        TimeOnly timeOnly = TimeOnly.ParseExact(time, ServerTime.TimeFormat);
        Occurrence occurrence = repeat.ToLowerInvariant() switch
        {
            "daily" => Occurrence.Daily(timeOnly),
            "mo" or "monday" or "1" => Occurrence.Weekly(DayOfWeek.Monday, timeOnly),
            "tu" or "tuesday" or "2" => Occurrence.Weekly(DayOfWeek.Tuesday, timeOnly),
            "we" or "wednesday" or "3" => Occurrence.Weekly(DayOfWeek.Wednesday, timeOnly),
            "th" or "thursday" or "4" => Occurrence.Weekly(DayOfWeek.Thursday, timeOnly),
            "fr" or "friday" or "5" => Occurrence.Weekly(DayOfWeek.Friday, timeOnly),
            "sa" or "saturday" or "6" => Occurrence.Weekly(DayOfWeek.Saturday, timeOnly),
            "su" or "sunday" or "7" or "0" => Occurrence.Weekly(DayOfWeek.Sunday, timeOnly),
            _ => throw new ArgumentOutOfRangeException(nameof(repeat), $"Unrecognized repeat qualifier: {repeat}. See /addDayEvent for possible qualifiers list.")
        };

        var dayEvent = new DayEvent(name, occurrence, chatId);
        MurkaChat.Logic.AddDayEvent(dayEvent);
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
            builder.AppendLine(GetDaytimeGreeting(ServerTime.GetCurrent().Time!.Value));
        }

        void Daily()
        {
            var daily = MurkaChat.Logic.GetDaily(date);
            if (daily?.Definition != null)
            {
                builder
                    .AppendFormat("Ежа: <b>{0}</b> (<b>{1}</b>).", daily.Definition, daily.Code)
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

            string chatId = MessageContext.ChatClient.Chat.ToChatInfo().ToString();
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
                    .AppendFormat("— <b>{0}</b> — {1}", dayEvent.Occurrence.Time, dayEvent.Name);
            }
        }
    }

    private static ServerTimeRange ParseServerTimeRange(string? from, string? to)
    {
        return new ServerTimeRange(ServerTime.SafeParse(from), ServerTime.SafeParse(to));
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
        await ChatWriter.SendTextAsync("Error occured: " + ex.InnerException?.Message ?? ex.Message);
    }
}
