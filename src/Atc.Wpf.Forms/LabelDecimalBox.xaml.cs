// ReSharper disable PreferConcreteValueOverDefault
namespace Atc.Wpf.Forms;

public partial class LabelDecimalBox : ILabelDecimalBox
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

    public event EventHandler<ValueChangedEventArgs<decimal?>>? ValueChanged;

    public LabelDecimalBox()
    {
        InitializeComponent();
    }

    public override bool IsValid()
    {
        ValidateValue(default, this, raiseEvents: false);
        return string.IsNullOrEmpty(ValidationText);
    }

    private static void OnValueLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelDecimalBox)d;

        ValidateValue(e, control, raiseEvents: true);
    }

    private static void ValidateValue(
        DependencyPropertyChangedEventArgs e,
        LabelDecimalBox control,
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