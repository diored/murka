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
        LogHandling = true,
        RequiredRole = UserRole.SuperAdmin
    };

    public async Task<bool> HandleAsync(
        MessageHandlingContext context,
        Feedback feedback
    )
    {
        Func<ChatId, Task<IContent>> buildAgenda = async (ChatId chatId) => new HtmlContent
        {
            Html = await logic.BuildAgendaAsync(
                chatId,
                ServerDateTime.GetCurrent().Date
            )
        };

        if (context.Message.Args.Count > 0 &&
            long.TryParse(context.Message.Args[0], out long receiverId))
        {
            await feedback.To(chatId => chatId.Id == receiverId).ContentAsync(buildAgenda);
        }
        else
        {
            await feedback.ToEveryone().ContentAsync(buildAgenda);
        }

        return true;
    }
}