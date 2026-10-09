// ReSharper disable PreferConcreteValueOverDefault
namespace Atc.Wpf.Forms;

public partial class LabelCurrencyBox : ILabelCurrencyBox
{
    [DependencyProperty(DefaultValue = "")]
    private string watermarkText;

    [DependencyProperty(DefaultValue = TextAlignment.Left)]
    private TextAlignment watermarkAlignment;

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
        PropertyChangedCallback = nameof(OnValueLostFocus),
        IsAnimationProhibited = true,
        DefaultUpdateSourceTrigger = UpdateSourceTrigger.LostFocus)]
    private decimal value;

    /// <summary>
    /// Occurs when the <c>Value</c> changes (committed when the control loses focus by default).
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<decimal?>>? ValueChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelCurrencyBox"/> class.
    /// </summary>
    public LabelCurrencyBox()
    {
        InitializeComponent();
    }

    /// <inheritdoc />
    public override bool IsValid()
    {
        ValidateValue(default, this, raiseEvents: false);
        return string.IsNullOrEmpty(ValidationText);
    }

    private static void OnValueLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelCurrencyBox)d;

        ValidateValue(e, control, raiseEvents: true);
    }

    private static void ValidateValue(
        DependencyPropertyChangedEventArgs e,
        LabelCurrencyBox control,
        bool raiseEvents)
    {
        if (e.NewValue is not decimal newValue)
        {
            control.ValidationText = Validations.ValueShouldBeADecimal;
            return;
        }

        if (e.OldValue is not decimal oldValue)
        {
            return;
        }

        if (raiseEvents)
        {
            control.ValueChanged?.Invoke(
                control,
                new ValueChangedEventArgs<decimal?>(
                    control.Identifier,
                    oldValue,
                    newValue));
        }
    }
}