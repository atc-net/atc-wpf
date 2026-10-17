namespace Atc.Wpf.Theming.Tests.TestSupport;

/// <summary>
/// Records what WPF data binding writes to <see cref="PresentationTraceSources.DataBindingSource"/>
/// (the "System.Windows.Data Error: NN" lines), and restores the trace source when disposed.
/// </summary>
internal sealed class BindingTraceCapture : TraceListener
{
    private readonly TraceSource source;
    private readonly SourceLevels originalLevel;
    private readonly List<(int Id, string Message)> events = [];

    private BindingTraceCapture()
    {
        // WPF only traces data binding when a debugger is attached, the registry asks for it,
        // or the trace sources have been refreshed.
        PresentationTraceSources.Refresh();

        source = PresentationTraceSources.DataBindingSource;
        originalLevel = source.Switch.Level;
        source.Switch.Level = SourceLevels.Warning;
        source.Listeners.Add(this);
    }

    public IReadOnlyList<(int Id, string Message)> Events
    {
        get
        {
            lock (events)
            {
                return events.ToArray();
            }
        }
    }

    public static BindingTraceCapture Start() => new();

    public override void TraceEvent(
        TraceEventCache? eventCache,
        string source,
        TraceEventType eventType,
        int id,
        string? message)
        => Record(id, message ?? string.Empty);

    public override void TraceEvent(
        TraceEventCache? eventCache,
        string source,
        TraceEventType eventType,
        int id,
        string? format,
        params object?[]? args)
    {
        var message = format is null
            ? string.Empty
            : args is null || args.Length == 0
                ? format
                : string.Format(CultureInfo.InvariantCulture, format, args);
        Record(id, message);
    }

    public override void Write(string? message)
    {
        // Only TraceEvent carries the event id; the header lines written around it are not needed.
    }

    public override void WriteLine(string? message)
    {
        // See Write.
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            source.Listeners.Remove(this);
            source.Switch.Level = originalLevel;
        }

        base.Dispose(disposing);
    }

    private void Record(
        int id,
        string message)
    {
        lock (events)
        {
            events.Add((id, message));
        }
    }
}