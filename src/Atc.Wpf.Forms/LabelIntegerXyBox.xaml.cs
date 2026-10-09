namespace Atc.Wpf.Forms;

public partial class LabelIntegerXyBox : ILabelIntegerXyBox
{
    /// <summary>
    /// Occurs when the X value changes in the inner X input box.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<int>))]
    private static readonly RoutedEvent valueXChanged;

    /// <summary>
    /// Occurs when the Y value changes in the inner Y input box.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<int>))]
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
        PropertyChangedCallback = nameof(OnValueXLostFocus),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        IsAnimationProhibited = true,
        DefaultUpdateSourceTrigger = UpdateSourceTrigger.LostFocus)]
    private int valueX;

    [DependencyProperty(
        DefaultValue = 0,
        PropertyChangedCallback = nameof(OnValueYLostFocus),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        IsAnimationProhibited = true,
        DefaultUpdateSourceTrigger = UpdateSourceTrigger.LostFocus)]
    private int valueY;

    /// <summary>
    /// Occurs when the <c>ValueX</c> property changes (committed when the control loses focus by default).
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<int?>>? ValueXLostFocus;

    /// <summary>
    /// Occurs when the <c>ValueY</c> property changes (committed when the control loses focus by default).
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<int?>>? ValueYLostFocus;

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelIntegerXyBox"/> class.
    /// </summary>
    public LabelIntegerXyBox()
    {
        InitializeComponent();
    }

    /// <inheritdoc />
    public override bool IsValid()
        => string.IsNullOrEmpty(ValidationText);

    private void OnValueXChanged(
        object sender,
        RoutedPropertyChangedEventArgs<int> e)
    {
        RaiseEvent(new RoutedPropertyChangedEventArgs<int>(
            e.OldValue,
            e.NewValue,
            ValueXChangedEvent));
    }

    private void OnValueYChanged(
        object sender,
        RoutedPropertyChangedEventArgs<int> e)
    {
        RaiseEvent(new RoutedPropertyChangedEventArgs<int>(
            e.OldValue,
            e.NewValue,
            ValueYChangedEvent));
    }

    private static void OnValueXLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelIntegerXyBox)d;

        if (e.NewValue is not int newValue)
        {
            control.ValidationText = Validations.ValueShouldBeAInteger;
            return;
        }

        if (e.OldValue is not int oldValue)
        {
            return;
        }

        control.ValueXLostFocus?.Invoke(
            control,
            new ValueChangedEventArgs<int?>(
                ControlHelper.GetIdentifier(control),
                oldValue,
                newValue));
    }

    private static void OnValueYLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelIntegerXyBox)d;

        if (e.NewValue is not int newValue)
        {
            control.ValidationText = Validations.ValueShouldBeAInteger;
            return;
        }

        if (e.OldValue is not int oldValue)
        {
            return;
        }

        control.ValueYLostFocus?.Invoke(
            control,
            new ValueChangedEventArgs<int?>(
                ControlHelper.GetIdentifier(control),
                oldValue,
                newValue));
    }
}