namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelPixelSizeBox.
/// </summary>
public partial class LabelPixelSizeBox : ILabelPixelSizeBox
{
    public static readonly DependencyProperty ValueWidthProperty = DependencyProperty.Register(
        nameof(ValueWidth),
        typeof(int),
        typeof(LabelPixelSizeBox),
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
        typeof(LabelPixelSizeBox),
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

    public LabelPixelSizeBox()
    {
        InitializeComponent();
    }
}