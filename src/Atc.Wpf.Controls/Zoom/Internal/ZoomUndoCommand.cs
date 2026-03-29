namespace Atc.Wpf.Controls.Zoom.Internal;

/// <summary>
/// An <see cref="IMergeableUndoCommand"/> that captures ZoomBox viewport state
/// so that zoom/pan changes can be recorded through an <see cref="IUndoRedoService"/>.
/// </summary>
internal sealed class ZoomUndoCommand : IMergeableUndoCommand
{
    private readonly ZoomBox zoomBox;
    private readonly Rect fromViewport;
    private readonly double fromZoom;
    private Rect toViewport;
    private double toZoom;

    public ZoomUndoCommand(
        ZoomBox zoomBox,
        Rect fromViewport,
        double fromZoom,
        Rect toViewport,
        double toZoom)
    {
        this.zoomBox = zoomBox;
        this.fromViewport = fromViewport;
        this.fromZoom = fromZoom;
        this.toViewport = toViewport;
        this.toZoom = toZoom;
    }

    /// <inheritdoc />
    public string Description => $"Zoom {toZoom:P0}";

    /// <inheritdoc />
    public int MergeId => 1;

    /// <inheritdoc />
    public bool TryMergeWith(IUndoCommand other)
    {
        if (other is not ZoomUndoCommand z)
        {
            return false;
        }

        toViewport = z.toViewport;
        toZoom = z.toZoom;
        return true;
    }

    /// <inheritdoc />
    public void Execute()
        => zoomBox.AnimatedZoomTo(toZoom, toViewport);

    /// <inheritdoc />
    public void UnExecute()
        => zoomBox.AnimatedZoomTo(fromZoom, fromViewport);
}