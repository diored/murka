using System.Text;
using System.Text.RegularExpressions;

using DioRed.Murka.Common;
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

        ChatWriter.OnException += ChatWriter_OnException;
    }

    public MurkaChatClient MurkaChat { get; }

    [BotCommand("^(/daily|ежа)$")]
    public async Task ShowDailyAsync()
    {
        DateTime serverTime = ServerTime.GetCurrent();
        Daily today = MurkaChat.Logic.GetDaily(serverTime);
        Daily tomorrow = MurkaChat.Logic.GetDaily(serverTime.AddDays(1));

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

    [BotCommand("^(/promo|промокоды)$")]
    public async Task ShowPromocodesAsync()
    {
        DateTime serverTime = ServerTime.GetCurrent();

        ICollection<Promocode> activePromocodes = MurkaChat.Logic.GetActivePromocodes(serverTime);

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

            await ChatWriter.SendHtmlAsync(builder.ToString());
        }
        else
        {
            await ChatWriter.SendTextAsync("Активных промокодов нет");
        }
    }

    [BotCommand("^(/admin|админ)$")]
    [AdminOnly]
    public async Task GreetAdminAsync()
    {
        //if (_context.ChatType == ChatType.Private)
        await ChatWriter.SendTextAsync("Hi admin =^.^=");
    }

    [BotCommand("привет|доброе утро|добрый день|добрый вечер", RegexOptions.IgnoreCase)]
    public async Task SayMurrAsync()
    {
        if (!(ServerTime.GetCurrent() - MurkaChat.LatestGreeting < TimeSpan.FromMinutes(1)))
        {
            MurkaChat.LatestGreeting = ServerTime.GetCurrent();
            await ChatWriter.SendTextAsync(MurkaChat.Logic.GetRandomGreeting());
        }
    }

    [BotCommand("^(/sea|море)$")]
    public async Task ShowSeaAsync()
    {
        await ChatWriter.SendPhotoAsync("https://operator.cdn.gmru.net/ms/05141cf319c4d5788eb1470cebd9a28c.jpg");
    }

    [BotCommand("^(/north|север)$")]
    public async Task ShowNorthAsync()
    {
        Northlands northlands = MurkaChat.Logic.GetNorthLands(ServerTime.GetCurrent());

        await ChatWriter.SendTextAsync($"Расписание ивентов в СЗ:\n— войско богов: {northlands.Gods}\n— армия севера: {northlands.North}");
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
        DateTime serverTime = ServerTime.GetCurrent();

        var builder = new StringBuilder()
            .AppendFormat("<b>Текущие ивенты</b> (на {0:yyyy-MM-dd}) <b>:</b>", serverTime);

        foreach (var evt in MurkaChat.Logic.GetActiveEvents(serverTime))
        {
            builder
                .AppendLine()
                .AppendFormat("{0} — <i>до {1}</i>", evt.Name, evt.Ends.ToString("yyyy-MM-dd HH:mm"));
        }

        await ChatWriter.SendHtmlAsync(builder.ToString());
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

        var daily = MurkaChat.Logic.GetDaily(dateTime)?.Definition;
        if (daily != null)
        {
            builder
                .AppendFormat("Ежа: <b>{0}</b>.", daily)
                .AppendLine();
        }

        Northlands northlands = MurkaChat.Logic.GetNorthLands(ServerTime.GetCurrent());

        builder
            .AppendLine("Северные земли:")
            .AppendFormat("— войско богов: <b>{0}</b>.", northlands.Gods)
            .AppendLine()
            .AppendFormat("— армия севера: <b>{0}</b>.", northlands.North)
            .AppendLine()
            .AppendFormat("Ивенты {0}:", dowG);

        var dayEvents = MurkaChat.Logic.GetDayEvents(dateTime).ToList();

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

        await ChatWriter.SendHtmlAsync(message);
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

    private void ChatWriter_OnException(Exception ex)
    {
        MurkaChat.Logic.Log("error", "Error occurred in chat", exception: ex);

        if (ex.Message.Contains("kicked"))
        {
            MurkaChat.Logic.RemoveChat(MurkaChat.Chat.ToChatInfo());
        }
    }
}
