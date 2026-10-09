namespace Atc.Wpf.Forms;

public partial class LabelPixelSizeBox : ILabelPixelSizeBox
{
    /// <summary>
    /// Occurs when the width value changes in the inner width input box.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<int>))]
    private static readonly RoutedEvent valueWidthChanged;

    /// <summary>
    /// Occurs when the height value changes in the inner height input box.
    /// </summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<int>))]
    private static readonly RoutedEvent valueHeightChanged;

    [DependencyProperty(
        DefaultValue = 0,
        PropertyChangedCallback = nameof(OnValueWidthLostFocus),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        IsAnimationProhibited = true,
        DefaultUpdateSourceTrigger = UpdateSourceTrigger.LostFocus)]
    private int valueWidth;

    [DependencyProperty(
        DefaultValue = 0,
        PropertyChangedCallback = nameof(OnValueHeightLostFocus),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        IsAnimationProhibited = true,
        DefaultUpdateSourceTrigger = UpdateSourceTrigger.LostFocus)]
    private int valueHeight;

    /// <summary>
    /// Occurs when the <c>ValueWidth</c> property changes (committed when the control loses focus by default).
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<int?>>? ValueWidthLostFocus;

    /// <summary>
    /// Occurs when the <c>ValueHeight</c> property changes (committed when the control loses focus by default).
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<int?>>? ValueHeightLostFocus;

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelPixelSizeBox"/> class.
    /// </summary>
    public LabelPixelSizeBox()
    {
        InitializeComponent();
    }

    /// <inheritdoc />
    public override bool IsValid()
        => string.IsNullOrEmpty(ValidationText);

    private void OnValueWidthChanged(
        object sender,
        RoutedPropertyChangedEventArgs<int> e)
    {
        RaiseEvent(new RoutedPropertyChangedEventArgs<int>(
            e.OldValue,
            e.NewValue,
            ValueWidthChangedEvent));
    }

    private void OnValueHeightChanged(
        object sender,
        RoutedPropertyChangedEventArgs<int> e)
    {
        RaiseEvent(new RoutedPropertyChangedEventArgs<int>(
            e.OldValue,
            e.NewValue,
            ValueHeightChangedEvent));
    }

    private static void OnValueWidthLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelPixelSizeBox)d;

        if (e.NewValue is not int newValue)
        {
            control.ValidationText = Validations.ValueShouldBeAInteger;
            return;
        }

        if (e.OldValue is not int oldValue)
        {
            return;
        }

        control.ValueWidthLostFocus?.Invoke(
            control,
            new ValueChangedEventArgs<int?>(
                ControlHelper.GetIdentifier(control),
                oldValue,
                newValue));
    }

    private static void OnValueHeightLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelPixelSizeBox)d;

        if (e.NewValue is not int newValue)
        {
            control.ValidationText = Validations.ValueShouldBeAInteger;
            return;
        }

        if (e.OldValue is not int oldValue)
        {
            return;
        }

        control.ValueHeightLostFocus?.Invoke(
            control,
            new ValueChangedEventArgs<int?>(
                ControlHelper.GetIdentifier(control),
                oldValue,
                newValue));
    }
}