namespace Atc.Wpf.Tests.MarkupExtensions;

public sealed class ManagedMarkupExtensionTests
{
    private const string TextBlockXaml = """
        <TextBlock
            xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
            xmlns:test="clr-namespace:Atc.Wpf.Tests.XUnitTestTypes;assembly=Atc.Wpf.Tests"
            Text="{test:TestTextExtension}" />
        """;

    // Each [StaFact] runs on its own thread, so targets another test left in the shared manager belong to a
    // different (finished) thread and cannot be updated from this one.
    public ManagedMarkupExtensionTests()
        => TestTextExtension.Manager.ActiveExtensions.Clear();

    // Flyout.xaml sets ToolTip through a Resx extension in a style setter; a culture change then updated the
    // sealed setter and threw "After a 'SetterBase' is in use (sealed), it cannot be modified".
    [StaFact]
    public void UpdateAllTargets_ExtensionInASealedStyleSetter_DoesNotThrow()
    {
        var style = (Style)XamlReader.Parse(
            """
            <Style
                xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                xmlns:test="clr-namespace:Atc.Wpf.Tests.XUnitTestTypes;assembly=Atc.Wpf.Tests"
                TargetType="ToolTip">
                <Setter Property="Content" Value="{test:TestTextExtension}" />
            </Style>
            """);
        style.Seal();

        var exception = Record.Exception(TestTextExtension.Manager.UpdateAllTargets);

        Assert.Null(exception);
    }

    [StaFact]
    public void UpdateAllTargets_ExtensionOnAnElement_UpdatesTheElement()
    {
        TestTextExtension.Text = "Before";
        var textBlock = (TextBlock)XamlReader.Parse(TextBlockXaml);

        TestTextExtension.Text = "After";
        TestTextExtension.Manager.UpdateAllTargets();

        Assert.Equal("After", textBlock.Text);
    }

    // A culture change is process-wide and can be raised on any UI thread, while the targets belong to the
    // windows (threads) that created them. Updating them from the wrong thread used to throw.
    [StaFact]
    public void UpdateAllTargets_ExtensionOnAnotherUiThreadsElement_UpdatesItOnThatThread()
    {
        TestTextExtension.Text = "Before";
        using var other = new UiThread(() => (TextBlock)XamlReader.Parse(TextBlockXaml));

        TestTextExtension.Text = "After";
        var exception = Record.Exception(TestTextExtension.Manager.UpdateAllTargets);

        Assert.Null(exception);

        // Read at a lower priority than the queued update, which the extension posts at Normal priority.
        Assert.Equal("After", other.Dispatcher.Invoke(() => other.Element.Text, DispatcherPriority.Background));
    }

    [StaFact]
    public void UpdateAllTargets_ExtensionOnAShutDownUiThread_IsSkippedWithoutThrowing()
    {
        TestTextExtension.Text = "Before";
        var other = new UiThread(() => (TextBlock)XamlReader.Parse(TextBlockXaml));
        other.Dispose();

        TestTextExtension.Text = "After";
        var exception = Record.Exception(TestTextExtension.Manager.UpdateAllTargets);

        Assert.Null(exception);
    }

    // An extension shared by a template gains a target per instantiation; references to collected targets
    // used to stay in its list forever.
    [StaFact]
    public void UpdateTarget_TargetsCollected_DropsTheirReferences()
    {
        var extension = new TestTextExtension();
        ProvideForUnreferencedTargets(extension, count: 5);
        Assert.Equal(5, extension.TargetReferenceCount);

        CollectGarbage();
        extension.UpdateTarget();

        Assert.Equal(0, extension.TargetReferenceCount);
        Assert.False(extension.IsTargetAlive);
    }

    [StaFact]
    public void ProvideValue_ManyCollectedTargets_PrunesThemOnAdd()
    {
        var extension = new TestTextExtension();
        ProvideForUnreferencedTargets(extension, count: 100);

        CollectGarbage();

        // Pruning is amortized (it runs once the list doubles), so keep adding until it has run.
        var liveTargets = Enumerable.Range(0, 64).Select(_ => new TextBlock()).ToList();
        foreach (var liveTarget in liveTargets)
        {
            extension.ProvideValue(new TargetServiceProvider(liveTarget));
        }

        Assert.Equal(liveTargets.Count, extension.TargetReferenceCount);
        Assert.True(extension.IsTargetAlive);
        GC.KeepAlive(liveTargets);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ProvideForUnreferencedTargets(
        TestTextExtension extension,
        int count)
    {
        for (var i = 0; i < count; i++)
        {
            extension.ProvideValue(new TargetServiceProvider(new TextBlock()));
        }
    }

    [SuppressMessage("Major Code Smell", "S1215:\"GC.Collect\" should not be called", Justification = "Forcing a collection is how the dead targets are produced.")]
    private static void CollectGarbage()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    private sealed class TargetServiceProvider(object target) : IServiceProvider, IProvideValueTarget
    {
        public object TargetObject => target;

        public object TargetProperty => TextBlock.TextProperty;

        public object? GetService(Type serviceType)
            => serviceType == typeof(IProvideValueTarget) ? this : null;
    }

    /// <summary>Runs a WPF dispatcher on its own STA thread and creates the element under test on it.</summary>
    private sealed class UiThread : IDisposable
    {
        private readonly Thread thread;

        public UiThread(Func<TextBlock> createElement)
        {
            using var ready = new ManualResetEventSlim();
            thread = new Thread(() =>
            {
                Element = createElement();
                Dispatcher = Dispatcher.CurrentDispatcher;
                ready.Set();
                Dispatcher.Run();
            })
            {
                IsBackground = true,
            };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            ready.Wait(TimeSpan.FromSeconds(10));
        }

        public Dispatcher Dispatcher { get; private set; } = null!;

        public TextBlock Element { get; private set; } = null!;

        public void Dispose()
        {
            Dispatcher.InvokeShutdown();
            thread.Join(TimeSpan.FromSeconds(10));
        }
    }
}