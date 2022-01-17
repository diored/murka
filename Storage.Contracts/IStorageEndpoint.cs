﻿namespace DioRed.Murka.Storage.Contracts;

public interface IStorageEndpoint
{
    IPromocodesStorage Promocodes { get; }
    IEventsStorage Events { get; }
    IDailiesStorage Dailies { get; }
    IChatsStorage Chats { get; }
    ILogStorage Log { get; }
}
