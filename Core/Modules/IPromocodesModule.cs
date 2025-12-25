using DioRed.Murka.Core.Entities;

namespace DioRed.Murka.Core.Modules;

public interface IPromocodesModule
{
    void Add(Promocode newPromocode);
    void Cleanup();
    Promocode[] GetActive();
    void Remove(string code);
    void Update(Promocode promocode);
}