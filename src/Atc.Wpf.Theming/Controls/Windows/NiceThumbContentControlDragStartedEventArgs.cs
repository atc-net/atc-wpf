namespace Atc.Wpf.Theming.Controls.Windows;

public class NiceThumbContentControlDragStartedEventArgs : DragStartedEventArgs
{
    public NiceThumbContentControlDragStartedEventArgs(
        double horizontalOffset,
        double verticalOffset)
        : base(horizontalOffset, verticalOffset)
    {
        RoutedEvent = NiceThumbContentControl.DragStartedEvent;
    }
}