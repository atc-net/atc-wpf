using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ReSharper disable CheckNamespace
namespace Atc.Wpf.Components.Monitoring.Logging;

/// <summary>
/// Wires the Application Monitor as a Microsoft.Extensions.Logging provider.
/// Every <see cref="ILogger"/> call (including <c>ILogger&lt;T&gt;</c>) routes
/// into the live picker view via <see cref="Messenger.Default"/>.
/// </summary>
/// <example>
/// <code>
/// var builder = Host.CreateApplicationBuilder();
/// builder.Logging.AddAtcWpfApplicationMonitor();
/// </code>
/// </example>
public static class ApplicationMonitorLoggingBuilderExtensions
{
    /// <summary>
    /// Adds <see cref="ApplicationMonitorLoggerProvider"/> as a singleton
    /// <see cref="ILoggerProvider"/> on the supplied <paramref name="builder"/>.
    /// Uses the default <see cref="Messenger"/> instance.
    /// </summary>
    public static ILoggingBuilder AddAtcWpfApplicationMonitor(this ILoggingBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddSingleton<ILoggerProvider, ApplicationMonitorLoggerProvider>();
        return builder;
    }

    /// <summary>
    /// Adds <see cref="ApplicationMonitorLoggerProvider"/> with an optional
    /// per-level filter. Useful for excluding noisy <see cref="LogLevel.Trace"/>
    /// or <see cref="LogLevel.Debug"/> entries from the picker without
    /// affecting other logging providers.
    /// </summary>
    public static ILoggingBuilder AddAtcWpfApplicationMonitor(
        this ILoggingBuilder builder,
        Func<LogLevel, bool>? filter)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddSingleton<ILoggerProvider>(
            _ => new ApplicationMonitorLoggerProvider(messenger: null, filter));
        return builder;
    }
}
