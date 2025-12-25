using Azure.Data.Tables;

using DioRed.Common.AzureStorage;
using DioRed.Murka.Core.Entities;
using DioRed.Vermilion;

using Microsoft.Extensions.DependencyInjection;

namespace DioRed.Murka.Infrastructure.AzureStorage;

public class DayEventsStorage(
    [FromKeyedServices("Murka")] AzureStorageClient azureStorage
) : StorageBase
{
    private readonly TableClient _tableClient = azureStorage.Table("DayEvents");

    public DayEvent[] Get(DateOnly date, ChatId chatId)
    {
        return [.. _tableClient
            .Query<TableEntity>(e => e.PartitionKey == string.Empty || e.PartitionKey == $"{chatId.ConnectorKey}:{chatId.Id}")
            .Select(FromEntity)
            .Where(e => OccurrenceContains(e.Occurrence, date))
            .OrderBy(e => e.Time)];
    }

    public void AddNew(DayEvent dayEvent)
    {
        string chatString = dayEvent.ChatId.HasValue
            ? $"{dayEvent.ChatId.Value.ConnectorKey}:{dayEvent.ChatId.Value.Id}"
            : string.Empty;

        TableEntity entity = new()
        {
            PartitionKey = chatString,
            RowKey = GenerateId(),
            Name = dayEvent.Name,
            Time = dayEvent.Time.ToString(CommonValues.TimeFormat),
            Occurrence = dayEvent.Occurrence
        };

        _tableClient.AddEntity(entity);
    }

    private DayEvent FromEntity(TableEntity entity)
    {
        ChatId? chatId = null;

        if (entity.PartitionKey.Split(':') is [{ } system, { } id] && long.TryParse(id, out long idValue))
        {
            chatId = new ChatId(system, string.Empty, idValue);
        }

        return new DayEvent
        (
            entity.Name,
            chatId,
            TimeOnly.ParseExact(entity.Time, CommonValues.TimeFormat),
            entity.Occurrence
        );
    }

    private static bool OccurrenceContains(string occurrence, DateOnly date)
    {
        if (occurrence == "daily")
        {
            return true;
        }

        if (occurrence.StartsWith("weekly:"))
        {
            return int.Parse(occurrence["weekly:".Length..]) == (int)date.DayOfWeek;
        }

        return false;
    }

    private class TableEntity : BaseTableEntity
    {
        public required string Name { get; init; }
        public required string Time { get; init; }
        public required string Occurrence { get; init; }
    }
}