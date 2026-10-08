namespace Atc.Wpf.Controls.Tests.TestSupport;

/// <summary>
/// Raises a KeyDown event on an element without a window.
/// </summary>
internal static class KeyPress
{
    public static void Send(
        UIElement element,
        Key key)
    {
        var args = new KeyEventArgs(Keyboard.PrimaryDevice, new TestPresentationSource(), 0, key)
        {
            RoutedEvent = Keyboard.KeyDownEvent,
        };

        element.RaiseEvent(args);
    }

    private sealed class TestPresentationSource : PresentationSource
    {
        public override Visual? RootVisual { get; set; }

        public override bool IsDisposed => false;

        protected override CompositionTarget? GetCompositionTargetCore()
            => null;
    }
}