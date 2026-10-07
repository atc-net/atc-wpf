namespace Atc.Wpf.Hardware.Pickers.Internal;

/// <summary>
/// Hands camera frames from the capture thread to the UI thread without allocating a buffer per frame and
/// without queueing one dispatcher operation per frame: while a render is scheduled, a newer frame simply
/// replaces the pending one (latest wins), and buffers of the current frame size are recycled.
/// </summary>
internal sealed class LatestFrameSlot
{
    private readonly Lock gate = new();
    private readonly Stack<byte[]> spareBuffers = new();
    private LatestFrame? pending;
    private bool renderScheduled;

    /// <summary>
    /// Capture thread: gets a buffer of <paramref name="length"/> bytes to fill with the next frame.
    /// </summary>
    public byte[] RentWriteBuffer(int length)
    {
        lock (gate)
        {
            while (spareBuffers.Count > 0)
            {
                var spare = spareBuffers.Pop();
                if (spare.Length == length)
                {
                    return spare;
                }
            }
        }

        return new byte[length];
    }

    /// <summary>
    /// Capture thread: publishes a filled buffer.
    /// </summary>
    /// <returns><see langword="true"/> when the caller must schedule a render; <see langword="false"/> when one is already scheduled.</returns>
    public bool Publish(
        byte[] buffer,
        int width,
        int height)
    {
        lock (gate)
        {
            if (pending is not null)
            {
                // The UI has not caught up: drop the older frame and recycle its buffer.
                spareBuffers.Push(pending.Buffer);
            }

            pending = new LatestFrame(buffer, width, height);

            if (renderScheduled)
            {
                return false;
            }

            renderScheduled = true;
            return true;
        }
    }

    /// <summary>
    /// UI thread: takes the latest frame. Return its buffer with <see cref="Return"/> once it has been copied.
    /// </summary>
    public bool TryTake([NotNullWhen(true)] out LatestFrame? frame)
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
    /// UI thread: gives a rendered frame's buffer back for reuse.
    /// </summary>
    public void Return(byte[] buffer)
    {
        lock (gate)
        {
            spareBuffers.Push(buffer);
        }
    }
}