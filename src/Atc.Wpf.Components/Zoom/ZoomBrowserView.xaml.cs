namespace Atc.Wpf.Components.Zoom;

/// <summary>
/// A reusable component that bundles a zoom toolbar, minimap, and status bar
/// around a <see cref="ZoomScrollViewer"/>.
/// </summary>
public partial class ZoomBrowserView
{
    [DependencyProperty(DefaultValue = true)]
    private bool showToolbar;

    [DependencyProperty(DefaultValue = true)]
    private bool showMiniMap;

    [DependencyProperty(DefaultValue = true)]
    private bool showStatusBar;

    [DependencyProperty]
    private ZoomScrollViewer? zoomContent;

    public ZoomBrowserView()
    {
        InitializeComponent();
    }
}