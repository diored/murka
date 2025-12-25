using DioRed.Common.AzureStorage;

namespace DioRed.Murka.Infrastructure.AzureStorage;

internal class TimeLimitedTableEntity : BaseTableEntity
{
    public string? ValidFrom { get; init; }
    public string? ValidTo { get; init; }
}