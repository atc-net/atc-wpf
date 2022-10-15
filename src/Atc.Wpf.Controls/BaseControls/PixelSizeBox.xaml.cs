namespace Atc.Wpf.Controls.BaseControls;

/// <summary>
/// Interaction logic for PixelSizeBox.
/// </summary>
public partial class PixelSizeBox
{
    public static readonly DependencyProperty HideUpDownButtonsProperty = DependencyProperty.Register(
        nameof(HideUpDownButtons),
        typeof(bool),
        typeof(PixelSizeBox),
        new PropertyMetadata(default(bool)));

    public bool HideUpDownButtons
    {
        get => (bool)GetValue(HideUpDownButtonsProperty);
        set => SetValue(HideUpDownButtonsProperty, value);
    }

    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
        nameof(Maximum),
        typeof(int),
        typeof(PixelSizeBox),
        new FrameworkPropertyMetadata(
            int.MaxValue,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

    public int Maximum
    {
        get => (int)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public static readonly DependencyProperty ValueWidthProperty = DependencyProperty.Register(
        nameof(ValueWidth),
        typeof(int),
        typeof(PixelSizeBox),
        new FrameworkPropertyMetadata(
            default(int),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            propertyChangedCallback: null,
            coerceValueCallback: null,
            isAnimationProhibited: true,
            UpdateSourceTrigger.LostFocus));

    public int ValueWidth
    {
        get => (int)GetValue(ValueWidthProperty);
        set => SetValue(ValueWidthProperty, value);
    }

    public static readonly DependencyProperty ValueHeightProperty = DependencyProperty.Register(
        nameof(ValueHeight),
        typeof(int),
        typeof(PixelSizeBox),
        new FrameworkPropertyMetadata(
            default(int),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            propertyChangedCallback: null,
            coerceValueCallback: null,
            isAnimationProhibited: true,
            UpdateSourceTrigger.LostFocus));

    public int ValueHeight
    {
        get => (int)GetValue(ValueHeightProperty);
        set => SetValue(ValueHeightProperty, value);
    }

    public PixelSizeBox()
    {
        InitializeComponent();
    }
}