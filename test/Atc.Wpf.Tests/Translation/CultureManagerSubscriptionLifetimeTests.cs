namespace Atc.Wpf.Tests.Translation;

// A lambda that uses its owner (this) must live as long as the owner, and never keep it alive.
[Collection(nameof(UiCultureTestGroup))]
[SuppressMessage("Major Code Smell", "S1215:\"GC.Collect\" should not be called", Justification = "Verifies that subscriptions do not root their owners.")]
public sealed class CultureManagerSubscriptionLifetimeTests : IDisposable
{
    private readonly CultureInfo originalUiCulture = CultureManager.UiCulture;

    public void Dispose()
        => CultureManager.UiCulture = originalUiCulture;

    [Fact]
    public void A_lambda_that_captures_its_owner_does_not_keep_the_owner_alive()
    {
        var owner = CreateOwnerAndForget();

        Collect();

        Assert.False(owner.IsAlive);
    }

    [Fact]
    public void A_lambda_that_captures_its_owner_fires_while_the_owner_is_alive()
    {
        var owner = new Owner();

        Collect();
        CultureManager.UiCulture = ChangedCulture();

        Assert.Equal(1, owner.Hits);
        GC.KeepAlive(owner);
    }

    [Fact]
    public void A_lambda_without_an_owner_keeps_firing()
    {
        var counter = SubscribeCounterWithoutOwner();

        Collect();
        CultureManager.UiCulture = ChangedCulture();

        Assert.Equal(1, counter.Value);
    }

    private static void Collect()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    private CultureInfo ChangedCulture()
        => new(originalUiCulture.Name == "da-DK" ? "de-DE" : "da-DK");

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CreateOwnerAndForget()
        => new(new Owner());

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Counter SubscribeCounterWithoutOwner()
    {
        var counter = new Counter();
        CultureManager.UiCultureChanged += (_, _) => counter.Value++;
        return counter;
    }

    private sealed class Counter
    {
        public int Value { get; set; }
    }

    private sealed class Owner
    {
        public Owner()
        {
            // Capturing a local as well as this makes the compiler build a closure class.
            var step = 1;
            CultureManager.UiCultureChanged += (_, _) => Hits += step;
        }

        public int Hits { get; private set; }
    }
}