namespace DioRed.Murka.Core.Entities;

public record Daily(string Code, string Definition)
{
    public static Daily Empty { get; } = new Daily(string.Empty, string.Empty);
}
