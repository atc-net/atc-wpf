namespace Atc.Wpf.Tests.TestSupport;

/// <summary>
/// Collects the Trace messages written while it is attached.
/// </summary>
internal sealed class TraceCapture : TraceListener
{
    private readonly List<string> messages = [];

    private TraceCapture()
    {
    }

    public IReadOnlyList<string> Messages
    {
        get
        {
            lock (messages)
            {
                return [.. messages];
            }
        }
    }

    public static TraceCapture Start()
    {
        var capture = new TraceCapture();
        Trace.Listeners.Add(capture);
        return capture;
    }

    public int Count(string fragment)
        => Messages.Count(x => x.Contains(fragment, StringComparison.Ordinal));

    public override void Write(string? message)
    {
    }

    public override void WriteLine(string? message)
    {
        lock (messages)
        {
            messages.Add(message ?? string.Empty);
        }
    }

    protected override void Dispose(bool disposing)
    {
        Trace.Listeners.Remove(this);
        base.Dispose(disposing);
    }
}