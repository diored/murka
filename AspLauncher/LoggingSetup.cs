using System.Diagnostics;

namespace DioRed.Murka.AspLauncher;

public static class LoggingSetup
{
    private const string LogName = "DioRED";

    public static void SetupDioRedLogging(this ILoggingBuilder logging, string source)
    {
        if (!EventLog.SourceExists(source))
        {
            EventLog.CreateEventSource(source, LogName);
        };

        logging.ClearProviders();
        logging.AddEventLog(settings =>
        {
            settings.LogName = LogName;
            settings.SourceName = source;
        });
    }
}