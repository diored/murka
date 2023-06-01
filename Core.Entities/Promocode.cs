namespace DioRed.Murka.Core.Entities;

public record Promocode(string Code, string Content, ServerDateTime? ValidFrom, ServerDateTime? ValidTo);