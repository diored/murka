using DioRed.Vermilion;
using DioRed.Vermilion.Handling;
using DioRed.Vermilion.Handling.Context;
using DioRed.Vermilion.Interaction;

namespace DioRed.Murka.Core.Commands.AdminOnly;

public class NotSoGlobalAnnounce : ICommandHandler
{
    public CommandDefinition Definition { get; } = new()
    {
        Template = "/ga?",
        TailPolicy = TailPolicy.HasTail,
        RequiredRole = UserRole.SuperAdmin,
        LogHandling = true
    };

    public async Task<bool> HandleAsync(
        MessageHandlingContext context,
        Feedback feedback
    )
    {
        await feedback.TextAsync($"""
            This will be announced:

            {context.Message.Tail}
            """
        );

        return true;
    }
}