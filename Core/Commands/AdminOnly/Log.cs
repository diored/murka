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
        string message = string.IsNullOrWhiteSpace(context.Message.Tail)
            ? "<empty>"
            : context.Message.Tail;

        var sender = new
        {
            context.Sender.Name,
            context.Sender.Id,
            context.Sender.Role
        };

        var chat = new
        {
            context.Chat.Id,
            context.Chat.Title
        };

        logger.LogInformation(
            "Message: {Message}, Sender: {Sender}, Chat: {Chat}",
            message,
            sender,
            chat
        );

        return Task.FromResult(true);
    }
}