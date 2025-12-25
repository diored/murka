using Azure.Data.Tables;

using DioRed.Common.AzureStorage;
using DioRed.Murka.Core.Entities;

using Microsoft.Extensions.DependencyInjection;

namespace DioRed.Murka.Infrastructure.AzureStorage;

public class ParametersStorage(
    [FromKeyedServices("Murka")] AzureStorageClient azureStorage
) : StorageBase
{
    private readonly TableClient _tableClient = azureStorage.Table("Parameters");

    public int GetInt32(string key)
    {
        return _tableClient
            .Query<TableEntity>()
            .Where(e => e.PartitionKey == CommonValues.DefaultPartitionKey
                && e.RowKey == key
                && e.IntValue.HasValue)
            .Single()
            .IntValue!.Value;
    }

    public void SetInt32(string key, int value)
    {
        _tableClient.AddEntity(new TableEntity
        {
            PartitionKey = CommonValues.DefaultPartitionKey,
            RowKey = key,
            IntValue = value
        });
    }

    private class TableEntity : BaseTableEntity
    {
        public int? IntValue { get; init; }
    }
}