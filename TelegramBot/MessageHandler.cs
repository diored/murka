using System.Text;
using System.Text.RegularExpressions;

using DioRed.Murka.Core;
using DioRed.Murka.Core.Entities;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace DioRed.Murka.TelegramBot;

public class MessageHandler
{
    private readonly HandleContext _context;

    public MessageHandler(HandleContext context)
    {
        _context = context;
    }

    private IDataSource Data => _context.DataSource;

    public async Task Handle(string message, ClientData clientData)
    {
        Task? task = message switch
        {
            "/daily" or "ежа" => ShowDaily(),
            "/promo" or "промокоды" => ShowPromocodes(),
            "/sea" or "море" => ShowSea(),
            "/north" or "север" => ShowNorth(),
            "/agenda" or "сводка" => ShowAgenda(ServerTime.GetCurrent()),
            "/tomorrow" or "завтра" => ShowAgenda(ServerTime.GetCurrent().AddDays(1)),
            "/events" or "ивенты" => ShowEvents(),
            "/murka" or "мурка" => StartDialog(),
            _ => null
        };

        if (task != null)
        {
            await task;
            return;
        }

        if (_murrTriggers.Any(mt => mt.IsMatch(message)) &&
            !(ServerTime.GetCurrent() - clientData.LatestGreeting < TimeSpan.FromMinutes(1)))
        {
            clientData.LatestGreeting = ServerTime.GetCurrent();
            await SayMurr();
        }
    }

    private async Task ShowDaily()
    {
        string? today = Data.GetDaily(ServerTime.GetCurrent())?.Definition;
        string? tomorrow = Data.GetDaily(ServerTime.GetCurrent().AddDays(1))?.Definition;

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

    async Task SayMurr()
    {
        await SendMessage(Data.GetRandomGreeting());
    }

    async Task ShowPromocodes()
    {
        Promocode[] activePromocodes = Data.GetActivePromocodes(ServerTime.GetCurrent());

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

    private async Task ShowSea()
    {
        await SendPhoto("https://operator.cdn.gmru.net/ms/05141cf319c4d5788eb1470cebd9a28c.jpg");
    }

    private async Task ShowNorth()
    {
        await SendMessage($"Расписание ивентов в СЗ:\n— войско богов: {Data.GetNorth(ServerTime.GetCurrent(), NorthArmy.Gods)}\n— армия севера: {Data.GetNorth(ServerTime.GetCurrent(), NorthArmy.North)}");
    }

    private async Task ShowAgenda(DateTime dateTime)
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

        var daily = Data.GetDaily(dateTime)?.Definition;
        if (daily != null)
        {
            builder
                .AppendFormat("Ежа: <b>{0}</b>.", daily)
                .AppendLine();
        }

        builder
            .AppendLine("Северные земли:")
            .AppendFormat("— войско богов: <b>{0}</b>.", Data.GetNorth(dateTime, NorthArmy.Gods))
            .AppendLine()
            .AppendFormat("— армия севера: <b>{0}</b>.", Data.GetNorth(dateTime, NorthArmy.North))
            .AppendLine()
            .AppendFormat("Ивенты {0}:", dowG);

        var dayEvents = Data.GetDayEvents(dateTime).ToList();

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

    private async Task ShowEvents()
    {
        var builder = new StringBuilder()
            .AppendFormat("<b>Текущие ивенты</b> (на {0:yyyy-MM-dd}) <b>:</b>", ServerTime.GetCurrent());

        foreach (var evt in Data.GetActiveEvents(ServerTime.GetCurrent()))
        {
            builder
                .AppendLine()
                .AppendFormat("{0} — <i>до {1}</i>", evt.Name, evt.Ends.ToString("yyyy-MM-dd HH:mm"));
        }

        await SendMessage(builder.ToString(), formatted: true);
    }

    private async Task StartDialog()
    {
        IReplyMarkup replyMarkup = new InlineKeyboardMarkup(new[]
        {
            new InlineKeyboardButton("ежа") { CallbackData = "ежа" },
            new InlineKeyboardButton("север") { CallbackData = "север" }
        });

        await _context.BotClient.SendTextMessageAsync(_context.ChatId, "Чем помочь?", replyMarkup: replyMarkup, cancellationToken: _context.CancellationToken);
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

    private async Task SendMessage(string message, bool formatted = false)
    {
        await _context.BotClient.SendTextMessageAsync(_context.ChatId, message, formatted ? ParseMode.Html : null, cancellationToken: _context.CancellationToken);
    }

    private async Task SendPhoto(string url)
    {
        await _context.BotClient.SendPhotoAsync(_context.ChatId, new InputOnlineFile(url), cancellationToken: _context.CancellationToken);
    }

    private static readonly Regex[] _murrTriggers =
    {
        new Regex("привет", RegexOptions.IgnoreCase),
        new Regex("доброе утро", RegexOptions.IgnoreCase),
        new Regex("добрый день", RegexOptions.IgnoreCase),
        new Regex("добрый вечер", RegexOptions.IgnoreCase)
    };
}
