namespace Atc.Wpf.DragDrop;

/// <summary>
/// Optional interface to customize drag initiation behavior.
/// Implement this interface in your ViewModel to control what happens
/// when a drag operation starts.
/// </summary>
public interface IDragHandler
{
    /// <summary>
    /// Called when a drag operation is about to start.
    /// Set <see cref="DragInfo.Effects"/> to <see cref="DragDropEffects.None"/>
    /// to cancel the drag.
    /// </summary>
    /// <param name="dragInfo">Information about the drag operation being initiated.</param>
    void StartDrag(DragInfo dragInfo);
}