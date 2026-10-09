namespace Atc.Wpf.Forms.Tests.TestSupport;

/// <summary>
/// Raises the focus events an editor raises when the user moves focus in and out of it.
/// </summary>
internal static class FocusEvents
{
    public static void GotFocus(UIElement editor)
        => editor.RaiseEvent(new RoutedEventArgs(UIElement.GotFocusEvent, editor));

    public static void LostFocus(UIElement editor)
        => editor.RaiseEvent(new RoutedEventArgs(UIElement.LostFocusEvent, editor));
}