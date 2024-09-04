using DioRed.Vermilion.Handling;
using DioRed.Vermilion.Handling.Context;
using DioRed.Vermilion.Handling.Templates;
using DioRed.Vermilion.Interaction;

namespace DioRed.Murka.Core.Commands;

public abstract class ShowAgendaBase(
    ILogic logic,
    Template template,
    Func<DateOnly> getDateFunc
) : ICommandHandler
{
    public CommandDefinition Definition { get; } = new()
    {
        Template = template,
        HasTail = false,
        LogHandling = true
    };

    public async Task<bool> HandleAsync(
        MessageHandlingContext context,
        Feedback feedback
    )
    {
        DateOnly date = getDateFunc();

        string agenda = await logic.BuildAgendaAsync(
            context.Chat.Id,
            date
        );

        await feedback.HtmlAsync(agenda);

        return true;
    }
}