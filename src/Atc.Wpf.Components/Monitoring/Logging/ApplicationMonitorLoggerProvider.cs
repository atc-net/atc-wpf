using Microsoft.Extensions.Logging;

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
    private readonly System.Collections.Concurrent.ConcurrentDictionary<string, ApplicationMonitorLogger> loggers
        = new();

    private readonly IMessenger messenger;
    private readonly Func<LogLevel, bool>? filter;

    public ApplicationMonitorLoggerProvider()
        : this(messenger: null, filter: null)
    {
    }

    public ApplicationMonitorLoggerProvider(
        IMessenger? messenger,
        Func<LogLevel, bool>? filter)
    {
        this.messenger = messenger ?? Messenger.Default;
        this.filter = filter;
    }

    public ILogger CreateLogger(string categoryName)
        => loggers.GetOrAdd(
            categoryName,
            static (name, args) => new ApplicationMonitorLogger(name, args.messenger, args.filter),
            (messenger, filter));

    public void Dispose()
        => loggers.Clear();
}
