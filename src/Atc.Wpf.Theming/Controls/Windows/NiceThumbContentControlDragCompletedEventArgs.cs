namespace Atc.Wpf.Theming.Controls.Windows;

/// <summary>
/// Provides data for the <see cref="NiceThumbContentControl.DragCompletedEvent"/> routed event.
/// </summary>
public sealed class NiceThumbContentControlDragCompletedEventArgs : DragCompletedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NiceThumbContentControlDragCompletedEventArgs"/> class.
    /// </summary>
    /// <param name="horizontalOffset">The horizontal change in position since the drag started.</param>
    /// <param name="verticalOffset">The vertical change in position since the drag started.</param>
    /// <param name="canceled">A value indicating whether the drag was canceled.</param>
    public NiceThumbContentControlDragCompletedEventArgs(
        double horizontalOffset,
        double verticalOffset,
        bool canceled)
        : base(horizontalOffset, verticalOffset, canceled)
    {
        RoutedEvent = NiceThumbContentControl.DragCompletedEvent;
    }
}