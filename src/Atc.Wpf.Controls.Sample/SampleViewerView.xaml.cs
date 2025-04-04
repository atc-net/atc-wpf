namespace Atc.Wpf.Controls.Sample;

public partial class SampleViewerView
{
    public static readonly DependencyProperty HeaderForegroundProperty = DependencyProperty.Register(
        nameof(HeaderForeground),
        typeof(SolidColorBrush),
        typeof(SampleViewerView),
        new PropertyMetadata(Brushes.Chocolate));

    public SolidColorBrush HeaderForeground
    {
        get => (SolidColorBrush)GetValue(HeaderForegroundProperty);
        set => SetValue(HeaderForegroundProperty, value);
    }

    public SampleViewerView()
    {
        InitializeComponent();

        DataContext = new SampleViewerViewModel();
    }
}