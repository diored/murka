using Azure.Data.Tables;

using DioRed.Common.AzureStorage;
using DioRed.Murka.Core.Entities;

using Microsoft.Extensions.DependencyInjection;

namespace DioRed.Murka.Infrastructure.AzureStorage;

public class PromocodesStorage(
    [FromKeyedServices("Murka")] AzureStorageClient azureStorage
) : StorageBase
{
    private readonly TableClient _tableClient = azureStorage.Table("Promocodes");

    public Promocode[] GetActive(ServerDateTime serverTime)
    {
        return [.. _tableClient
            .Query<TableEntity>()
            .Select(FromEntity)
            .Where(p => p.ValidAt(serverTime))
            .OrderBy(p => p.ValidTo?.ToString() ?? string.Empty)];
    }

    public void AddNew(Promocode promocode)
    {
        _tableClient.AddEntity(ToEntity(promocode));
    }

    public void Update(Promocode promocode)
    {
        _tableClient.UpdateEntity(ToEntity(promocode), Azure.ETag.All);
    }

    public void Remove(string code)
    {
        _tableClient.DeleteEntity(CommonValues.DefaultPartitionKey, code, Azure.ETag.All);
    }

    public string[] RemoveOutdated()
    {
        return [.. RemoveOutdated<TableEntity>(_tableClient)
            .Select(e => e.RowKey)];
    }

    private static Promocode FromEntity(TableEntity entity)
    {
        return new Promocode
        (
            entity.RowKey,
            entity.Content,
            ServerDateTime.ParseOrDefault(entity.ValidFrom),
            ServerDateTime.ParseOrDefault(entity.ValidTo)
        );
    }

    private static TableEntity ToEntity(Promocode promocode)
    {
        return new TableEntity
        {
            PartitionKey = CommonValues.DefaultPartitionKey,
            RowKey = promocode.Code,
            Content = promocode.Content,
            ValidFrom = promocode.ValidFrom?.ToString(),
            ValidTo = promocode.ValidTo?.ToString()
        };
    }

    private class TableEntity : TimeLimitedTableEntity
    {
        public string Content { get; init; } = default!;
    }
}