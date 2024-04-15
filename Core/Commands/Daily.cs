using DioRed.Murka.Core.Entities;
using DioRed.Murka.Graphics;
using DioRed.Vermilion.Handling;
using DioRed.Vermilion.Handling.Context;
using DioRed.Vermilion.Interaction;

namespace DioRed.Murka.Core.Commands;

public class Daily(
    ILogic logic
) : ICommandHandler
{
    public CommandDefinition Definition { get; } = new()
    {
        Template = new[] { "/daily", "ежа" },
        LogHandling = true
    };

    public async Task<bool> HandleAsync(
        MessageHandlingContext context,
        Feedback feedback
    )
    {
        int days;
        switch (context.Message.Args)
        {
            case []:
                days = 5;
                break;

            case [{ } daysArg] when int.TryParse(daysArg, out days) && days is > 0 and < 30:
                break;

            default:
                return false;
        }

        ServerDateTime serverTime = ServerDateTime.GetCurrent();

        ScheduleItem[] schedule = await Task.WhenAll(
            Enumerable.Range(0, days)
                .Select(async i =>
                {
                    DateOnly date = serverTime.Date.AddDays(i);
                    return new ScheduleItem(
                        date,
                        (await logic.GetDailyAsync(date)).Id
                    );
                })
        );

        const string folder = "forecasts";
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        string fileName = $"""{folder}\{serverTime.Date:yyyy_MM_dd}_{days}.png""";

        if (!File.Exists(fileName))
        {
            File.WriteAllBytes(
                fileName,
                [.. ForecastBuilder.BuildImage(schedule)]
            );
        }

        using Stream fileStream = File.OpenRead(fileName);

        await feedback.ImageAsync(fileStream);

        return true;
    }
}