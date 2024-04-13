using DioRed.Murka.Core.Entities;
using DioRed.Vermilion.Handling;
using DioRed.Vermilion.Handling.Context;

namespace DioRed.Murka.Core.Commands;

public class ShowNorth(
    ILogic logic
) : ICommandHandler
{
    public CommandDefinition Definition { get; } = new()
    {
        Template = new[] { "/north", "север" },
        HasTail = false,
        LogHandling = true
    };

    public async Task<bool> HandleAsync(
        MessageHandlingContext context,
        Feedback feedback
    )
    {
        DateOnly date = ServerDateTime.GetCurrent().Date;

        Northlands northlands = await logic.GetNorthLandsAsync(date);

        await feedback.TextAsync($"""
            Расписание ивентов в СЗ:
            — войско богов: {northlands.Gods}
            — армия севера: {northlands.North}
            """
        );

        return true;
    }
}