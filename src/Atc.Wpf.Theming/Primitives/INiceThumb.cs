namespace Atc.Wpf.Theming.Primitives;

public interface INiceThumb : IInputElement
{
    event DragStartedEventHandler DragStarted;

    event DragDeltaEventHandler DragDelta;

    event DragCompletedEventHandler DragCompleted;

    event MouseButtonEventHandler MouseDoubleClick;
}