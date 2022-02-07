using DioRed.Murka.Storage.Contracts;

namespace DioRed.Murka.Core;

public interface IStorageEndpoint
{
    IPromocodesStorage Promocodes { get; }
    IEventsStorage Events { get; }
    IDailiesStorage Dailies { get; }
    IChatsStorage Chats { get; }
    ILogStorage Log { get; }
    IImageStorage Images { get; }
}
