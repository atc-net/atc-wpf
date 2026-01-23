namespace Atc.Wpf.Sample.SamplesWpfControls.Layouts;

public partial class ResponsivePanelView
{
    public ResponsivePanelView()
    {
        InitializeComponent();
        SizeChanged += OnSizeChanged;
        Loaded += OnLoaded;
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        UpdateBreakpointDisplay();
    }

    private void OnSizeChanged(
        object sender,
        SizeChangedEventArgs e)
    {
        UpdateBreakpointDisplay();
    }

    private void UpdateBreakpointDisplay()
    {
        var width = ActualWidth;
        var breakpoint = ResponsivePanel.GetBreakpoint(width);

        var breakpointName = breakpoint switch
        {
            ResponsiveBreakpoint.Xs => "XS (Extra Small)",
            ResponsiveBreakpoint.Sm => "SM (Small)",
            ResponsiveBreakpoint.Md => "MD (Medium)",
            ResponsiveBreakpoint.Lg => "LG (Large)",
            ResponsiveBreakpoint.Xl => "XL (Extra Large)",
            _ => "Unknown",
        };

        var thresholds = breakpoint switch
        {
            ResponsiveBreakpoint.Xs => "< 576px",
            ResponsiveBreakpoint.Sm => "576px - 767px",
            ResponsiveBreakpoint.Md => "768px - 991px",
            ResponsiveBreakpoint.Lg => "992px - 1199px",
            ResponsiveBreakpoint.Xl => ">= 1200px",
            _ => string.Empty,
        };

        TxtBreakpoint.Text = $"Current: {breakpointName} ({thresholds}) | Width: {width:F0}px";
    }
}