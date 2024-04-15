using DioRed.Vermilion;
using DioRed.Vermilion.Handling;
using DioRed.Vermilion.Handling.Context;
using DioRed.Vermilion.Interaction;

namespace DioRed.Murka.Core.Commands.AdminOnly;

public class Echo : ICommandHandler
{
    public CommandDefinition Definition { get; } = new()
    {
        Template = "/e",
        RequiredRole = UserRole.SuperAdmin
    };

    public async Task<bool> HandleAsync(
        MessageHandlingContext context,
        Feedback feedback
    )
    {
        await feedback.TextAsync(context.Message.Text);

        return true;
    }
}