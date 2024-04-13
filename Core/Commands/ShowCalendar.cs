using DioRed.Vermilion.Handling;
using DioRed.Vermilion.Handling.Context;

namespace DioRed.Murka.Core.Commands;

public class ShowCalendar(
    ILogic logic
) : ICommandHandler
{
    public CommandDefinition Definition { get; } = new()
    {
        Template = new[] { "/calendar", "календарь" },
        HasTail = false,
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