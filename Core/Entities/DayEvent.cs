namespace DioRed.Murka.Core.Entities;

public record DayEvent(string Name, string? ChatId, TimeOnly Time, string Occurrence);