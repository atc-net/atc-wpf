namespace Atc.Wpf.Theming.Tests.Helpers;

// ModeChanged is static; a view that listens to it must still be collectable when it is gone.
[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1215:\"GC.Collect\" should not be called", Justification = "Verifies that the static event does not root its subscribers.")]
public sealed class WindowsThemeSyncEventTests
{
    [Fact]
    public void ModeChanged_does_not_keep_a_subscriber_alive()
    {
        var subscriber = SubscribeAndForget();

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        Assert.False(subscriber.IsAlive);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference SubscribeAndForget()
    {
        var subscriber = new Subscriber();
        WindowsThemeSync.ModeChanged += subscriber.OnModeChanged;
        return new WeakReference(subscriber);
    }

    private sealed class Subscriber
    {
        public int Calls { get; private set; }

        public void OnModeChanged(
            object? sender,
            EventArgs e)
            => Calls++;
    }
}