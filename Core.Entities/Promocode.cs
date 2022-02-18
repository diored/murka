using DioRed.Murka.Common;

namespace DioRed.Murka.Core.Entities;

public record Promocode(string Code, string Content, ServerTimeRange Valid) : TimeLimitedEntity(Valid);