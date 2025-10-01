namespace Atc.Wpf.Controls.ButtonControls;

public partial class ImageToggledButton
{
    public event RoutedEventHandler? IsToggledChanged;

    [DependencyProperty]
    private ImageLocation? imageLocation;

    [DependencyProperty(DefaultValue = 16)]
    private int imageWidth;

    [DependencyProperty(DefaultValue = 16)]
    private int imageHeight;

    [DependencyProperty(DefaultValue = 10)]
    private double imageContentSpacing;

    [DependencyProperty(DefaultValue = 0)]
    private double imageBorderSpacing;

    [DependencyProperty(DefaultValue = false)]
    private bool isBusy;

    [DependencyProperty(
        DefaultValue = false,
        PropertyChangedCallback = nameof(OnIsToggledChanged))]
    private bool isToggled;

    [DependencyProperty]
    private object? onContent;

    [DependencyProperty]
    private object? offContent;

    [DependencyProperty]
    private ImageSource? onImageSource;

    [DependencyProperty]
    private ImageSource? offImageSource;

    [DependencyProperty(DefaultValue = "")]
    private string onSvgImageSource;

    [DependencyProperty(DefaultValue = "")]
    private string offSvgImageSource;

    [DependencyProperty]
    private Color? onSvgImageOverrideColor;

    [DependencyProperty]
    private Color? offSvgImageOverrideColor;

    [DependencyProperty]
    private ICommand? onCommand;

    [DependencyProperty]
    private ICommand? offCommand;

    public ImageToggledButton()
    {
        InitializeComponent();

        SetCurrentValue(ImageLocationProperty, Controls.ImageLocation.Left);
    }

    private static void OnIsToggledChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((ImageToggledButton)d).RaiseIsToggledChanged();

    private void RaiseIsToggledChanged()
        => IsToggledChanged?.Invoke(this, new RoutedEventArgs());
}