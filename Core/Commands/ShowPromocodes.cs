using System.Text;

using DioRed.Murka.Core.Entities;
using DioRed.Vermilion.Handling;
using DioRed.Vermilion.Handling.Context;
using DioRed.Vermilion.Interaction;

namespace DioRed.Murka.Core.Commands;

public class ShowPromocodes(
    ILogic logic
) : ICommandHandler
{
    public CommandDefinition Definition { get; } = new()
    {
        Template = new[] { "/promo", "промокоды" },
        TailPolicy = TailPolicy.HasNoTail,
        LogHandling = true
    };

    public async Task<bool> HandleAsync(
        MessageHandlingContext context,
        Feedback feedback
    )
    {
        ICollection<Promocode> activePromocodes = await logic.GetActivePromocodesAsync();

        if (activePromocodes.Count == 0)
        {
            await feedback.TextAsync("Активных промокодов нет");
            return true;
        }

        StringBuilder builder = new("<b>Активные промокоды:</b>");

        foreach (Promocode promocode in activePromocodes)
        {
            builder.Append($"""


                <code>{promocode.Code}</code>
                """);

            if (promocode.ValidTo.HasValue)
            {
                builder.Append($" (до {promocode.ValidTo})");
            }

            builder.Append($"""

                {promocode.Content}
                """);
        }

        await feedback.HtmlAsync(builder.ToString());

        return true;
    }
}