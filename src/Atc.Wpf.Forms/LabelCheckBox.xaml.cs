namespace Atc.Wpf.Forms;

public partial class LabelCheckBox : ILabelCheckBox
{
    [DependencyProperty(
        DefaultValue = false,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        PropertyChangedCallback = nameof(OnIsCheckedChanged))]
    private bool isChecked;

    /// <summary>
    /// Occurs when the <c>IsChecked</c> value changes.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<bool>>? IsCheckedChanged;

    static LabelCheckBox()
    {
        LabelPositionProperty.OverrideMetadata(
            typeof(LabelCheckBox),
            new FrameworkPropertyMetadata(
                LabelPosition.Left,
                OnLabelPositionChanged));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelCheckBox"/> class.
    /// </summary>
    public LabelCheckBox()
    {
        InitializeComponent();
    }

    private static void OnLabelPositionChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelCheckBox)d;

        control.SetCurrentValue(LabelWidthSizeDefinitionProperty, SizeDefinitionType.Pixel);
        control.SetCurrentValue(
            LabelWidthNumberProperty,
            e.NewValue is LabelPosition.Right ? 25 : 120);
    }

    private static void OnIsCheckedChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelCheckBox)d;

        control.IsCheckedChanged?.Invoke(
            control,
            new ValueChangedEventArgs<bool>(
                control.Identifier,
                !control.IsChecked,
                control.IsChecked));
    }
}