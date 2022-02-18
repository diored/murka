using DioRed.Common.AzureStorage;
using DioRed.Murka.Common;

namespace DioRed.Murka.Storage.Contracts;

public interface ITimeLimitedStorage<T>
{
    T[] GetActive(ServerTime serverTime);
    BaseTableEntity[] RemoveOutdated();
}
