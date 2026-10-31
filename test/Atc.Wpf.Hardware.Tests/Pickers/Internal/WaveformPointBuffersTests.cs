namespace Atc.Wpf.Hardware.Tests.Pickers.Internal;

public sealed class WaveformPointBuffersTests
{
    private const int Capacity = 4;

    [StaFact]
    public void Fill_AlternatesBetweenTwoReusedPairs()
    {
        var sut = new WaveformPointBuffers(Capacity);
        var ring = new float[Capacity];

        var first = sut.Fill(ring, 0, 100, 50);
        var second = sut.Fill(ring, 0, 100, 50);
        var third = sut.Fill(ring, 0, 100, 50);

        Assert.NotSame(first.Top, second.Top);
        Assert.NotSame(first.Bottom, second.Bottom);
        Assert.Same(first.Top, third.Top);
        Assert.Same(first.Bottom, third.Bottom);
    }

    [StaFact]
    public void Fill_ReadsTheRingOldestToNewestAndMirrorsTheLines()
    {
        var sut = new WaveformPointBuffers(Capacity);
        float[] ring = [0.5f, 1f, 2f, 0f];

        var (top, bottom) = sut.Fill(ring, ringHead: 1, width: 72, height: 24);

        // Oldest first: ring[1], ring[2] (clamped to 1), ring[3], ring[0]; mid 12, half height 10.
        Assert.Equal([new Point(0, 2), new Point(20, 2), new Point(40, 12), new Point(60, 7)], top);
        Assert.Equal([new Point(0, 22), new Point(20, 22), new Point(40, 12), new Point(60, 17)], bottom);
    }

    [StaFact]
    public void Fill_ABufferClearedMeanwhile_IsRebuiltInFull()
    {
        var sut = new WaveformPointBuffers(Capacity);
        var ring = new float[Capacity];
        var first = sut.Fill(ring, 0, 100, 50);
        sut.Fill(ring, 0, 100, 50);
        first.Top.Clear();

        var third = sut.Fill(ring, 0, 100, 50);

        Assert.Equal(Capacity, third.Top.Count);
        Assert.Equal(Capacity, third.Bottom.Count);
    }
}