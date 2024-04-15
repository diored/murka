using System.Text;

using DioRed.Murka.Core.Entities;
using DioRed.Vermilion.Handling;
using DioRed.Vermilion.Handling.Context;
using DioRed.Vermilion.Interaction;

namespace DioRed.Murka.Core.Commands;

public class ShowEvents(
    ILogic logic
) : ICommandHandler
{
    public CommandDefinition Definition { get; } = new()
    {
        Template = new[] { "/events", "ивенты" },
        HasTail = false,
        LogHandling = true
    };

    public async Task<bool> HandleAsync(
        MessageHandlingContext context,
        Feedback feedback
    )
    {
        ICollection<Event> events = await logic.GetActiveEventsAsync();

        if (events.Count == 0)
        {
            await feedback.TextAsync("На данный момент ивентов нет.");

            return true;
        }

        DateOnly date = ServerDateTime.GetCurrent().Date;

        StringBuilder builder = new($"<b>Текущие ивенты</b> (на {date:yyyy-MM-dd}) <b>:</b>");

        foreach (var evt in events)
        {
            builder.Append($"""

                — {evt.Name}
                """
            );

            if (evt.ValidTo.HasValue)
            {
                builder.Append(" — <i>");

                if (evt.ValidTo.Value.Date == date)
                {
                    builder.Append("сегодня последний день");
                }
                else if (evt.ValidTo.Value.Date == date.AddDays(1))
                {
                    builder.Append("до завтра");
                }
                else
                {
                    builder.Append($"до {evt.ValidTo.Value}");
                }

                builder.Append("</i>");
            }
        }

        await feedback.HtmlAsync(builder.ToString());

        return true;
    }
}