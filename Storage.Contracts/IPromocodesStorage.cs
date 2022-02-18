using DioRed.Murka.Core.Entities;

namespace DioRed.Murka.Storage.Contracts;

public interface IPromocodesStorage : ITimeLimitedStorage<Promocode>
{
    void AddNew(Promocode newPromocode);
}
