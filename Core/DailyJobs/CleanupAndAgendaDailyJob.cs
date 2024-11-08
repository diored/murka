using DioRed.Murka.Core.Entities;
using DioRed.Vermilion;
using DioRed.Vermilion.Interaction.Content;
using DioRed.Vermilion.Interaction.Receivers;
using DioRed.Vermilion.Jobs;

namespace DioRed.Murka.Core.DailyJobs;

public class CleanupAndAgendaDailyJob(
    ILogic logic
) : IDailyJob
{
    public DailyJobDefinition Definition { get; } = new()
    {
        TimeOfDay = new TimeOnly(0, 0, 0),
        TimeZoneOffset = CommonValues.ServerTimeZoneShift,
        Id = "Cleanup and agenda"
    };

    public async Task Handle(IServiceProvider services, BotCore botCore)
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