namespace Atc.Wpf.Sample.SamplesWpf.Extensions;

/// <summary>
/// Interaction logic for BitmapSourceInvertColorsView.
/// </summary>
public partial class BitmapSourceInvertColorsView
{
    public BitmapSourceInvertColorsView()
    {
        InitializeComponent();

        Loaded += OnLoaded;
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        var bitmap = new BitmapImage(
            new Uri(@"pack://application:,,,/Atc.Wpf.Sample;component/Assets/road.jpg", UriKind.Absolute));

        var bitmapSource = bitmap.InvertColors();

        TestImage.Source = bitmapSource;
    }
}