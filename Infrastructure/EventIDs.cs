using Microsoft.Extensions.Logging;

namespace DioRed.Murka.Infrastructure;

public static class EventIDs
{
    public static EventId MurkaCleanup { get; } = new(1001, "Murka API: outdated records removed");

    public static EventId MurkaMigrationSuccess { get; } = new(1101, "Murka API: entity migration success");
    public static EventId MurkaMigrationFailed { get; } = new(1102, "Murka API: entity migration failed");
}