using DioRed.Vermilion.Handling;
using DioRed.Vermilion.Handling.Context;
using DioRed.Vermilion.Interaction;

namespace DioRed.Murka.Core.Commands;

public class ShowCalendar(
    ILogic logic
) : ICommandHandler
{
    public CommandDefinition Definition { get; } = new()
    {
        Template = new[] { "/calendar", "календарь" },
        TailPolicy = TailPolicy.HasNoTail,
        LogHandling = true
    };

    public async Task<bool> HandleAsync(
        MessageHandlingContext context,
        Feedback feedback
    )
    {
        string link = await logic.GetLinkAsync("daily");

        await feedback.ImageAsync(link);

        return true;
    }
}