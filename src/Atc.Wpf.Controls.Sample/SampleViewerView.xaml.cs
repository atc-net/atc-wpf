namespace Atc.Wpf.Controls.Sample;

public partial class SampleViewerView
{
    [DependencyProperty(DefaultValue = "Brushes.Chocolate")]
    private SolidColorBrush headerForeground;

    public SampleViewerView()
    {
        InitializeComponent();

        DataContext = new SampleViewerViewModel();
    }
}