namespace DioRed.Murka.Core.Entities;

public record ChatInfo(string Id, string Type, string Title)
{
    public string ChatId { get; } = $"{Type}:{Id}";
}