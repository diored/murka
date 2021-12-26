using DioRed.Murka.Core.Entities;

namespace DioRed.Murka.Storage.Contracts;

public interface IChatsStorage
{
    ICollection<ChatInfo> GetTelegramChats();
    void Add(ChatInfo chatInfo);
    void Remove(ChatInfo chatInfo);
}
