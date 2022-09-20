namespace Atc.Wpf.Sample.Samples.Extensions;

/// <summary>
/// Interaction logic for BitmapSourceToResizedBitmapImageView.
/// </summary>
public partial class BitmapSourceToResizedBitmapImageView
{
    public BitmapSourceToResizedBitmapImageView()
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

        var newWidth = (int)OrgImage.Width / 2;
        var bitmapSource = bitmap.ToResizedBitmapImage(newWidth);

        TestImage.Source = bitmapSource;
    }
}