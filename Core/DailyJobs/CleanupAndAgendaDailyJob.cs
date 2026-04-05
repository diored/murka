using DioRed.Common.Jobs;
using DioRed.Murka.Core.Entities;
using DioRed.Vermilion;
using DioRed.Vermilion.Interaction.Content;
using DioRed.Vermilion.Interaction.Receivers;
using DioRed.Vermilion.Jobs;

namespace DioRed.Murka.Core.DailyJobs;

public class CleanupAndAgendaDailyJob(
    ILogic logic
) : IScheduledJob
{
    public ScheduledJobDefinition Definition { get; } = new()
    {
        Schedule = new DailySchedule(
            timeOfDay: new TimeSpan(0, 0, 0),
            timeZone: CommonValues.ServerTimeZone
        ),
        Id = "Cleanup and agenda"
    };

    public async Task Handle(IServiceProvider services, BotCore botCore, CancellationToken ct = default)
    {
        await logic.CleanupAsync();

        await botCore.PostAsync(
            Receiver.Broadcast(chatInfo => !chatInfo.Tags.Contains("no-agenda")),
            async chatInfo => new HtmlContent
            {
                Html = await logic.BuildAgendaAsync(
                    chatInfo.ChatId,
                    ServerDateTime.GetCurrent().Date
                )
            }
        );
    }
}