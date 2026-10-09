namespace Atc.Wpf.Controls.Inputs;

public partial class PixelSizeBox
{
    /// <summary>Occurs when the value in the width input box changes.</summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<int>))]
    private static readonly RoutedEvent valueWidthChanged;

    /// <summary>Occurs when the value in the height input box changes.</summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<int>))]
    private static readonly RoutedEvent valueHeightChanged;

    [DependencyProperty]
    private bool hideUpDownButtons;

    [DependencyProperty(
        DefaultValue = PropertyDefaultValueConstants.MaxValue,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private int maximum;

    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        IsAnimationProhibited = true)]
    private int valueWidth;

    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        IsAnimationProhibited = true)]
    private int valueHeight;

    /// <summary>Occurs when an edit of <see cref="ValueWidth"/> ends: focus leaves the input box after the value changed. Carries the control identifier and the value from before and after the edit.</summary>
    public event EventHandler<ValueChangedEventArgs<int?>>? ValueWidthLostFocus;

    /// <summary>Occurs when an edit of <see cref="ValueHeight"/> ends: focus leaves the input box after the value changed. Carries the control identifier and the value from before and after the edit.</summary>
    public event EventHandler<ValueChangedEventArgs<int?>>? ValueHeightLostFocus;

    /// <summary>Initializes a new instance of the <see cref="PixelSizeBox"/> class.</summary>
    public PixelSizeBox()
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

    private void OnValueWidthChanged(
        object sender,
        RoutedPropertyChangedEventArgs<double?> e)
    {
        if (e.OldValue is null || e.NewValue is null)
        {
            return;
        }

        RaiseEvent(new RoutedPropertyChangedEventArgs<int>((int)e.OldValue, (int)e.NewValue, ValueWidthChangedEvent));
    }

    private void OnValueHeightChanged(
        object sender,
        RoutedPropertyChangedEventArgs<double?> e)
    {
        if (e.OldValue is null || e.NewValue is null)
        {
            return;
        }

        RaiseEvent(new RoutedPropertyChangedEventArgs<int>((int)e.OldValue, (int)e.NewValue, ValueHeightChangedEvent));
    }
}