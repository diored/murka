using Azure.Data.Tables;

using DioRed.Common.AzureStorage;
using DioRed.Murka.Core.Entities;
using DioRed.Murka.Storage.Contracts;

namespace DioRed.Murka.Storage.AzureTables;

public class DailiesStorage : IDailiesStorage
{
    private readonly TableClient _tableClient;

    public DailiesStorage(TableClient tableClient)
    {
        _tableClient = tableClient;
    }

    public Daily Get(DateTime dateTime)
    {
        TableEntity entity = _tableClient
            .GetEntity<TableEntity>(dateTime.Month.ToString(), dateTime.Day.ToString())
            .Value;

        return _dailies[entity.Code];
    }

    private static readonly Dictionary<string, Daily> _dailies = new()
    {
        ["W"] = new("Оружие", "ПВ2 (Аурогон) / ПП / МИ / ГШ"),
        ["A"] = new("Доспех", "ПВ1 (Чернокрыл) / СЦ / ХХ 4-1 / ХХ 4-2"),
        ["R"] = new("Реликвия", "ХС / ЛА / ДР")
    };

    private class TableEntity : BaseTableEntity
    {
        public string Code { get; set; } = default!;
    }
}
