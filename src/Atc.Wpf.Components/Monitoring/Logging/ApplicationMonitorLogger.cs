// ReSharper disable CheckNamespace
namespace Atc.Wpf.Components.Monitoring.Logging;

/// <summary>
/// <see cref="ILogger"/> implementation that converts each log call into an
/// <see cref="ApplicationEventEntry"/> and dispatches it through an
/// <see cref="IMessenger"/> — picked up by any
/// <see cref="ApplicationMonitorViewModel"/> on its messenger.
/// </summary>
internal sealed class ApplicationMonitorLogger : ILogger
{
    private readonly string categoryName;
    private readonly IMessenger messenger;
    private readonly Func<LogLevel, bool>? filter;

    public ApplicationMonitorLogger(
        string categoryName,
        IMessenger messenger,
        Func<LogLevel, bool>? filter = null)
    {
        this.categoryName = categoryName;
        this.messenger = messenger;
        this.filter = filter;
    }

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull
        => null;

    public bool IsEnabled(LogLevel logLevel)
    {
        if (logLevel == LogLevel.None)
        {
            return false;
        }

        return filter?.Invoke(logLevel) ?? true;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        ArgumentNullException.ThrowIfNull(formatter);

        if (!IsEnabled(logLevel))
        {
            return;
        }

        var message = formatter(state, exception);
        if (string.IsNullOrEmpty(message) && exception is null)
        {
            return;
        }

        if (exception is not null)
        {
            message = string.IsNullOrEmpty(message)
                ? exception.ToString()
                : message + Environment.NewLine + exception;
        }

        var entry = new ApplicationEventEntry(
            MapLevel(logLevel),
            categoryName,
            message);

        messenger.Send(entry);
    }

    private static LogCategoryType MapLevel(LogLevel level)
        => level switch
        {
            LogLevel.Trace => LogCategoryType.Trace,
            LogLevel.Debug => LogCategoryType.Debug,
            LogLevel.Information => LogCategoryType.Information,
            LogLevel.Warning => LogCategoryType.Warning,
            LogLevel.Error => LogCategoryType.Error,
            LogLevel.Critical => LogCategoryType.Critical,
            _ => LogCategoryType.Information,
        };
}