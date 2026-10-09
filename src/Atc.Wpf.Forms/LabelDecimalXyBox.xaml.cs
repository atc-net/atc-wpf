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
        PropertyChangedCallback = nameof(OnValueXPropertyChanged),
        IsAnimationProhibited = true,
        DefaultUpdateSourceTrigger = UpdateSourceTrigger.LostFocus)]
    private decimal valueX;

    [DependencyProperty(
        DefaultValue = 0,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        PropertyChangedCallback = nameof(OnValueYPropertyChanged),
        IsAnimationProhibited = true,
        DefaultUpdateSourceTrigger = UpdateSourceTrigger.LostFocus)]
    private decimal valueY;

    /// <summary>
    /// Occurs when an edit of <c>ValueX</c> ends: focus leaves the input box after the value changed.
    /// Carries the control identifier and the value from before and after the edit.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<decimal?>>? ValueXLostFocus;

    /// <summary>
    /// Occurs when an edit of <c>ValueY</c> ends: focus leaves the input box after the value changed.
    /// Carries the control identifier and the value from before and after the edit.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<decimal?>>? ValueYLostFocus;

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelDecimalXyBox"/> class.
    /// </summary>
    public LabelDecimalXyBox()
    {
        InitializeComponent();

        _ = new LostFocusValueTracker<decimal>(
            this,
            () => ValueX,
            (oldValue, newValue) => ValueXLostFocus?.Invoke(
                this,
                new ValueChangedEventArgs<decimal?>(ControlHelper.GetIdentifier(this), oldValue, newValue)));

        _ = new LostFocusValueTracker<decimal>(
            this,
            () => ValueY,
            (oldValue, newValue) => ValueYLostFocus?.Invoke(
                this,
                new ValueChangedEventArgs<decimal?>(ControlHelper.GetIdentifier(this), oldValue, newValue)));
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

    private static void OnValueXPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelDecimalXyBox)d;

        if (e.NewValue is not decimal)
        {
            control.ValidationText = Validations.ValueShouldBeADecimal;
            return;
        }
    }

    private static void OnValueYPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelDecimalXyBox)d;

        if (e.NewValue is not decimal)
        {
            control.ValidationText = Validations.ValueShouldBeADecimal;
            return;
        }
    }
}