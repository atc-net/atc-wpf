// ReSharper disable PreferConcreteValueOverDefault
namespace Atc.Wpf.Forms;

public partial class LabelDecimalXyBox : ILabelDecimalXyBox
{
    /// <summary>
    /// Occurs when the X value changes in the inner X input box.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<decimal>))]
    private static readonly RoutedEvent valueXChanged;

    /// <summary>
    /// Occurs when the Y value changes in the inner Y input box.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<decimal>))]
    private static readonly RoutedEvent valueYChanged;

    [DependencyProperty(
        DefaultValue = "",
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private string prefixTextX;

    [DependencyProperty(
        DefaultValue = "",
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private string prefixTextY;

    [DependencyProperty(
        DefaultValue = "",
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private string suffixText;

    [DependencyProperty(
        DefaultValue = 0,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        PropertyChangedCallback = nameof(OnValueXLostFocus),
        IsAnimationProhibited = true,
        DefaultUpdateSourceTrigger = UpdateSourceTrigger.LostFocus)]
    private decimal valueX;

    [DependencyProperty(
        DefaultValue = 0,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        PropertyChangedCallback = nameof(OnValueYLostFocus),
        IsAnimationProhibited = true,
        DefaultUpdateSourceTrigger = UpdateSourceTrigger.LostFocus)]
    private decimal valueY;

    /// <summary>
    /// Occurs when the <c>ValueX</c> property changes (committed when the control loses focus by default).
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<decimal?>>? ValueXLostFocus;

    /// <summary>
    /// Occurs when the <c>ValueY</c> property changes (committed when the control loses focus by default).
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<decimal?>>? ValueYLostFocus;

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelDecimalXyBox"/> class.
    /// </summary>
    public LabelDecimalXyBox()
    {
        InitializeComponent();
    }

    /// <inheritdoc />
    public override bool IsValid()
        => string.IsNullOrEmpty(ValidationText);

    private void OnValueXChanged(
        object sender,
        RoutedPropertyChangedEventArgs<decimal> e)
    {
        RaiseEvent(new RoutedPropertyChangedEventArgs<decimal>(
            e.OldValue,
            e.NewValue,
            ValueXChangedEvent));
    }

    private void OnValueYChanged(
        object sender,
        RoutedPropertyChangedEventArgs<decimal> e)
    {
        RaiseEvent(new RoutedPropertyChangedEventArgs<decimal>(
            e.OldValue,
            e.NewValue,
            ValueYChangedEvent));
    }

    private static void OnValueXLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelDecimalXyBox)d;

        if (e.NewValue is not decimal newValue)
        {
            control.ValidationText = Validations.ValueShouldBeADecimal;
            return;
        }

        if (e.OldValue is not decimal oldValue)
        {
            return;
        }

        control.ValueXLostFocus?.Invoke(
            control,
            new ValueChangedEventArgs<decimal?>(
                ControlHelper.GetIdentifier(control),
                oldValue,
                newValue));
    }

    private static void OnValueYLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelDecimalXyBox)d;

        if (e.NewValue is not decimal newValue)
        {
            control.ValidationText = Validations.ValueShouldBeADecimal;
            return;
        }

        if (e.OldValue is not decimal oldValue)
        {
            return;
        }

        control.ValueYLostFocus?.Invoke(
            control,
            new ValueChangedEventArgs<decimal?>(
                ControlHelper.GetIdentifier(control),
                oldValue,
                newValue));
    }
}