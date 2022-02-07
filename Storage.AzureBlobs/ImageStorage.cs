using DioRed.Common.AzureStorage;
using DioRed.Murka.Storage.Contracts;

namespace DioRed.Murka.Storage.Files;

public class ImageStorage : IImageStorage
{
    private readonly AzureStorageClient _azureStorageClient;

    public ImageStorage(AzureStorageClient azureStorageClient)
    {
        _azureStorageClient = azureStorageClient;
    }

    public BinaryData Get(string name)
    {
        return _azureStorageClient.Blob($"img/{name}").DownloadContent().Value.Content;
    }
}
