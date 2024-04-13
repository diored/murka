using DioRed.Murka.Core;
using DioRed.Murka.Core.Entities;
using DioRed.Murka.Manager.Models;

namespace DioRed.Murka.Manager.Data;

public class DataProvider(ILogic logic)
{
    public async Task<List<PromocodeModel>> GetActivePromocodesAsync()
    {
        var active = await logic.GetActivePromocodesAsync();

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

    public async Task<bool> AddPromocodeAsync(PromocodeModel promocode)
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
            await logic.AddPromocodeAsync(entity);
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
            logic.UpdatePromocodeAsync(entity);
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
            logic.RemovePromocodeAsync(promocode.Code);
            return true;
        }
        catch
        {
            return false;
        }
    }
}