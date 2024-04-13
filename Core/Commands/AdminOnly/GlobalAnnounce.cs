using DioRed.Vermilion;
using DioRed.Vermilion.Handling;
using DioRed.Vermilion.Handling.Context;

namespace DioRed.Murka.Core.Commands.AdminOnly;

public class GlobalAnnounce : ICommandHandler
{
    public CommandDefinition Definition { get; } = new()
    {
        Template = "/ga",
        HasTail = true,
        RequiredRole = UserRole.SuperAdmin,
        LogHandling = true
    };

    public async Task<bool> HandleAsync(
        MessageHandlingContext context,
        Feedback feedback
    )
    {
        await feedback.ToEveryone().TextAsync(context.Message.Tail);

        return true;
    }
}