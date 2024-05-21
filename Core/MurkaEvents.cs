using Microsoft.Extensions.Logging;

namespace DioRed.Murka.Core;

public static class MurkaEvents
{
    // 21xx - database cleanup
    public static EventId CleanupStarted { get; } = new EventId(2100, "Cleanup started");
    public static EventId CleanupFinished { get; } = new EventId(2101, "Cleanup finished");
    public static EventId CleanupFailed { get; } = new EventId(2190, "Cleanup failed");
}