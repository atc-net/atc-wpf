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
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        PropertyChangedCallback = nameof(OnValueWidthPropertyChanged),
        IsAnimationProhibited = true,
        DefaultUpdateSourceTrigger = UpdateSourceTrigger.LostFocus)]
    private int valueWidth;

    [DependencyProperty(
        DefaultValue = 0,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        PropertyChangedCallback = nameof(OnValueHeightPropertyChanged),
        IsAnimationProhibited = true,
        DefaultUpdateSourceTrigger = UpdateSourceTrigger.LostFocus)]
    private int valueHeight;

    /// <summary>
    /// Occurs when an edit of <c>ValueWidth</c> ends: focus leaves the input box after the value changed.
    /// Carries the control identifier and the value from before and after the edit.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<int?>>? ValueWidthLostFocus;

    /// <summary>
    /// Occurs when an edit of <c>ValueHeight</c> ends: focus leaves the input box after the value changed.
    /// Carries the control identifier and the value from before and after the edit.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<int?>>? ValueHeightLostFocus;

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelPixelSizeBox"/> class.
    /// </summary>
    public LabelPixelSizeBox()
    {
        InitializeComponent();

        _ = new LostFocusValueTracker<int>(
            this,
            () => ValueWidth,
            (oldValue, newValue) => ValueWidthLostFocus?.Invoke(
                this,
                new ValueChangedEventArgs<int?>(ControlHelper.GetIdentifier(this), oldValue, newValue)));

        _ = new LostFocusValueTracker<int>(
            this,
            () => ValueHeight,
            (oldValue, newValue) => ValueHeightLostFocus?.Invoke(
                this,
                new ValueChangedEventArgs<int?>(ControlHelper.GetIdentifier(this), oldValue, newValue)));
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

    private static void OnValueWidthPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelPixelSizeBox)d;

        if (e.NewValue is not int)
        {
            control.ValidationText = Validations.ValueShouldBeAInteger;
            return;
        }
    }

    private static void OnValueHeightPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelPixelSizeBox)d;

        if (e.NewValue is not int)
        {
            control.ValidationText = Validations.ValueShouldBeAInteger;
            return;
        }
    }
}