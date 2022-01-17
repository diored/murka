using DioRed.Common.AzureStorage;

namespace DioRed.Murka.Storage.Contracts;

public interface ITimeBasedStorage<T>
{
    void AddNew(T entity);
    T[] GetActive(DateTime dateTime);
    BaseTableEntity[] RemoveOutdated();
}
