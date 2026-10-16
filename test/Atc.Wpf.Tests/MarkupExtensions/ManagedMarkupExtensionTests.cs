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
        Assert.Equal("After", other.Dispatcher.Invoke(() => other.Element.Text));
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