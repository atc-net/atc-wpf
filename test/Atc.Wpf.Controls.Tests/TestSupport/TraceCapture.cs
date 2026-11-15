namespace Atc.Wpf.Controls.Tests.TestSupport;

/// <summary>
/// Records the messages written to <see cref="Trace"/> (for example <see cref="Trace.TraceWarning(string)"/>) while it is alive,
/// and removes itself from <see cref="Trace.Listeners"/> when disposed. <see cref="Trace.Listeners"/> is process-wide, so tests
/// that count messages run in <see cref="TraceTestGroup"/>, apart from all other tests, and filter the messages they count.
/// </summary>
internal sealed class TraceCapture : TraceListener
{
    private readonly List<string> messages = [];

    private TraceCapture()
        => Trace.Listeners.Add(this);

    public static TraceCapture Start() => new();

    public int Count(params string[] parts)
    {
        lock (messages)
        {
            return messages.Count(message => parts.All(part => message.Contains(part, StringComparison.Ordinal)));
        }
    }

    public override void TraceEvent(
        TraceEventCache? eventCache,
        string source,
        TraceEventType eventType,
        int id,
        string? message)
        => Record(message ?? string.Empty);

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
        Record(message);
    }

    public override void Write(string? message)
    {
        // Only the messages of TraceEvent are recorded; the header lines written around them are not needed.
    }

    public override void WriteLine(string? message)
    {
        // See Write.
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Trace.Listeners.Remove(this);
        }

        base.Dispose(disposing);
    }

    private void Record(string message)
    {
        lock (messages)
        {
            messages.Add(message);
        }
    }
}