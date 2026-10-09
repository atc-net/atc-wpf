namespace Atc.Wpf.Theming.Controls.Windows;

/// <summary>
/// Provides data for the <see cref="NiceThumbContentControl.DragStartedEvent"/> routed event.
/// </summary>
public sealed class NiceThumbContentControlDragStartedEventArgs : DragStartedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NiceThumbContentControlDragStartedEventArgs"/> class.
    /// </summary>
    /// <param name="horizontalOffset">The horizontal position where the drag started.</param>
    /// <param name="verticalOffset">The vertical position where the drag started.</param>
    public NiceThumbContentControlDragStartedEventArgs(
        double horizontalOffset,
        double verticalOffset)
        : base(horizontalOffset, verticalOffset)
    {
        RoutedEvent = NiceThumbContentControl.DragStartedEvent;
    }
}