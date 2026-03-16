namespace Atc.Wpf.Sample.SamplesWpfControls.Zoom;

public partial class ZoomBoxView
{
    public ZoomBoxView()
    {
        InitializeComponent();

        var vm = new ZoomBoxDemoViewModel();
        vm.SetZoomScrollViewer(ZoomScrollViewer);
        DataContext = vm;
    }
}