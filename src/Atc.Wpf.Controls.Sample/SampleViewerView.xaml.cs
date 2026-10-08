namespace Atc.Wpf.Controls.Sample;

public partial class SampleViewerView
{
    [DependencyProperty(DefaultValue = "Brushes.Chocolate")]
    private SolidColorBrush headerForeground;

    /// <summary>
    /// Shows the sample content in a right-to-left layout, to check how controls mirror.
    /// </summary>
    [DependencyProperty]
    private bool isSampleRightToLeft;

    public SampleViewerView()
    {
        InitializeComponent();

        DataContext = new SampleViewerViewModel();
    }
}