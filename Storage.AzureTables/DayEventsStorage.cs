using Azure.Data.Tables;

using DioRed.Common.AzureStorage;
using DioRed.Murka.Common;
using DioRed.Murka.Core.Entities;
using DioRed.Murka.Storage.Contracts;

namespace DioRed.Murka.Storage.AzureTables;

public class DayEventsStorage : StorageBase, IDayEventsStorage
{
    private readonly TableClient _tableClient;

    public DayEventsStorage(TableClient tableClient)
    {
        _tableClient = tableClient;
    }

    public DayEvent[] Get(DateOnly date, string chatId)
    {
        return _tableClient
            .Query<TableEntity>(e => e.PartitionKey == string.Empty || e.PartitionKey == chatId)
            .Select(FromEntity)
            .Where(e => e.Occurrence.Contains(date))
            .OrderBy(e => e.Occurrence.Time)
            .ToArray();
    }

    public void AddNew(DayEvent dayEvent)
    {
        TableEntity entity = new()
        {
            PartitionKey = dayEvent.ChatId ?? string.Empty,
            RowKey = StorageHelper.GenerateId(),
            Name = dayEvent.Name,
            Time = dayEvent.Occurrence.Time.ToString(ServerTime.TimeFormat),
            Occurrence = dayEvent.Occurrence switch
            {
                DailyOccurrence => "daily",
                WeeklyOccurrence weekly => $"weekly:{(int)weekly.DayOfWeek}",
                _ => throw new ArgumentOutOfRangeException(nameof(dayEvent.Occurrence), "Unsupported occurrence")
            }
        };

        _tableClient.AddEntity(entity);
    }

    private DayEvent FromEntity(TableEntity entity)
    {
        TimeOnly time = TimeOnly.ParseExact(entity.Time, ServerTime.TimeFormat);
        Occurrence occurrence = entity.Occurrence switch
        {
            "daily" => Occurrence.Daily(time),
            var x when x.StartsWith("weekly:") => Occurrence.Weekly((DayOfWeek)int.Parse(x["weekly:".Length..]), time),
            _ => throw new ArgumentOutOfRangeException(nameof(entity.Occurrence), "Unsupported occurrence")
        };
        return new DayEvent(entity.Name, occurrence, entity.PartitionKey);
    }

    private class TableEntity : BaseTableEntity
    {
        public string Name { get; set; } = default!;
        public string Time { get; set; } = default!;
        public string Occurrence { get; set; } = default!;
    }
}
