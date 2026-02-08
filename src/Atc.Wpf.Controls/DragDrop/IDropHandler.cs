namespace Atc.Wpf.Controls.DragDrop;

/// <summary>
/// Defines how a drop target handles drag-over and drop operations.
/// Implement this interface in your ViewModel to customize drop behavior.
/// </summary>
public interface IDropHandler
{
    /// <summary>
    /// Called continuously while the mouse is over the drop target.
    /// Set <see cref="DropInfo.Effects"/> to <see cref="DragDropEffects.None"/>
    /// to deny the drop, or to another value (e.g. <see cref="DragDropEffects.Move"/>)
    /// to allow it.
    /// </summary>
    /// <param name="dropInfo">Information about the current drop operation.</param>
    void DragOver(DropInfo dropInfo);

    /// <summary>
    /// Called when the user releases the mouse button over the drop target.
    /// Execute the actual drop logic here (move, copy, etc.).
    /// </summary>
    /// <param name="dropInfo">Information about the drop operation.</param>
    void Drop(DropInfo dropInfo);
}