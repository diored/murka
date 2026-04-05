using DioRed.Murka.Core.Entities;
using DioRed.Vermilion;
using DioRed.Vermilion.Handling;
using DioRed.Vermilion.Handling.Context;
using DioRed.Vermilion.Interaction;
using DioRed.Vermilion.Interaction.Content;

namespace DioRed.Murka.Core.Commands.AdminOnly;

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
        Feedback feedback,
        CancellationToken ct = default
    )
    {
        async Task<IContent> buildAgenda(ChatMetadata chatMetadata) => new HtmlContent
        {
            Html = await logic.BuildAgendaAsync(
                chatMetadata.ChatId,
                ServerDateTime.GetCurrent().Date
            )
        };

        Feedback receiver = context.Message.Args.Count > 0 &&
            long.TryParse(context.Message.Args[0], out long receiverId)
            ? feedback.To(chatInfo => chatInfo.ChatId.Id == receiverId)
            : feedback.ToEveryone();

        await receiver.ContentAsync(buildAgenda, ct);

        return true;
    }
}