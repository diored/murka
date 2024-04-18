using System.Collections.Concurrent;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Spectre.Console;

namespace DioRed.Murka.ConsoleLauncher;

public class CleanConsoleLogger : ILogger
{
    public LogLevel LogLevel { get; set; }

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= LogLevel;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        string message = formatter(state, exception);

        string timestamp = $"[lime]{DateTime.Now:HH:mm:ss}[/] • ";

        string messageColor = logLevel switch
        {
            LogLevel.Trace => "grey",
            LogLevel.Debug => "teal",
            LogLevel.Warning => "yellow",
            LogLevel.Error => "red",
            LogLevel.Critical => "maroon",
            _ => "silver"
        };

        string suffix = eventId != 0
            ? $" [darkseagreen4]#{eventId}[/]"
            : "";

        AnsiConsole.MarkupLine($"{timestamp}[{messageColor}]{message}[/]{suffix}");

        if (exception is not null)
        {
            AnsiConsole.WriteException(exception);
        }
    }
}

public class CleanConsoleLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, CleanConsoleLogger> _loggers = new();

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, name => new CleanConsoleLogger());
    }

    public void Dispose()
    {
        _loggers.Clear();
    }
}

public static class CleanConsoleLoggerFactoryExtensions
{
    public static ILoggingBuilder AddCleanConsole(this ILoggingBuilder builder)
    {
        builder.Services.AddSingleton<ILoggerProvider, CleanConsoleLoggerProvider>();
        return builder;
    }
}