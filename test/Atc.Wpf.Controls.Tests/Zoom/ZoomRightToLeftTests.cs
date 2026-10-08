namespace Atc.Wpf.Controls.Tests.Zoom;

public sealed class ZoomRightToLeftTests : IDisposable
{
    private const string ViewXaml = """
        <Grid
            xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
            xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
            xmlns:zoom="clr-namespace:Atc.Wpf.Controls.Zoom;assembly=Atc.Wpf.Controls">
            <Grid.ColumnDefinitions>
                <ColumnDefinition />
                <ColumnDefinition Width="200" />
            </Grid.ColumnDefinitions>
            <zoom:ZoomScrollViewer x:Name="ZoomScrollViewer">
                <Canvas x:Name="ZoomContent" Width="800" Height="600" Background="Black">
                    <Rectangle Width="400" Height="600" Fill="Red" />
                    <TextBlock Text="Zoom" />
                </Canvas>
            </zoom:ZoomScrollViewer>
            <zoom:ZoomMiniMap
                x:Name="ZoomMiniMap"
                Grid.Column="1"
                Height="100"
                DataContext="{Binding ElementName=ZoomScrollViewer}" />
        </Grid>
        """;

    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void ZoomContent_WithMiniMap_LoadedIntoRightToLeftHost_IsNotMirroredAgainstItsParent()
    {
        var host = CreateHost();
        host.FlowDirection = FlowDirection.RightToLeft;
        using var source = CreateHiddenSource(host);

        var (view, zoomContent) = CreateView();
        host.Content = view;
        Pump();

        Assert.False(IsMirroredAgainstParent(zoomContent));
    }

    [StaFact]
    public void ZoomContent_WithMiniMap_HostSwitchedToRightToLeftAfterLoad_IsNotMirroredAgainstItsParent()
    {
        var host = CreateHost();
        using var source = CreateHiddenSource(host);

        var (view, zoomContent) = CreateView();
        host.Content = view;
        Pump();
        host.FlowDirection = FlowDirection.RightToLeft;
        Pump();

        Assert.False(IsMirroredAgainstParent(zoomContent));
    }

    [StaFact]
    public void MiniMap_ContentAttachedAfterTheMiniMapTemplate_StillShowsTheThumbnail()
    {
        var host = CreateHost();
        host.FlowDirection = FlowDirection.RightToLeft;
        using var source = CreateHiddenSource(host);

        var (view, zoomContent) = CreateView();
        host.Content = view;
        Pump();

        var miniMap = (ZoomMiniMap)view.FindName("ZoomMiniMap");
        var thumbnailCanvas = (Canvas)miniMap.Template.FindName("PART_Content", miniMap);
        var brush = Assert.IsType<VisualBrush>(thumbnailCanvas.Background);
        Assert.Same(zoomContent, brush.Visual);
    }

    [StaTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void MiniMap_RightToLeftContent_ThumbnailIsMirroredLikeTheZoomView(
        bool rightToLeftBeforeLoad)
    {
        var host = CreateHost();
        if (rightToLeftBeforeLoad)
        {
            host.FlowDirection = FlowDirection.RightToLeft;
        }

        using var source = CreateHiddenSource(host);

        var (view, _) = CreateView();
        host.Content = view;
        Pump();
        host.FlowDirection = FlowDirection.RightToLeft;
        Pump();

        // The content's red half is on its left; mirrored on screen, it is on the right.
        var miniMap = (ZoomMiniMap)view.FindName("ZoomMiniMap");
        var thumbnailCanvas = (Canvas)miniMap.Template.FindName("PART_Content", miniMap);
        var root = (FrameworkElement)source.RootVisual;
        var thumbnail = thumbnailCanvas
            .TransformToAncestor(root)
            .TransformBounds(new Rect(thumbnailCanvas.RenderSize));
        var pixels = Render(root);

        Assert.False(IsRed(pixels, thumbnail.Left + (thumbnail.Width * 0.25), thumbnail.Top + (thumbnail.Height / 2)));
        Assert.True(IsRed(pixels, thumbnail.Left + (thumbnail.Width * 0.75), thumbnail.Top + (thumbnail.Height / 2)));
    }

    private static ContentControl CreateHost()
    {
        RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);

        var zoomStyles = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Atc.Wpf.Controls;component/Zoom/ZoomBox.xaml", UriKind.Absolute),
        };

        var resources = new ResourceDictionary();
        resources.MergedDictionaries.Add(zoomStyles);
        resources.Add(typeof(ZoomBox), new Style(typeof(ZoomBox), (Style)zoomStyles["AtcApps.Styles.ZoomBox"]));
        resources.Add(typeof(ZoomScrollViewer), new Style(typeof(ZoomScrollViewer), (Style)zoomStyles["AtcApps.Styles.ZoomScrollViewer"]));
        resources.Add(typeof(ZoomMiniMap), new Style(typeof(ZoomMiniMap), (Style)zoomStyles["AtcApps.Styles.ZoomMiniMap"]));

        return new ContentControl { Resources = resources };
    }

    // A window that is never shown, so layout and rendering run as in an application.
    private static HwndSource CreateHiddenSource(UIElement host)
    {
        var root = new Grid { Width = 600, Height = 400 };
        root.Children.Add(host);

        return new HwndSource(new HwndSourceParameters("ZoomRightToLeftTests") { WindowStyle = 0 })
        {
            SizeToContent = SizeToContent.WidthAndHeight,
            RootVisual = root,
        };
    }

    private static (Grid View, Canvas ZoomContent) CreateView()
    {
        var view = (Grid)XamlReader.Parse(ViewXaml);
        return (view, (Canvas)view.FindName("ZoomContent"));
    }

    // WPF mirrors an element whose FlowDirection differs from its parent's; under a parent with
    // the same FlowDirection a mirror (negative horizontal scale) is wrong.
    private static bool IsMirroredAgainstParent(FrameworkElement element)
        => VisualTreeHelper.GetTransform(element) is { } transform &&
           transform.Value.M11 < 0;

    private static RenderTargetBitmap Render(FrameworkElement element)
    {
        var bitmap = new RenderTargetBitmap(
            (int)element.ActualWidth,
            (int)element.ActualHeight,
            96,
            96,
            PixelFormats.Pbgra32);
        bitmap.Render(element);
        return bitmap;
    }

    private static bool IsRed(
        BitmapSource bitmap,
        double x,
        double y)
    {
        var pixel = new byte[4];
        bitmap.CopyPixels(new Int32Rect((int)x, (int)y, 1, 1), pixel, 4, 0);

        // Pbgra32: blue, green, red, alpha.
        return pixel[2] > 128 && pixel[1] < 100 && pixel[0] < 100;
    }

    private static void Pump()
    {
        for (var i = 0; i < 5; i++)
        {
            Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);
        }
    }
}