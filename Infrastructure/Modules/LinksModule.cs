using DioRed.Murka.Core.Modules;
using DioRed.Murka.Infrastructure.AzureStorage;

namespace DioRed.Murka.Infrastructure.Modules;

public class LinksModule(LinksStorage storage) : ILinksModule
{
    public string? Get(string id)
    {
        return storage.Get(id);
    }
}