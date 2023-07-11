using Microsoft.Extensions.Logging;

namespace DioRed.Murka.Core;

public static class EventIDs
{
    // 1xxx — message handling
    // 19xx - error
    public static EventId MessageHandleException { get; } = new EventId(1900, "Message handler exception");

    // 2xxx - database maintenance
    // 21xx - cleanup
    public static EventId CleanupStarted { get; } = new EventId(2100, "Cleanup started");
    public static EventId CleanupFinished { get; } = new EventId(2101, "Cleanup finished");
    public static EventId CleanupFailed { get; } = new EventId(2190, "Cleanup failed");

    // 3xxx - chat management
    // 31xx - chat adding
    public static EventId ChatAdded { get; } = new EventId(3100, "Chat added");
    public static EventId ChatAddFailure { get; } = new EventId(3190, "Chat add failure");

    // 32xx - chat removing
    public static EventId ChatRemoved { get; } = new EventId(3200, "Chat removed");
    public static EventId ChatRemoveFailure { get; } = new EventId(3290, "Chat remove failure");
}