using DioRed.Murka.Storage.Contracts;

namespace DioRed.Murka.Core;

public interface IStorageEndpoint
{
    IChatsStorage Chats { get; }
    IDailiesStorage Dailies { get; }
    IDayEventsStorage DayEvents { get; }
    IEventsStorage Events { get; }
    IImageStorage Images { get; }
    ILogStorage Log { get; }
    IPromocodesStorage Promocodes { get; }
}
