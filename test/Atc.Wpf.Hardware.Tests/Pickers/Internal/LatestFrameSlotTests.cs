namespace Atc.Wpf.Hardware.Tests.Pickers.Internal;

public sealed class LatestFrameSlotTests
{
    private const int Width = 4;
    private const int Height = 2;
    private const int Length = Width * Height * 4;

    [Fact]
    public void Publish_WhileARenderIsAlreadyScheduled_DoesNotAskForAnotherRender()
    {
        var sut = new LatestFrameSlot();

        var first = sut.Publish(sut.RentWriteBuffer(Length), Width, Height);
        var second = sut.Publish(sut.RentWriteBuffer(Length), Width, Height);

        Assert.True(first);
        Assert.False(second);
    }

    [Fact]
    public void TryTake_AfterSeveralPublishes_ReturnsOnlyTheLatestFrame()
    {
        var sut = new LatestFrameSlot();
        var older = sut.RentWriteBuffer(Length);
        older[0] = 1;
        sut.Publish(older, Width, Height);
        var latest = sut.RentWriteBuffer(Length);
        latest[0] = 2;
        sut.Publish(latest, Width, Height);

        Assert.True(sut.TryTake(out var frame));
        Assert.Equal(2, frame.Buffer[0]);
        Assert.Equal(Width, frame.Width);
        Assert.Equal(Height, frame.Height);
        Assert.False(sut.TryTake(out _));
    }

    [Fact]
    public void Publish_AfterTheScheduledRenderTookItsFrame_AsksForANewRender()
    {
        var sut = new LatestFrameSlot();
        sut.Publish(sut.RentWriteBuffer(Length), Width, Height);
        sut.TryTake(out var frame);
        sut.Return(frame.Buffer);

        Assert.True(sut.Publish(sut.RentWriteBuffer(Length), Width, Height));
    }

    [Fact]
    public void RentWriteBuffer_ReusesReturnedAndDroppedBuffersOfTheSameSize()
    {
        var sut = new LatestFrameSlot();
        var a = sut.RentWriteBuffer(Length);
        sut.Publish(a, Width, Height);
        var b = sut.RentWriteBuffer(Length);
        sut.Publish(b, Width, Height); // a is dropped and recycled

        var c = sut.RentWriteBuffer(Length);
        sut.TryTake(out var frame); // b
        sut.Return(frame.Buffer);
        var d = sut.RentWriteBuffer(Length);

        Assert.Same(a, c);
        Assert.Same(b, d);
    }

    [Fact]
    public void RentWriteBuffer_ForADifferentFrameSize_AllocatesAMatchingBuffer()
    {
        var sut = new LatestFrameSlot();
        var small = sut.RentWriteBuffer(Length);
        sut.Publish(small, Width, Height);
        sut.TryTake(out var frame);
        sut.Return(frame.Buffer);

        var large = sut.RentWriteBuffer(Length * 4);

        Assert.NotSame(small, large);
        Assert.Equal(Length * 4, large.Length);
    }
}