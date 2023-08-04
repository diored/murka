using DioRed.Vermilion;

namespace DioRed.Murka.Core;

internal class ChatStorage : IChatStorage
{
    private readonly ILogic _logic;

    public ChatStorage(ILogic logic)
    {
        _logic = logic;
    }

    public void AddChat(ChatId chatId, string title)
    {
        _logic.AddChat(chatId, title);
    }

    public ICollection<ChatId> GetChats()
    {
        return _logic.GetChats();
    }

    public void RemoveChat(ChatId chatId)
    {
        _logic.RemoveChat(chatId);
    }
}