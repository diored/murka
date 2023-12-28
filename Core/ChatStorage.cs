using DioRed.Vermilion;

namespace DioRed.Murka.Core;

internal class ChatStorage(ILogic logic) : IChatStorage
{
    public void AddChat(ChatId chatId, string title)
    {
        logic.AddChat(chatId, title);
    }

    public ICollection<ChatId> GetChats()
    {
        return logic.GetChats();
    }

    public void RemoveChat(ChatId chatId)
    {
        logic.RemoveChat(chatId);
    }
}