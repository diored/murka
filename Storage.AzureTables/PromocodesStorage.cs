using Azure.Data.Tables;

using DioRed.Common.AzureStorage;
using DioRed.Murka.Common;
using DioRed.Murka.Core.Entities;
using DioRed.Murka.Storage.Contracts;

namespace DioRed.Murka.Storage.AzureTables;

public class PromocodesStorage : IPromocodesStorage
{
    private readonly TableClient _tableClient;

    public PromocodesStorage(TableClient tableClient)
    {
        _tableClient = tableClient;
    }

    public Promocode[] GetActive(DateTime dateTime)
    {
        return _tableClient
            .Query<TableEntity>(entity => entity.ValidTo >= dateTime)
            .OrderBy(entity => entity.ValidTo)
            .Select(entity => new Promocode(entity.ValidTo, entity.RowKey, entity.Content))
            .ToArray();
    }

    public void AddNew(Promocode promocode)
    {
        TableEntity entity = new()
        {
            PartitionKey = Guid.NewGuid().ToString(),
            RowKey = promocode.Code,
            Content = promocode.Content,
            ValidTo = promocode.ValidTo
        };

        _tableClient.AddEntity(entity);
    }

    public void RemoveOutdated()
    {
        DateTime dateTime = ServerTime.GetCurrent();

        TableEntity[] entitiesToRemove = _tableClient
            .Query<TableEntity>(entity => entity.ValidTo < dateTime)
            .ToArray();

        foreach (var entity in entitiesToRemove)
        {
            _tableClient.DeleteEntity(entity.PartitionKey, entity.RowKey);
        }
    }

    private class TableEntity : BaseTableEntity
    {
        public string Content { get; set; } = default!;
        public DateTime ValidTo { get; set; }
    }
}
