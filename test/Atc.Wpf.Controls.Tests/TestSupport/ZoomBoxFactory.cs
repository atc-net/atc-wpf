namespace Atc.Wpf.Controls.Tests.TestSupport;

/// <summary>
/// Creates a <see cref="ZoomBox"/> with its default template (from ZoomBox.xaml) and lays it out,
/// without a window or theme resources.
/// </summary>
internal static class ZoomBoxFactory
{
    public static ZoomBox CreateTemplated(
        FrameworkElement content,
        double width,
        double height)
    {
        RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);

        var zoomStyles = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Atc.Wpf.Controls;component/Zoom/ZoomBox.xaml", UriKind.Absolute),
        };

        var zoomBox = new ZoomBox
        {
            Style = (Style)zoomStyles["AtcApps.Styles.ZoomBox"],
            Content = content,
        };

        zoomBox.Measure(new Size(width, height));
        zoomBox.Arrange(new Rect(0, 0, width, height));
        zoomBox.UpdateLayout();

        return zoomBox;
    }
}