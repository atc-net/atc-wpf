namespace Atc.Wpf.Theming.Controls.Windows;

public sealed class NiceThumbContentControlDragStartedEventArgs : DragStartedEventArgs
{
    public NiceThumbContentControlDragStartedEventArgs(
        double horizontalOffset,
        double verticalOffset)
        : base(horizontalOffset, verticalOffset)
    {
        RoutedEvent = NiceThumbContentControl.DragStartedEvent;
    }
}