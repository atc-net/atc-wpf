namespace Atc.Wpf.Theming.Controls;

public interface IAtcThumb : IInputElement
{
    event DragStartedEventHandler DragStarted;

    event DragDeltaEventHandler DragDelta;

    event DragCompletedEventHandler DragCompleted;

    event MouseButtonEventHandler MouseDoubleClick;
}