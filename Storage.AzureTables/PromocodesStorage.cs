using Azure.Data.Tables;

using DioRed.Common.AzureStorage;
using DioRed.Murka.Common;
using DioRed.Murka.Core.Entities;
using DioRed.Murka.Storage.Contracts;

namespace DioRed.Murka.Storage.AzureTables;

public class PromocodesStorage : StorageBase, IPromocodesStorage
{
    private readonly TableClient _tableClient;

    public PromocodesStorage(TableClient tableClient)
    {
        _tableClient = tableClient;
    }

    public Promocode[] GetActive(ServerTime serverTime)
    {
        return _tableClient
            .Query<TableEntity>()
            .Select(FromEntity)
            .Where(p => p.Valid.Contains(serverTime))
            .OrderBy(p => p.Valid.To?.ToString() ?? "")
            .ToArray();
    }

    public void AddNew(Promocode promocode)
    {
        TableEntity entity = new()
        {
            PartitionKey = StorageHelper.GenerateId(),
            RowKey = promocode.Code,
            Content = promocode.Content,
            ValidFrom = promocode.Valid.From?.ToString(),
            ValidTo = promocode.Valid.To?.ToString()
        };

        _tableClient.AddEntity(entity);
    }

    public BaseTableEntity[] RemoveOutdated() => RemoveOutdated<TableEntity>(_tableClient);

    private Promocode FromEntity(TableEntity entity)
    {
        return new Promocode(entity.RowKey, entity.Content, entity.GetValid());
    }

    private class TableEntity : TimeLimitedTableEntity
    {
        public string Content { get; set; } = default!;

        public override string ToString() => RowKey;
    }
}
