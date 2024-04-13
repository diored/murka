using DioRed.Murka.Core.Entities;
using DioRed.Vermilion;
using DioRed.Vermilion.Handling;
using DioRed.Vermilion.Handling.Context;

namespace DioRed.Murka.Core.Commands.AdminOnly;

public class AddEvent(
    ILogic logic
) : ICommandHandler
{
    public CommandDefinition Definition { get; } = new()
    {
        Template = "/addEvent",
        HasTail = true,
        RequiredRole = UserRole.SuperAdmin,
        LogHandling = true
    };

    public async Task<bool> HandleAsync(
        MessageHandlingContext context,
        Feedback feedback
    )
    {
        if (context.Message.Args.Count is < 1 or > 3 ||
            context.Message.Args[0] is not { } eventName)
        {
            return false;
        }

        ServerDateTime? starts = context.Message.Args[1] is { } startsString
            ? ServerDateTime.Parse(startsString)
            : null;

        ServerDateTime? ends = context.Message.Args[2] is { } endsString
            ? ServerDateTime.Parse(endsString)
            : null;

        await logic.AddEventAsync(
            new Event(
                eventName,
                starts,
                ends
            )
        );

        await feedback.TextAsync($"""
            Event has been added:
            - Name: {eventName}
            - Starts: {starts}
            - Ends: {ends}
            """
        );

        return true;
    }
}