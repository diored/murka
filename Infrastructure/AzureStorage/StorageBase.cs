using Azure.Data.Tables;

using DioRed.Murka.Core.Entities;

namespace DioRed.Murka.Infrastructure.AzureStorage;

public abstract class StorageBase
{
    internal static T[] RemoveOutdated<T>(TableClient tableClient)
        where T : TimeLimitedTableEntity, new()
    {
        ServerDateTime dateTime = ServerDateTime.GetCurrent();

        T[] entitiesToRemove = [.. tableClient
            .Query<T>()
            .Where(entity =>
            {
                ServerDateTime? validTo = ServerDateTime.ParseOrDefault(entity.ValidTo);
                return validTo.HasValue && validTo.Value < dateTime;
            })];

        foreach (var entity in entitiesToRemove)
        {
            tableClient.DeleteEntity(entity.PartitionKey, entity.RowKey);
        }

        return entitiesToRemove;
    }

    protected static string GenerateId()
    {
        return Guid.NewGuid().ToString()[^12..];
    }
}