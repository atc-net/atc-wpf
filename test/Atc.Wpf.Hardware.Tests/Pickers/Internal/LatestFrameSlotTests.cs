namespace Atc.Wpf.Hardware.Tests.Pickers.Internal;

[SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "The slot owns the published frames; the tests check what it disposes.")]
public sealed class LatestFrameSlotTests
{
    [Fact]
    public void Publish_WhileARenderIsAlreadyScheduled_DoesNotAskForAnotherRender()
    {
        var sut = new LatestFrameSlot<Frame>();

        var first = sut.Publish(new Frame(1));
        var second = sut.Publish(new Frame(2));

        Assert.True(first);
        Assert.False(second);
    }

    [Fact]
    public void TryTake_AfterSeveralPublishes_ReturnsOnlyTheLatestFrameAndDisposesTheDroppedOnes()
    {
        var sut = new LatestFrameSlot<Frame>();
        var older = new Frame(1);
        var latest = new Frame(2);
        sut.Publish(older);
        sut.Publish(latest);

        Assert.True(sut.TryTake(out var frame));
        Assert.Same(latest, frame);
        Assert.True(older.IsDisposed);
        Assert.False(latest.IsDisposed);
        Assert.False(sut.TryTake(out _));
    }

    [Fact]
    public void Publish_AfterTheScheduledRenderTookItsFrame_AsksForANewRender()
    {
        var sut = new LatestFrameSlot<Frame>();
        sut.Publish(new Frame(1));
        sut.TryTake(out _);

        Assert.True(sut.Publish(new Frame(2)));
    }

    [Fact]
    public void HasPendingFrame_IsTrueUntilTheUiTakesTheFrame()
    {
        var sut = new LatestFrameSlot<Frame>();
        Assert.False(sut.HasPendingFrame);

        sut.Publish(new Frame(1));
        Assert.True(sut.HasPendingFrame);

        sut.TryTake(out _);
        Assert.False(sut.HasPendingFrame);
    }

    [Fact]
    public void Clear_DisposesThePendingFrame()
    {
        var sut = new LatestFrameSlot<Frame>();
        var pending = new Frame(1);
        sut.Publish(pending);

        sut.Clear();

        Assert.True(pending.IsDisposed);
        Assert.False(sut.TryTake(out _));
    }

    private sealed class Frame(int number) : IDisposable
    {
        public int Number { get; } = number;

        public bool IsDisposed { get; private set; }

        public void Dispose()
            => IsDisposed = true;
    }
}