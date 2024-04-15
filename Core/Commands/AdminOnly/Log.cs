using DioRed.Vermilion;
using DioRed.Vermilion.Handling;
using DioRed.Vermilion.Handling.Context;
using DioRed.Vermilion.Interaction;

using Microsoft.Extensions.Logging;

namespace DioRed.Murka.Core.Commands.AdminOnly;

public class Log(
    ILogger<Log> logger
) : ICommandHandler
{
    public CommandDefinition Definition { get; } = new()
    {
        Template = "/log",
        HasTail = true,
        RequiredRole = UserRole.SuperAdmin,
        LogHandling = false
    };

    public Task<bool> HandleAsync(
        MessageHandlingContext context,
        Feedback feedback
    )
    {
        logger.LogInformation("{Message}", context.Message.Tail);

        return Task.FromResult(true);
    }
}