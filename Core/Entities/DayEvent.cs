using DioRed.Vermilion;

namespace DioRed.Murka.Core.Entities;

public record DayEvent(string Name, ChatId? ChatId, TimeOnly Time, string Occurrence);