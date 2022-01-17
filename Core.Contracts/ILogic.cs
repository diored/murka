using DioRed.Murka.Core.Entities;

namespace DioRed.Murka.Core.Contracts;

public interface ILogic
{
    void Cleanup();
    ICollection<Event> GetActiveEvents(DateTime dateTime);
    ICollection<Promocode> GetActivePromocodes(DateTime dateTime);
    Daily GetDaily(DateTime dateTime);
    ICollection<string> GetDayEvents(DateTime dateTime);
    Northlands GetNorthLands(DateTime dateTime);
    string GetRandomGreeting();
    ICollection<ChatInfo> GetChats();
    void AddChat(ChatInfo chatInfo);
    void RemoveChat(ChatInfo chatInfo);
    void Log(string level, string message, object? argumentObject = null, Exception? exception = null);

}
