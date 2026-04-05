using Azure.Data.Tables;

using DioRed.Common.AzureStorage;
using DioRed.Murka.Core.Entities;

using Microsoft.Extensions.DependencyInjection;

namespace DioRed.Murka.Infrastructure.AzureStorage;

public class EventsStorage(
    [FromKeyedServices("Murka")] AzureStorageClient azureStorage
) : StorageBase
{
    private readonly TableClient _tableClient = azureStorage.Table("Events");

    public Event[] GetActive(ServerDateTime serverTime)
    {
        return [.. _tableClient
            .Query<TableEntity>()
            .Select(FromEntity)
            .Where(e => e.ValidAt(serverTime))
            .OrderBy(e => e.ValidTo?.ToString() ?? string.Empty)];
    }

    public void AddNew(Event newEvent)
    {
        _tableClient.AddEntity(ToEntity(newEvent));
    }

    public string[] RemoveOutdated()
    {
        return RemoveOutdated<TableEntity>(_tableClient)
            .Select(e => e.Name)
            .ToArray();
    }

    private static Event FromEntity(TableEntity entity)
    {
        return new Event
        (
            entity.Name,
            ServerDateTime.ParseOrDefault(entity.ValidFrom),
            ServerDateTime.ParseOrDefault(entity.ValidTo)
        );
    }

    private static TableEntity ToEntity(Event @event)
    {
        return new TableEntity
        {
            PartitionKey = "RU",
            RowKey = GenerateId(),
            Name = @event.Name,
            ValidFrom = @event.ValidFrom?.ToString(),
            ValidTo = @event.ValidTo?.ToString()
        };
    }

    private class TableEntity : TimeLimitedTableEntity
    {
        public string Name { get; init; } = default!;
    }
}