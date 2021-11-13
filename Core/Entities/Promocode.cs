namespace DioRed.Murka.Core.Entities;

public record Promocode(DateOnly ValidTo, string Code, string Content);