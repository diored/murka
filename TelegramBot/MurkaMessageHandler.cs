using System.Text;
using System.Text.RegularExpressions;

using DioRed.Murka.Core;
using DioRed.Murka.Core.Entities;
using DioRed.TelegramBot;

using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace DioRed.Murka.TelegramBot;

public class MurkaMessageHandler : MessageHandler
{
    public MurkaMessageHandler(MessageContext messageContext)
        : base(messageContext)
    {
        Chat = (MurkaChatClient)messageContext.Chat;
    }

    public MurkaChatClient Chat { get; }

    [BotCommand("^(/daily|ежа)$")]
    public async Task ShowDaily()
    {
        string? today = Chat.DataSource.GetDaily(ServerTime.GetCurrent())?.Definition;
        string? tomorrow = Chat.DataSource.GetDaily(ServerTime.GetCurrent().AddDays(1))?.Definition;

        if (today == null)
        {
            await SendMessage("Пока хз, что по еже.");
        }
        else if (tomorrow == null)
        {
            await SendMessage($"Сегодня ежа: {today}.");
        }
        else
        {
            await SendMessage($"Сегодня ежа: {today}, а завтра: {tomorrow}.");
        }
    }

    [BotCommand("^(/promo|промокоды)$")]
    public async Task ShowPromocodes()
    {
        Promocode[] activePromocodes = Chat.DataSource.GetActivePromocodes(ServerTime.GetCurrent());

        if (activePromocodes.Any())
        {
            StringBuilder builder = new();
            builder.Append("<b>Активные промокоды:</b>");
            foreach (Promocode promocode in activePromocodes)
            {
                builder
                    .AppendLine()
                    .AppendFormat("<code>{0}</code> (до {1}) — {2}", promocode.Code, promocode.ValidTo, promocode.Content);
            }

            await SendMessage(builder.ToString(), formatted: true);
        }
        else
        {
            await SendMessage("Активных промокодов нет");
        }
    }

    public async Task GreetAdmin()
    {
        //if (_context.ChatType == ChatType.Private)
        await SendMessage("Hi admin =^.^=");
    }

    [BotCommand("привет|доброе утро|добрый день|добрый вечер", RegexOptions.IgnoreCase)]
    public async Task SayMurr()
    {
        if (!(ServerTime.GetCurrent() - ((MurkaChatClient)MessageContext.Chat).LatestGreeting < TimeSpan.FromMinutes(1)))
        {
            ((MurkaChatClient)MessageContext.Chat).LatestGreeting = ServerTime.GetCurrent();
            await SendMessage(Chat.DataSource.GetRandomGreeting());
        }
    }

    [BotCommand("^(/sea|море)$")]
    public async Task ShowSea()
    {
        await SendPhoto("https://operator.cdn.gmru.net/ms/05141cf319c4d5788eb1470cebd9a28c.jpg");
    }

    [BotCommand("^(/north|север)$")]
    public async Task ShowNorth()
    {
        await SendMessage($"Расписание ивентов в СЗ:\n— войско богов: {Chat.DataSource.GetNorth(ServerTime.GetCurrent(), NorthArmy.Gods)}\n— армия севера: {Chat.DataSource.GetNorth(ServerTime.GetCurrent(), NorthArmy.North)}");
    }

    [BotCommand("^(/agenda|сводка)$")]
    public async Task ShowAgenda()
    {
        await ShowAgendaFor(ServerTime.GetCurrent());
    }

    [BotCommand("^(/tomorrow|завтра)$")]
    public async Task ShowAgendaTomorrow()
    {
        await ShowAgendaFor(ServerTime.GetCurrent().AddDays(1));
    }

    [BotCommand("^(/events|ивенты)$")]
    public async Task ShowEvents()
    {
        var builder = new StringBuilder()
            .AppendFormat("<b>Текущие ивенты</b> (на {0:yyyy-MM-dd}) <b>:</b>", ServerTime.GetCurrent());

        foreach (var evt in Chat.DataSource.GetActiveEvents(ServerTime.GetCurrent()))
        {
            builder
                .AppendLine()
                .AppendFormat("{0} — <i>до {1}</i>", evt.Name, evt.Ends.ToString("yyyy-MM-dd HH:mm"));
        }

        await SendMessage(builder.ToString(), formatted: true);
    }

    [BotCommand("^(/murka|мурка)$")]
    public async Task StartDialog()
    {
        IReplyMarkup replyMarkup = new InlineKeyboardMarkup(new[]
        {
                new InlineKeyboardButton("ежа") { CallbackData = "ежа" },
                new InlineKeyboardButton("север") { CallbackData = "север" }
            });

        await MessageContext.Bot.SendTextMessageAsync(MessageContext.Chat.Chat.Id, "Чем помочь?", replyMarkup: replyMarkup);
    }

    private async Task ShowAgendaFor(DateTime dateTime)
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

        var daily = Chat.DataSource.GetDaily(dateTime)?.Definition;
        if (daily != null)
        {
            builder
                .AppendFormat("Ежа: <b>{0}</b>.", daily)
                .AppendLine();
        }

        builder
            .AppendLine("Северные земли:")
            .AppendFormat("— войско богов: <b>{0}</b>.", Chat.DataSource.GetNorth(dateTime, NorthArmy.Gods))
            .AppendLine()
            .AppendFormat("— армия севера: <b>{0}</b>.", Chat.DataSource.GetNorth(dateTime, NorthArmy.North))
            .AppendLine()
            .AppendFormat("Ивенты {0}:", dowG);

        var dayEvents = Chat.DataSource.GetDayEvents(dateTime).ToList();

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

        await SendMessage(message, formatted: true);
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
