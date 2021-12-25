using Azure.Data.Tables;

using DioRed.Common.AzureStorage;
using DioRed.Murka.Common;
using DioRed.Murka.Core.Entities;
using DioRed.Murka.Storage.Contracts;

namespace DioRed.Murka.Storage.AzureTables;

public class EventsStorage : IEventsStorage
{
    private readonly TableClient _tableClient;

    public EventsStorage(TableClient tableClient)
    {
        _tableClient = tableClient;
    }

    public Event[] GetActive(DateTime dateTime)
    {
        return _tableClient
            .Query<TableEntity>(entity => entity.Ends >= dateTime)
            .OrderBy(entity => entity.Ends)
            .Select(entity => new Event(entity.Name, entity.Ends))
            .ToArray();
    }

    public void AddNew(Event @event)
    {
        TableEntity entity = new()
        {
            PartitionKey = "RU",
            RowKey = Guid.NewGuid().ToString(),
            Name = @event.Name,
            Ends = @event.Ends
        };

        _tableClient.AddEntity(entity);
    }

    public void RemoveOutdated()
    {
        DateTime dateTime = ServerTime.GetCurrent();

        TableEntity[] entitiesToRemove = _tableClient
            .Query<TableEntity>(entity => entity.Ends < dateTime)
            .ToArray();

        foreach (var entity in entitiesToRemove)
        {
            _tableClient.DeleteEntity(entity.PartitionKey, entity.RowKey);
        }
    }

    private class TableEntity : BaseTableEntity
    {
        public string Name { get; set; } = default!;
        public DateTime Ends { get; set; }
        public string? Servers { get; set; }
    }
}
