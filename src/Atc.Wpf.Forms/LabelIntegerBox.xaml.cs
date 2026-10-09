namespace Atc.Wpf.Forms;

public partial class LabelIntegerBox : ILabelIntegerBox
{
    [DependencyProperty(DefaultValue = "")]
    private string watermarkText;

    [DependencyProperty(DefaultValue = TextAlignment.Left)]
    private TextAlignment watermarkAlignment;

    [DependencyProperty(DefaultValue = TextTrimming.None)]
    private TextTrimming watermarkTrimming;

    [DependencyProperty(
        DefaultValue = "",
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private string prefixText;

    [DependencyProperty(
        DefaultValue = "",
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private string suffixText;

    [DependencyProperty(
        DefaultValue = 0,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        PropertyChangedCallback = nameof(OnValuePropertyChanged),
        IsAnimationProhibited = true,
        DefaultUpdateSourceTrigger = UpdateSourceTrigger.LostFocus)]
    private int value;

    /// <summary>
    /// Occurs when an edit of <c>Value</c> ends: focus leaves the input box after the value changed.
    /// Carries the control identifier and the value from before and after the edit.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<int?>>? ValueLostFocus;

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelIntegerBox"/> class.
    /// </summary>
    public LabelIntegerBox()
    {
        InitializeComponent();

        _ = new LostFocusValueTracker<int>(
            this,
            () => Value,
            (oldValue, newValue) => ValueLostFocus?.Invoke(
                this,
                new ValueChangedEventArgs<int?>(Identifier, oldValue, newValue)));
    }

    /// <inheritdoc />
    public override bool IsValid()
        => string.IsNullOrEmpty(ValidationText);

    private static void OnValuePropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelIntegerBox)d;

        if (e.NewValue is not int)
        {
            control.ValidationText = Validations.ValueShouldBeAInteger;
            return;
        }
    }
}