using Microsoft.Extensions.Logging;

namespace DioRed.Murka.Core;

public static class EventIDs
{
    public static EventId MessageHandleException { get; } = new EventId(1, "Message handler exception");

    public static EventId CleanupStarted { get; } = new EventId(101, "Cleanup started");
    public static EventId CleanupFinished { get; } = new EventId(102, "Cleanup finished");
    public static EventId CleanupFailed { get; } = new EventId(103, "Cleanup failed");

    public static EventId ChatAdded { get; } = new EventId(201, "Chat added");
    public static EventId ChatAddFailure { get; } = new EventId(202, "Chat add failure");

    public static EventId ChatRemoved { get; } = new EventId(301, "Chat removed");
    public static EventId ChatRemoveFailure { get; } = new EventId(302, "Chat remove failure");
}