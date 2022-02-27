using Azure.Data.Tables;

using DioRed.Common.AzureStorage;
using DioRed.Murka.Common;
using DioRed.Murka.Core.Entities;
using DioRed.Murka.Storage.Contracts;

namespace DioRed.Murka.Storage.AzureTables;

public class EventsStorage : StorageBase, IEventsStorage
{
    private readonly TableClient _tableClient;

    public EventsStorage(TableClient tableClient)
    {
        _tableClient = tableClient;
    }

    public Event[] GetActive(ServerTime serverTime)
    {
        return _tableClient
            .Query<TableEntity>()
            .Select(FromEntity)
            .Where(e => e.Valid.Contains(serverTime))
            .OrderBy(e => e.Valid.To?.ToString() ?? "")
            .ToArray();
    }

    public void AddNew(Event @event)
    {
        TableEntity entity = new()
        {
            PartitionKey = "RU",
            RowKey = StorageHelper.GenerateId(),
            Name = @event.Name,
            ValidFrom = @event.Valid.From?.ToString(),
            ValidTo = @event.Valid.To?.ToString()
        };

        _tableClient.AddEntity(entity);
    }

    public BaseTableEntity[] RemoveOutdated() => RemoveOutdated<TableEntity>(_tableClient);

    private Event FromEntity(TableEntity entity)
    {
        return new Event(entity.Name, entity.GetValid());
    }

    private class TableEntity : TimeLimitedTableEntity
    {
        public string Name { get; set; } = default!;

        public override string ToString() => Name;
    }
}
