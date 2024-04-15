using DioRed.Murka.Core.Entities;
using DioRed.Vermilion;
using DioRed.Vermilion.Handling;
using DioRed.Vermilion.Handling.Context;
using DioRed.Vermilion.Interaction;
using DioRed.Vermilion.Interaction.Content;

namespace DioRed.Murka.Core.Commands;

public class BroadcastAgenda(
    ILogic logic
) : ICommandHandler
{
    public CommandDefinition Definition { get; } = new()
    {
        Template = new[] { "/agenda!" },
        HasTail = false,
        LogHandling = true,
        RequiredRole = UserRole.SuperAdmin
    };

    public async Task<bool> HandleAsync(
        MessageHandlingContext context,
        Feedback feedback
    )
    {
        DateOnly date = context.Message.Command is "/tomorrow" or "завтра"
            ? ServerDateTime.GetCurrent().Date.AddDays(1)
            : ServerDateTime.GetCurrent().Date;

        Func<ChatId, Task<IContent>> buildAgenda = async (ChatId chatId) => new HtmlContent
        {
            Html = await logic.BuildAgendaAsync(
                chatId,
                date
            )
        };

        await feedback.ToEveryone().ContentAsync(buildAgenda);

        return true;
    }
}