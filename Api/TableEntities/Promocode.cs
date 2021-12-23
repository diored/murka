using DioRed.Common.AzureStorage;

namespace DioRed.Murka.Api.TableEntities;

public class Promocode : BaseTableEntity
{
    public string Content { get; set; } = default!;
    public DateTime ValidTo { get; set; }
}
