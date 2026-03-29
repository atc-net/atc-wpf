namespace Atc.Wpf.Controls.Zoom;

/// <summary>
/// Links two <see cref="ZoomBox"/> viewports so that changes to the primary
/// are propagated to the secondary based on the specified <see cref="ViewportLinkMode"/>.
/// Dispose to unlink.
/// </summary>
public sealed class ZoomViewportLink : IDisposable
{
    private readonly ZoomBox primary;
    private readonly ZoomBox secondary;
    private bool isSyncing;
    private bool disposed;

    private ZoomViewportLink(
        ZoomBox primary,
        ZoomBox secondary,
        ViewportLinkMode mode)
    {
        this.primary = primary;
        this.secondary = secondary;
        Mode = mode;

        primary.ContentOffsetXChanged += OnPrimaryChanged;
        primary.ContentOffsetYChanged += OnPrimaryChanged;

        if (mode == ViewportLinkMode.Mirror)
        {
            primary.ContentZoomChanged += OnPrimaryChanged;
        }

        SyncNow();
    }

    /// <summary>
    /// Gets the link mode.
    /// </summary>
    public ViewportLinkMode Mode { get; }

    /// <summary>
    /// Creates a link between two ZoomBox instances.
    /// </summary>
    /// <param name="primary">The source viewport (drives changes).</param>
    /// <param name="secondary">The target viewport (receives changes).</param>
    /// <param name="mode">The synchronization mode.</param>
    /// <returns>A disposable link. Dispose to unlink.</returns>
    public static ZoomViewportLink Create(
        ZoomBox primary,
        ZoomBox secondary,
        ViewportLinkMode mode)
    {
        ArgumentNullException.ThrowIfNull(primary);
        ArgumentNullException.ThrowIfNull(secondary);

        if (ReferenceEquals(primary, secondary))
        {
            throw new ArgumentException(
                "Cannot link a ZoomBox to itself.",
                nameof(secondary));
        }

        return new ZoomViewportLink(primary, secondary, mode);
    }

    /// <summary>
    /// Forces an immediate synchronization from primary to secondary.
    /// </summary>
    public void SyncNow()
    {
        if (isSyncing || disposed)
        {
            return;
        }

        isSyncing = true;
        try
        {
            secondary.ContentOffsetX = primary.ContentOffsetX;
            secondary.ContentOffsetY = primary.ContentOffsetY;

            if (Mode == ViewportLinkMode.Mirror)
            {
                secondary.ZoomTo(primary.ViewportZoom);
            }
        }
        finally
        {
            isSyncing = false;
        }
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        primary.ContentOffsetXChanged -= OnPrimaryChanged;
        primary.ContentOffsetYChanged -= OnPrimaryChanged;
        primary.ContentZoomChanged -= OnPrimaryChanged;
    }

    private void OnPrimaryChanged(
        object? sender,
        EventArgs e)
        => SyncNow();
}