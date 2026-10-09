// ReSharper disable CheckNamespace
namespace Atc.Wpf.Components.Monitoring.Logging;

/// <summary>
/// Registers per-category loggers that forward every <c>ILogger</c> call into
/// any <see cref="ApplicationMonitorViewModel"/> listening on the supplied
/// <see cref="IMessenger"/> (defaults to <see cref="Messenger.Default"/>).
/// </summary>
[ProviderAlias("AtcWpfApplicationMonitor")]
public sealed class ApplicationMonitorLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, ApplicationMonitorLogger> loggers
        = new(StringComparer.Ordinal);

    private readonly IMessenger messenger;
    private readonly Func<LogLevel, bool>? filter;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationMonitorLoggerProvider"/> class that forwards
    /// all log levels through <see cref="Messenger.Default"/>.
    /// </summary>
    public ApplicationMonitorLoggerProvider()
        : this(messenger: null, filter: null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationMonitorLoggerProvider"/> class.
    /// </summary>
    /// <param name="messenger">The messenger to send entries through, or <see langword="null"/> to use <see cref="Messenger.Default"/>.</param>
    /// <param name="filter">An optional predicate that decides which log levels are forwarded, or <see langword="null"/> to forward all levels.</param>
    public ApplicationMonitorLoggerProvider(
        IMessenger? messenger,
        Func<LogLevel, bool>? filter)
    {
        this.messenger = messenger ?? Messenger.Default;
        this.filter = filter;
    }

    /// <inheritdoc />
    public ILogger CreateLogger(string categoryName)
        => loggers.GetOrAdd(
            categoryName,
            static (name, args) => new ApplicationMonitorLogger(name, args.messenger, args.filter),
            (messenger, filter));

    /// <inheritdoc />
    public void Dispose()
        => loggers.Clear();
}