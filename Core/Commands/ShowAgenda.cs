using DioRed.Murka.Core.Entities;
using DioRed.Vermilion.Handling;
using DioRed.Vermilion.Handling.Context;

namespace DioRed.Murka.Core.Commands;

public class ShowAgenda(
    ILogic logic
) : ICommandHandler
{
    public CommandDefinition Definition { get; } = new()
    {
        Template = new[] { "/daily", "сводка", "/tomorrow", "завтра" },
        HasTail = false,
        LogHandling = true
    };

    public async Task<bool> HandleAsync(
        MessageHandlingContext context,
        Feedback feedback
    )
    {
        DateOnly date = context.Message.Command is "/tomorrow" or "завтра"
            ? ServerDateTime.GetCurrent().Date.AddDays(1)
            : ServerDateTime.GetCurrent().Date;

        string agenda = await logic.BuildAgendaAsync(
            context.Chat.Id,
            date
        );

        await feedback.HtmlAsync(agenda);

        return true;
    }
}