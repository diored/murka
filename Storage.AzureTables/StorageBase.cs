using Azure.Data.Tables;

using DioRed.Murka.Common;

namespace DioRed.Murka.Storage.AzureTables;

public abstract class StorageBase
{
    internal static T[] RemoveOutdated<T>(TableClient tableClient)
        where T : TimeLimitedTableEntity, new()
    {
        ServerTime dateTime = ServerTime.GetCurrent();

        var entitiesToRemove = tableClient
            .Query<T>()
            .Where(entity =>
            {
                ServerTime? validTo = entity.GetValid().To;
                return validTo.HasValue && validTo.Value < dateTime;
            })
            .ToArray();

        foreach (var entity in entitiesToRemove)
        {
            tableClient.DeleteEntity(entity.PartitionKey, entity.RowKey);
        }

        return entitiesToRemove;
    }
}