using Azure.Data.Tables;

using DioRed.Common.AzureStorage;
using DioRed.Murka.Core.Entities;
using DioRed.Murka.Storage.Contracts;

namespace DioRed.Murka.Storage.AzureTables;

public class ChatsStorage : IChatsStorage
{
    private readonly TableClient _tableClient;

    public ChatsStorage(TableClient tableClient)
    {
        _tableClient = tableClient;
    }

    public void Add(ChatInfo chatInfo)
    {
        TableEntity entity = new()
        {
            PartitionKey = chatInfo.Type,
            RowKey = chatInfo.Id,
            Title = chatInfo.Title
        };

        _tableClient.AddEntity(entity);
    }

    public ICollection<ChatInfo> GetTelegramChats()
    {
        return _tableClient
            .Query<TableEntity>()
            .Where(entity => entity.PartitionKey.StartsWith("Telegram"))
            .Select(entity => new ChatInfo(entity.RowKey, entity.PartitionKey, entity.Title))
            .ToArray();
    }

    public void Remove(ChatInfo chatInfo)
    {
        _tableClient.DeleteEntity(chatInfo.Type, chatInfo.Id);
    }

    private class TableEntity : BaseTableEntity
    {
        public string Title { get; set; } = default!;
    }
}
