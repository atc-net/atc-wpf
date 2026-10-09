namespace Atc.Wpf.Theming.Controls;

/// <summary>
/// Defines a draggable thumb element.
/// </summary>
public interface IAtcThumb : IInputElement
{
    /// <summary>
    /// Occurs when a drag operation starts.
    /// </summary>
    event DragStartedEventHandler DragStarted;

    /// <summary>
    /// Occurs one or more times as the mouse changes position while dragging.
    /// </summary>
    event DragDeltaEventHandler DragDelta;

    /// <summary>
    /// Occurs when a drag operation completes.
    /// </summary>
    event DragCompletedEventHandler DragCompleted;

    /// <summary>
    /// Occurs when a mouse button is clicked two or more times.
    /// </summary>
    event MouseButtonEventHandler MouseDoubleClick;
}