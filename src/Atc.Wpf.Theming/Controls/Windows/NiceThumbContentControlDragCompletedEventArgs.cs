namespace Atc.Wpf.Theming.Controls.Windows;

public sealed class NiceThumbContentControlDragCompletedEventArgs : DragCompletedEventArgs
{
    public NiceThumbContentControlDragCompletedEventArgs(
        double horizontalOffset,
        double verticalOffset,
        bool canceled)
        : base(horizontalOffset, verticalOffset, canceled)
    {
        RoutedEvent = NiceThumbContentControl.DragCompletedEvent;
    }
}