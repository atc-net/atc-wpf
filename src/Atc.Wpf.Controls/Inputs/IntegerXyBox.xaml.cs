namespace Atc.Wpf.Controls.Inputs;

public partial class IntegerXyBox
{
    /// <summary>Occurs when the value in the X input box changes.</summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<int>))]
    private static readonly RoutedEvent valueXChanged;

    /// <summary>Occurs when the value in the Y input box changes.</summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<int>))]
    private static readonly RoutedEvent valueYChanged;

    [DependencyProperty]
    private bool hideUpDownButtons;

    [DependencyProperty(
        DefaultValue = PropertyDefaultValueConstants.MinValue,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private int minimum;

    [DependencyProperty(
        DefaultValue = PropertyDefaultValueConstants.MaxValue,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private int maximum;

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
        IsAnimationProhibited = true,
        DefaultUpdateSourceTrigger = UpdateSourceTrigger.LostFocus)]
    private int valueX;

    [DependencyProperty(
        DefaultValue = 0,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        IsAnimationProhibited = true,
        DefaultUpdateSourceTrigger = UpdateSourceTrigger.LostFocus)]
    private int valueY;

    /// <summary>Occurs when an edit of <see cref="ValueX"/> ends: focus leaves the input box after the value changed. Carries the control identifier and the value from before and after the edit.</summary>
    public event EventHandler<ValueChangedEventArgs<int?>>? ValueXLostFocus;

    /// <summary>Occurs when an edit of <see cref="ValueY"/> ends: focus leaves the input box after the value changed. Carries the control identifier and the value from before and after the edit.</summary>
    public event EventHandler<ValueChangedEventArgs<int?>>? ValueYLostFocus;

    /// <summary>Initializes a new instance of the <see cref="IntegerXyBox"/> class.</summary>
    public IntegerXyBox()
    {
        InitializeComponent();

        _ = new LostFocusValueTracker<int>(
            this,
            () => ValueX,
            (oldValue, newValue) => ValueXLostFocus?.Invoke(
                this,
                new ValueChangedEventArgs<int?>(ControlHelper.GetIdentifier(this), oldValue, newValue)));

        _ = new LostFocusValueTracker<int>(
            this,
            () => ValueY,
            (oldValue, newValue) => ValueYLostFocus?.Invoke(
                this,
                new ValueChangedEventArgs<int?>(ControlHelper.GetIdentifier(this), oldValue, newValue)));
    }

    private void OnValueXChanged(
        object sender,
        RoutedPropertyChangedEventArgs<double?> e)
    {
        if (e.OldValue is null || e.NewValue is null)
        {
            return;
        }

        RaiseEvent(new RoutedPropertyChangedEventArgs<int>((int)e.OldValue, (int)e.NewValue, ValueXChangedEvent));
    }

    private void OnValueYChanged(
        object sender,
        RoutedPropertyChangedEventArgs<double?> e)
    {
        if (e.OldValue is null || e.NewValue is null)
        {
            return;
        }

        RaiseEvent(new RoutedPropertyChangedEventArgs<int>((int)e.OldValue, (int)e.NewValue, ValueYChangedEvent));
    }
}