using DioRed.Murka.Core;
using DioRed.Murka.Core.Entities;
using DioRed.Murka.Manager.Models;

namespace DioRed.Murka.Manager.Data;

public class DataProvider(ILogic logic)
{
    public List<PromocodeModel> GetActivePromocodes()
    {
        var active = logic.GetActivePromocodes();

        return active
            .Select(item => new PromocodeModel
            {
                Code = item.Code,
                ValidFrom = item.ValidFrom?.ToString(),
                ValidTo = item.ValidTo?.ToString(),
                Content = item.Content
            })
            .ToList();
    }

    public bool AddPromocode(PromocodeModel promocode)
    {
        var entity = new Promocode
        (
            promocode.Code,
            promocode.Content,
            ServerDateTime.ParseOrDefault(promocode.ValidFrom),
            ServerDateTime.ParseOrDefault(promocode.ValidTo)
        );

        try
        {
            logic.AddPromocode(entity);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool UpdatePromocode(PromocodeModel promocode)
    {
        var entity = new Promocode
        (
            promocode.Code,
            promocode.Content,
            ServerDateTime.ParseOrDefault(promocode.ValidFrom),
            ServerDateTime.ParseOrDefault(promocode.ValidTo)
        );

        try
        {
            logic.UpdatePromocode(entity);
            return true;
        }
        catch
        {
            return false;
        }

    }

    public bool RemovePromocode(PromocodeModel promocode)
    {
        try
        {
            logic.RemovePromocode(promocode.Code);
            return true;
        }
        catch
        {
            return false;
        }
    }
}