using Azure.Data.Tables;

using DioRed.Common.AzureStorage;

using Microsoft.Extensions.DependencyInjection;

namespace DioRed.Murka.Infrastructure.AzureStorage;

public class LinksStorage(
    [FromKeyedServices("Murka")] AzureStorageClient azureStorage
)
{
    private readonly TableClient _tableClient = azureStorage.Table("Links");

    public string? Get(string id)
    {
        var entity = _tableClient
            .GetEntityIfExists<TableEntity>("link", id);

        return entity is { HasValue: true, Value: not null }
            ? entity.Value.Path
            : null;
    }

    private class TableEntity : BaseTableEntity
    {
        public string Path { get; set; } = default!;
    }
}