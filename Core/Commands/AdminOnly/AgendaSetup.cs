using DioRed.Vermilion;
using DioRed.Vermilion.Handling;
using DioRed.Vermilion.Handling.Context;
using DioRed.Vermilion.Interaction;

namespace DioRed.Murka.Core.Commands.AdminOnly;

public class AgendaSetup : ICommandHandler
{
    public CommandDefinition Definition { get; } = new()
    {
        Template = "/agenda",
        HasTail = true,
        RequiredRole = UserRole.ChatAdmin,
        LogHandling = true
    };

    public async Task<bool> HandleAsync(
        MessageHandlingContext context,
        Feedback feedback
    )
    {
        bool status;
        switch (context.Message.Tail.ToUpper())
        {
            case "ON":
                await feedback.RemoveTagAsync("no-agenda");
                status = true;
                break;

            case "OFF":
                await feedback.AddTagAsync("no-agenda");
                status = false;
                break;

            default:
                return false;
        }

        await feedback.TextAsync($"Agenda subscription status: {(status ? "ON" : "OFF")}");

        return true;
    }
}