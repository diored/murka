using DioRed.Common.AzureStorage;

using Microsoft.Extensions.DependencyInjection;

namespace DioRed.Murka.Infrastructure.AzureStorage;

public class ImagesStorage(
    [FromKeyedServices("Murka")] AzureStorageClient azureStorage
    ) : StorageBase
{
    public BinaryData Get(string name)
    {
        return azureStorage.Blob("img", name).DownloadContent().Value.Content;
    }
}