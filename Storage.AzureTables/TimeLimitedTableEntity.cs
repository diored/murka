using DioRed.Common.AzureStorage;
using DioRed.Murka.Common;

namespace DioRed.Murka.Storage.AzureTables;

internal class TimeLimitedTableEntity : BaseTableEntity
{
    public string? ValidFrom { get; set; }
    public string? ValidTo { get; set; }

    public ServerTimeRange GetValid()
    {
        ServerTime? validFrom = ServerTime.SafeParse(ValidFrom);
        ServerTime? validTo = ServerTime.SafeParse(ValidTo);

        return new ServerTimeRange(validFrom, validTo);
    }
}