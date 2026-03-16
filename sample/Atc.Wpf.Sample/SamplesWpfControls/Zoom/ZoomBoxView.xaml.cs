namespace Atc.Wpf.Sample.SamplesWpfControls.Zoom;

public partial class ZoomBoxView
{
    public ZoomBoxView()
    {
        InitializeComponent();

        var vm = new ZoomBoxDemoViewModel();
        vm.SetZoomScrollViewer(ZoomScrollViewer);
        DataContext = vm;

        Loaded += (_, _) =>
        {
            if (ZoomScrollViewer.Content is FrameworkElement content)
            {
                ZoomMiniMap.VisualElement = content;
            }
        };

        PreviewKeyDown += (_, e) =>
        {
            ZoomScrollViewer.ZoomContent?.TryHandleKeyDown(e);
        };
    }
}