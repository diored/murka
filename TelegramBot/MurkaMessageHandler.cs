using System.Text;
using System.Text.RegularExpressions;

using DioRed.Murka.Core;
using DioRed.Murka.Core.Entities;
using DioRed.Vermilion;
using DioRed.Vermilion.Attributes;

using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace DioRed.Murka.TelegramBot;

public class MurkaMessageHandler : MessageHandler
{
    public MurkaMessageHandler(MessageContext messageContext)
        : base(messageContext)
    {
        MurkaChat = (MurkaChatClient)messageContext.ChatClient;
    }

    public MurkaChatClient MurkaChat { get; }

    [BotCommand("^(/daily|ежа)$")]
    public async Task ShowDailyAsync()
    {
        DateTime serverTime = ServerTime.GetCurrent();
        Daily today = MurkaChat.DataSource.GetDaily(serverTime);
        Daily tomorrow = MurkaChat.DataSource.GetDaily(serverTime.AddDays(1));

        if (string.IsNullOrEmpty(today.Code))
        {
            await SendTextAsync("Пока хз, что по еже.");
        }
        else if (string.IsNullOrEmpty(tomorrow.Code))
        {
            await SendHtmlAsync($"Сегодня ежа в {today.Definition} (<b>{today.Code}</b>).");
        }
        else
        {
            await SendHtmlAsync($"Сегодня ежа в {today.Definition} (<b>{today.Code}</b>), а завтра в {tomorrow.Definition} (<b>{tomorrow.Code}</b>).");
        }
    }

    [BotCommand("^(/promo|промокоды)$")]
    public async Task ShowPromocodesAsync()
    {
        Promocode[] activePromocodes = MurkaChat.DataSource.GetActivePromocodes(ServerTime.GetCurrent());

        if (activePromocodes.Any())
        {
            StringBuilder builder = new();
            builder.Append("<b>Активные промокоды:</b>");
            foreach (Promocode promocode in activePromocodes)
            {
                builder
                    .AppendLine()
                    .AppendFormat("<code>{0}</code> (до {1}) — {2}", promocode.Code, ServerTime.GetServerTime(promocode.ValidTo), promocode.Content);
            }

            await SendHtmlAsync(builder.ToString());
        }
        else
        {
            await SendTextAsync("Активных промокодов нет");
        }
    }

    [BotCommand("^(/admin|админ)$")]
    [AdminOnly]
    public async Task GreetAdminAsync()
    {
        //if (_context.ChatType == ChatType.Private)
        await SendTextAsync("Hi admin =^.^=");
    }

    [BotCommand("привет|доброе утро|добрый день|добрый вечер", RegexOptions.IgnoreCase)]
    public async Task SayMurrAsync()
    {
        if (!(ServerTime.GetCurrent() - MurkaChat.LatestGreeting < TimeSpan.FromMinutes(1)))
        {
            MurkaChat.LatestGreeting = ServerTime.GetCurrent();
            await SendTextAsync(MurkaChat.DataSource.GetRandomGreeting());
        }
    }

    [BotCommand("^(/sea|море)$")]
    public async Task ShowSeaAsync()
    {
        await SendPhotoAsync("https://operator.cdn.gmru.net/ms/05141cf319c4d5788eb1470cebd9a28c.jpg");
    }

    [BotCommand("^(/north|север)$")]
    public async Task ShowNorthAsync()
    {
        await SendTextAsync($"Расписание ивентов в СЗ:\n— войско богов: {MurkaChat.DataSource.GetNorth(ServerTime.GetCurrent(), NorthArmy.Gods)}\n— армия севера: {MurkaChat.DataSource.GetNorth(ServerTime.GetCurrent(), NorthArmy.North)}");
    }

    [BotCommand("^(/agenda|сводка)$")]
    public async Task ShowAgendaAsync()
    {
        await ShowAgendaForAsync(ServerTime.GetCurrent());
    }

    [BotCommand("^(/tomorrow|завтра)$")]
    public async Task ShowAgendaTomorrowAsync()
    {
        await ShowAgendaForAsync(ServerTime.GetCurrent().AddDays(1));
    }

    [BotCommand("^(/events|ивенты)$")]
    public async Task ShowEventsAsync()
    {
        var builder = new StringBuilder()
            .AppendFormat("<b>Текущие ивенты</b> (на {0:yyyy-MM-dd}) <b>:</b>", ServerTime.GetCurrent());

        foreach (var evt in MurkaChat.DataSource.GetActiveEvents(ServerTime.GetCurrent()))
        {
            builder
                .AppendLine()
                .AppendFormat("{0} — <i>до {1}</i>", evt.Name, evt.Ends.ToString("yyyy-MM-dd HH:mm"));
        }

        await SendHtmlAsync(builder.ToString());
    }

    [BotCommand("^(/murka|мурка)$")]
    public async Task StartDialogAsync()
    {
        IReplyMarkup replyMarkup = new InlineKeyboardMarkup(new[]
        {
                new InlineKeyboardButton("ежа") { CallbackData = "ежа" },
                new InlineKeyboardButton("север") { CallbackData = "север" }
            });

        await MessageContext.BotClient.SendTextMessageAsync(MurkaChat.Chat.Id, "Чем помочь?", replyMarkup: replyMarkup);
    }

    private async Task ShowAgendaForAsync(DateTime dateTime)
    {
        string dowG = dateTime.DayOfWeek switch
        {
            DayOfWeek.Monday => "в <i>понедельник</i>",
            DayOfWeek.Tuesday => "во <i>вторник</i>",
            DayOfWeek.Wednesday => "в <i>среду</i>",
            DayOfWeek.Thursday => "в <i>четверг</i>",
            DayOfWeek.Friday => "в <i>пятницу</i>",
            DayOfWeek.Saturday => "в <i>субботу</i>",
            _ => "в <i>воскресенье</i>"
        };

        var builder = new StringBuilder()
            .AppendLine(GetDaytimeGreeting(ServerTime.GetCurrent()));

        var daily = MurkaChat.DataSource.GetDaily(dateTime)?.Definition;
        if (daily != null)
        {
            builder
                .AppendFormat("Ежа: <b>{0}</b>.", daily)
                .AppendLine();
        }

        builder
            .AppendLine("Северные земли:")
            .AppendFormat("— войско богов: <b>{0}</b>.", MurkaChat.DataSource.GetNorth(dateTime, NorthArmy.Gods))
            .AppendLine()
            .AppendFormat("— армия севера: <b>{0}</b>.", MurkaChat.DataSource.GetNorth(dateTime, NorthArmy.North))
            .AppendLine()
            .AppendFormat("Ивенты {0}:", dowG);

        var dayEvents = MurkaChat.DataSource.GetDayEvents(dateTime).ToList();

        if (dayEvents.Any())
        {
            foreach (string dayEvent in dayEvents)
            {
                builder
                    .AppendLine()
                    .AppendFormat("— <b>{0}</b>", dayEvent);
            }
        }
        else
        {
            builder.Append(" <b>отсутствуют</b>");
        }

        var message = builder.ToString();

        await SendHtmlAsync(message);
    }

    private static string GetDaytimeGreeting(DateTime dateTime)
    {
        return dateTime.Hour switch
        {
            < 5 => "Доброй ночи! =^.^=",
            < 12 => "Доброе утро! =^.^=",
            < 18 => "Добрый день! =^.^=",
            >= 18 => "Добрый вечер! =^.^="
        };
    }
}
