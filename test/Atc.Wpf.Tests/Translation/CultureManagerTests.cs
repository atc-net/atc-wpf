namespace Atc.Wpf.Tests.Translation;

public sealed class CultureManagerTests
{
    [Fact]
    [SuppressMessage("Major Code Smell", "S1215:\"GC.Collect\" should not be called", Justification = "Intentional — verifying that the weak-event implementation does not root subscribers.")]
    public void Subscriber_should_be_collectable_after_subscribing_to_UiCultureChanged()
    {
        // Arrange — subscribe and forget the only strong root in a non-inlined scope.
        var weakRef = SubscribeAndForget();

        // Act — force a full GC; the weak event should not root the subscriber.
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Assert — subscriber was collected; the static event no longer roots it.
        Assert.False(weakRef.IsAlive);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference SubscribeAndForget()
    {
        var subscriber = new TestSubscriber();
        CultureManager.UiCultureChanged += subscriber.OnCultureChanged;
        return new WeakReference(subscriber);
    }

    private sealed class TestSubscriber
    {
        public bool WasCalled { get; private set; }

        public void OnCultureChanged(
            object? sender,
            UiCultureEventArgs e)
            => WasCalled = true;
    }
}