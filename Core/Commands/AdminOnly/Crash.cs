using DioRed.Vermilion;
using DioRed.Vermilion.Handling;
using DioRed.Vermilion.Handling.Context;
using DioRed.Vermilion.Interaction;

namespace DioRed.Murka.Core.Commands.AdminOnly;

public class Crash : ICommandHandler
{
    public CommandDefinition Definition { get; } = new()
    {
        Template = "/crash",
        RequiredRole = UserRole.SuperAdmin,
        LogHandling = true
    };

    public async Task<bool> HandleAsync(
        MessageHandlingContext context,
        Feedback feedback,
        CancellationToken ct = default
    )
    {
        await feedback.TextAsync("crashing", ct);

        throw new InvalidOperationException("Controlled crashing");
    }
}