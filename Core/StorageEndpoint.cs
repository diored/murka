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

        Chats = new ChatsStorage(storageClient.Table("Chats"));
        Dailies = new DailiesStorage(storageClient.Table("Dailies"));
        DayEvents = new DayEventsStorage(storageClient.Table("DayEvents"));
        Events = new EventsStorage(storageClient.Table("Events"));
        Log = new LogStorage(storageClient.Table("Log"));
        Promocodes = new PromocodesStorage(storageClient.Table("Promocodes"));

        Images = new ImageStorage(storageClient);
    }

    public IChatsStorage Chats { get; }
    public IDailiesStorage Dailies { get; }
    public IDayEventsStorage DayEvents { get; }
    public IEventsStorage Events { get; }
    public IImageStorage Images { get; }
    public ILogStorage Log { get; }
    public IPromocodesStorage Promocodes { get; }
}
