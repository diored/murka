using DioRed.Murka.Common;

namespace DioRed.Murka.Core.Entities;

public record Event(string Name, ServerTimeRange Valid) : TimeLimitedEntity(Valid);