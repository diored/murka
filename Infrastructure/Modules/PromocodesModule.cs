using DioRed.Murka.Core.Entities;
using DioRed.Murka.Core.Modules;
using DioRed.Murka.Infrastructure.AzureStorage;

using Microsoft.Extensions.Logging;

namespace DioRed.Murka.Infrastructure.Modules;

public class PromocodesModule(PromocodesStorage storage, ILogger<PromocodesModule> logger) : IPromocodesModule
{
    public Promocode[] GetActive()
    {
        return storage.GetActive(ServerDateTime.GetCurrent());
    }

    public void Add(Promocode newPromocode)
    {
        storage.AddNew(newPromocode);
    }

    public void Update(Promocode promocode)
    {
        storage.Update(promocode);
    }

    public void Remove(string code)
    {
        storage.Remove(code);
    }

    public void Cleanup()
    {
        string[] removed = storage.RemoveOutdated();
        if (removed.Length != 0)
        {
            logger.LogInformation(EventIDs.MurkaCleanup, "Outdated promocodes [{Promocodes}] were removed", string.Join(", ", removed));
        }
    }
}