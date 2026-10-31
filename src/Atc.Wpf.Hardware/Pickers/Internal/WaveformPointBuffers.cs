namespace Atc.Wpf.Hardware.Pickers.Internal;

/// <summary>
/// The point collections for the two mirrored waveform lines of the audio meter and tester. They are redrawn at
/// 30 Hz, so instead of allocating two new collections per frame, two pairs are reused in turn: the pair being
/// filled is never the one the lines currently show, so filling it raises no render invalidations.
/// </summary>
internal sealed class WaveformPointBuffers
{
    private readonly PointCollection[] topBuffers;
    private readonly PointCollection[] bottomBuffers;
    private int next;

    public WaveformPointBuffers(int capacity)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 2);

        Capacity = capacity;
        topBuffers = [new PointCollection(capacity), new PointCollection(capacity)];
        bottomBuffers = [new PointCollection(capacity), new PointCollection(capacity)];
    }

    public int Capacity { get; }

    /// <summary>
    /// Fills the pair not currently shown with the ring of peaks, read oldest to newest so the trace scrolls.
    /// </summary>
    /// <param name="ring">The peaks, <see cref="Capacity"/> long.</param>
    /// <param name="ringHead">The index of the oldest peak.</param>
    /// <param name="width">The canvas width.</param>
    /// <param name="height">The canvas height.</param>
    /// <returns>The filled pair, to assign to the lines.</returns>
    public (PointCollection Top, PointCollection Bottom) Fill(
        float[] ring,
        int ringHead,
        double width,
        double height)
    {
        ArgumentNullException.ThrowIfNull(ring);

        var top = topBuffers[next];
        var bottom = bottomBuffers[next];
        next = 1 - next;

        // A buffer can have been cleared (the tester clears the shown lines when it stops).
        var rebuild = top.Count != Capacity || bottom.Count != Capacity;
        if (rebuild)
        {
            top.Clear();
            bottom.Clear();
        }

        var midY = height / 2;
        var halfHeight = (height / 2) - 2;

        for (var i = 0; i < Capacity; i++)
        {
            var sample = ring[(ringHead + i) % Capacity];
            if (sample > 1f)
            {
                sample = 1f;
            }

            var x = (i / (double)(Capacity - 1)) * (width - 12);
            var topPoint = new Point(x, midY - (sample * halfHeight));
            var bottomPoint = new Point(x, midY + (sample * halfHeight));

            if (rebuild)
            {
                top.Add(topPoint);
                bottom.Add(bottomPoint);
            }
            else
            {
                top[i] = topPoint;
                bottom[i] = bottomPoint;
            }
        }

        return (top, bottom);
    }
}