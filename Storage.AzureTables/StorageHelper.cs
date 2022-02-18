using DioRed.Murka.Common;

namespace DioRed.Murka.Storage.AzureTables;

internal static class StorageHelper
{
    public static string GenerateId()
    {
        return Guid.NewGuid().ToString()[^12..];
    }
}
