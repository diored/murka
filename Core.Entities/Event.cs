namespace DioRed.Murka.Core.Entities;

public record Event(string Name, ServerDateTime? ValidFrom, ServerDateTime? ValidTo);