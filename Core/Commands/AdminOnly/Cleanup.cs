using DioRed.Vermilion;
using DioRed.Vermilion.Handling;
using DioRed.Vermilion.Handling.Context;
using DioRed.Vermilion.Interaction;

namespace DioRed.Murka.Core.Commands.AdminOnly;

public class Cleanup(
    ILogic logic
) : ICommandHandler
{
    public CommandDefinition Definition { get; } = new()
    {
        Template = "/cleanup",
        TailPolicy = TailPolicy.HasNoTail,
        RequiredRole = UserRole.SuperAdmin,
        LogHandling = true
    };

    public async Task<bool> HandleAsync(
        MessageHandlingContext context,
        Feedback feedback,
        CancellationToken ct = default
    )
    {
        await logic.CleanupAsync();
        await feedback.TextAsync("Cleanup done", ct);

        return true;
    }
}