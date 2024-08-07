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
        LogHandling = false
    };

    public Task<bool> HandleAsync(
        MessageHandlingContext context,
        Feedback feedback
    )
    {
        logger.LogInformation(
            "Message: {Message}, Sender: {Sender}, Chat: {Chat}",
            context.Message.Tail,
            (context.Sender.Id, context.Sender.Role, context.Sender.Name),
            (context.Chat.Id, context.Chat.Title)
        );

        return Task.FromResult(true);
    }
}