using DioRed.Vermilion.Handling;
using DioRed.Vermilion.Handling.Context;

namespace DioRed.Murka.Core.Commands;

public class ShowSeaSigns(
    ILogic logic
) : ICommandHandler
{
    public CommandDefinition Definition { get; } = new()
    {
        Template = new[] { "/sea", "море" },
        HasTail = false,
        LogHandling = true
    };

    public async Task<bool> HandleAsync(
        MessageHandlingContext context,
        Feedback feedback
    )
    {
        string link = await logic.GetLinkAsync("sea");
        await feedback.ImageAsync(link);

        return true;
    }
}