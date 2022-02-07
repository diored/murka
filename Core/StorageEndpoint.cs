using DioRed.Common.AzureStorage;
using DioRed.Murka.Storage.AzureTables;
using DioRed.Murka.Storage.Contracts;
using DioRed.Murka.Storage.Files;

namespace DioRed.Murka.Core;

public class StorageEndpoint : IStorageEndpoint
{
    public StorageEndpoint(string accountName, string storageAccountKey)
    {
        AzureStorageSettings storageSettings = new(accountName, storageAccountKey);
        AzureStorageClient storageClient = new(storageSettings);

        Promocodes = new PromocodesStorage(storageClient.Table("Promocodes"));
        Events = new EventsStorage(storageClient.Table("Events"));
        Dailies = new DailiesStorage(storageClient.Table("Dailies"));
        Chats = new ChatsStorage(storageClient.Table("Chats"));
        Log = new LogStorage(storageClient.Table("Log"));

        Images = new ImageStorage(storageClient);
    }

    public IPromocodesStorage Promocodes { get; }
    public IEventsStorage Events { get; }
    public IDailiesStorage Dailies { get; }
    public IChatsStorage Chats { get; }
    public ILogStorage Log { get; }
    public IImageStorage Images { get; }
}
