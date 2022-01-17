using DioRed.Common.AzureStorage;
using DioRed.Murka.Storage.Contracts;

namespace DioRed.Murka.Storage.AzureTables;

public class StorageEndpoint : IStorageEndpoint
{
    public StorageEndpoint(string accountName, string storageAccountKey)
    {
        AzureTableStorageSettings storageSettings = new(accountName, storageAccountKey);
        AzureTableStorageClient storageClient = new(storageSettings);

        Promocodes = new PromocodesStorage(storageClient.Table("Promocodes"));
        Events = new EventsStorage(storageClient.Table("Events"));
        Dailies = new DailiesStorage(storageClient.Table("Dailies"));
        Chats = new ChatsStorage(storageClient.Table("Chats"));
        Log = new LogStorage(storageClient.Table("Log"));
    }

    public IPromocodesStorage Promocodes { get; }
    public IEventsStorage Events { get; }
    public IDailiesStorage Dailies { get; }
    public IChatsStorage Chats { get; }
    public ILogStorage Log { get; }
}
