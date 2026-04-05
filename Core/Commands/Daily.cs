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
        Feedback feedback,
        CancellationToken ct = default
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

        string folder = Path.Combine(
            GetAppRootDirectory(),
            "shared",
            "forecasts"
        );
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        string fileName = Path.Combine(folder, $"{serverTime.Date:yyyy_MM_dd}_{days}.png");

        if (!File.Exists(fileName))
        {
            File.WriteAllBytes(
                fileName,
                [.. ForecastBuilder.BuildImage(schedule)]
            );
        }

        using Stream fileStream = File.OpenRead(fileName);

        await feedback.ImageAsync(fileStream, ct);

        return true;
    }

    private static string GetAppRootDirectory()
    {
        DirectoryInfo baseDirectory = new(Path.GetFullPath(AppContext.BaseDirectory));
        string normalizedName = baseDirectory.Name.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        if (string.Equals(normalizedName, "current", StringComparison.OrdinalIgnoreCase))
        {
            return baseDirectory.FullName;
        }

        if (baseDirectory.Parent is { } releasesDirectory &&
            string.Equals(releasesDirectory.Name, "releases", StringComparison.OrdinalIgnoreCase) &&
            releasesDirectory.Parent is { } appRootDirectory)
        {
            return appRootDirectory.FullName;
        }

        return baseDirectory.FullName;
    }
}
