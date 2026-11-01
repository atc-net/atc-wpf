namespace Atc.Wpf.Controls.Tests.TestSupport;

/// <summary>
/// Raises (or creates) keyboard events for an element without a window.
/// </summary>
internal static class KeyPress
{
    public static void Send(
        UIElement element,
        Key key)
        => element.RaiseEvent(CreateArgs(Keyboard.KeyDownEvent, key));

    public static KeyEventArgs CreateArgs(
        RoutedEvent routedEvent,
        Key key)
        => new(Keyboard.PrimaryDevice, new TestPresentationSource(), 0, key)
        {
            RoutedEvent = routedEvent,
        };

    private sealed class TestPresentationSource : PresentationSource
    {
        public override Visual? RootVisual { get; set; }

        public override bool IsDisposed => false;

        protected override CompositionTarget? GetCompositionTargetCore()
            => null;
    }
}