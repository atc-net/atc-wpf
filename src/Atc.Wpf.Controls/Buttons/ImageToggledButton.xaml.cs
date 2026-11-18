namespace Atc.Wpf.Controls.Buttons;

public partial class ImageToggledButton
{
    /// <summary>Occurs when the toggled state of the button changes.</summary>
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
        GeneratePartialHooks = true)]
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

    /// <summary>Initializes a new instance of the <see cref="ImageToggledButton"/> class.</summary>
    public ImageToggledButton()
    {
        InitializeComponent();

        SetCurrentValue(ImageLocationProperty, Controls.ImageLocation.Left);
    }

    partial void OnIsToggledChanged(
        bool oldValue,
        bool newValue)
        => RaiseIsToggledChanged();

    private void RaiseIsToggledChanged()
        => IsToggledChanged?.Invoke(this, new RoutedEventArgs());
}