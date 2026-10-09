namespace Atc.Wpf.Theming.Primitives;

/// <summary>
/// Defines a draggable thumb element.
/// </summary>
public interface INiceThumb : IInputElement
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