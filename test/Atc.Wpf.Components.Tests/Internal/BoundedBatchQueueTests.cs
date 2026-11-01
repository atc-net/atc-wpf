namespace Atc.Wpf.Components.Tests.Internal;

public sealed class BoundedBatchQueueTests
{
    [Fact]
    public void EnqueueRange_BeyondTheCapacity_KeepsTheNewestAndCountsTheDropped()
    {
        var sut = new BoundedBatchQueue<int>(capacity: 1_000);

        sut.EnqueueRange(Enumerable.Range(0, 200_000));

        Assert.Equal(1_000, sut.Count);
        Assert.True(sut.TryTakeAll(out var batch));
        Assert.Equal(199_000, batch.Items[0]);
        Assert.Equal(199_999, batch.Items[^1]);
        Assert.Equal(199_000, batch.DroppedCount);
        Assert.Equal(0, sut.Count);
    }

    [Fact]
    public void Enqueue_NoCapacity_KeepsEverything()
    {
        var sut = new BoundedBatchQueue<int>(capacity: 0);

        for (var i = 0; i < 5_000; i++)
        {
            sut.Enqueue(i);
        }

        Assert.Equal(5_000, sut.Count);
    }

    [Fact]
    public void Capacity_Lowered_DropsTheOldest()
    {
        var sut = new BoundedBatchQueue<int>(capacity: 100);
        sut.EnqueueRange(Enumerable.Range(0, 100));

        sut.Capacity = 10;

        Assert.True(sut.TryTakeAll(out var batch));
        Assert.Equal(Enumerable.Range(90, 10), batch.Items);
        Assert.Equal(90, batch.DroppedCount);
    }

    [Fact]
    public void Clear_AfterABatchWasTaken_MakesThatBatchStale()
    {
        var sut = new BoundedBatchQueue<string>(capacity: 100);
        sut.Enqueue("before");
        Assert.True(sut.TryTakeAll(out var inFlight));
        sut.Enqueue("also before");

        var generation = sut.Clear();
        sut.Enqueue("after");

        Assert.NotEqual(generation, inFlight.Generation);
        Assert.True(sut.TryTakeAll(out var next));
        Assert.Equal(["after"], next.Items);
        Assert.Equal(generation, next.Generation);
    }

    [Fact]
    public async Task WaitToReadAsync_ItemsLeftAfterAnEarlierWake_ReturnsAtOnce()
    {
        var sut = new BoundedBatchQueue<int>(capacity: 10);
        sut.Enqueue(1);
        Assert.True(await sut.WaitToReadAsync(TestContext.Current.CancellationToken));

        // The consumer did not take the item (paused), so the next wait must not block until another write.
        var second = sut.WaitToReadAsync(TestContext.Current.CancellationToken).AsTask();

        Assert.True(second.IsCompleted);
        Assert.True(await second);
    }

    [Fact]
    public async Task WaitToReadAsync_Completed_ReturnsFalse()
    {
        var sut = new BoundedBatchQueue<int>(capacity: 10);

        sut.Complete();

        Assert.False(await sut.WaitToReadAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task WaitToReadAsync_ItemWrittenFromAnotherThread_Wakes()
    {
        var sut = new BoundedBatchQueue<int>(capacity: 10);
        var wait = sut.WaitToReadAsync(TestContext.Current.CancellationToken).AsTask();

        await Task.Run(() => sut.Enqueue(1), TestContext.Current.CancellationToken);

        Assert.True(await wait.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken));
    }
}