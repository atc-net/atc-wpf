namespace Atc.Wpf.Hardware.Pickers.Internal;

/// <summary>
/// Hands camera frames from the capture thread to the UI thread without queueing one dispatcher operation per
/// frame: while a render is scheduled, a newer frame replaces the pending one (latest wins) and the older one is
/// disposed. The frame itself is handed over, not a copy, so the UI thread copies its pixels exactly once.
/// </summary>
/// <typeparam name="TFrame">The frame type; the slot disposes frames it drops.</typeparam>
internal sealed class LatestFrameSlot<TFrame>
    where TFrame : class, IDisposable
{
    private readonly Lock gate = new();
    private TFrame? pending;
    private bool renderScheduled;

    /// <summary>
    /// Gets a value indicating whether a frame is waiting for the UI thread, i.e. the UI is behind and the
    /// capture thread can skip acquiring the next frame altogether.
    /// </summary>
    public bool HasPendingFrame
    {
        get
        {
            lock (gate)
            {
                return pending is not null;
            }
        }
    }

    /// <summary>
    /// Capture thread: publishes a frame; the slot owns it from now on.
    /// </summary>
    /// <returns><see langword="true"/> when the caller must schedule a render; <see langword="false"/> when one is already scheduled.</returns>
    public bool Publish(TFrame frame)
    {
        ArgumentNullException.ThrowIfNull(frame);

        TFrame? dropped;
        bool scheduleRender;
        lock (gate)
        {
            dropped = pending;
            pending = frame;
            scheduleRender = !renderScheduled;
            renderScheduled = true;
        }

        // The UI has not caught up: drop the older frame.
        dropped?.Dispose();
        return scheduleRender;
    }

    /// <summary>
    /// UI thread: takes the latest frame. The caller owns and disposes it.
    /// </summary>
    public bool TryTake([NotNullWhen(true)] out TFrame? frame)
    {
        lock (gate)
        {
            renderScheduled = false;
            frame = pending;
            pending = null;
            return frame is not null;
        }
    }

    /// <summary>
    /// Disposes the pending frame, if any (when the preview stops).
    /// </summary>
    public void Clear()
    {
        TFrame? dropped;
        lock (gate)
        {
            dropped = pending;
            pending = null;
        }

        dropped?.Dispose();
    }
}