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
        PropertyChangedCallback = nameof(OnValueWidthLostFocus),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        IsAnimationProhibited = true)]
    private int valueWidth;

    [DependencyProperty(
        PropertyChangedCallback = nameof(OnValueHeightLostFocus),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        IsAnimationProhibited = true)]
    private int valueHeight;

    /// <summary>Occurs when the <see cref="ValueWidth"/> property changes, with the control identifier and the old and new values.</summary>
    public event EventHandler<ValueChangedEventArgs<int?>>? ValueWidthLostFocus;

    /// <summary>Occurs when the <see cref="ValueHeight"/> property changes, with the control identifier and the old and new values.</summary>
    public event EventHandler<ValueChangedEventArgs<int?>>? ValueHeightLostFocus;

    /// <summary>Initializes a new instance of the <see cref="PixelSizeBox"/> class.</summary>
    public PixelSizeBox()
    {
        InitializeComponent();
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

    private static void OnValueWidthLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (PixelSizeBox)d;

        control.ValueWidthLostFocus?.Invoke(
            control,
            new ValueChangedEventArgs<int?>(
                ControlHelper.GetIdentifier(control),
                (int)e.OldValue,
                (int)e.NewValue));
    }

    private static void OnValueHeightLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (PixelSizeBox)d;

        control.ValueHeightLostFocus?.Invoke(
            control,
            new ValueChangedEventArgs<int?>(
                ControlHelper.GetIdentifier(control),
                (int)e.OldValue,
                (int)e.NewValue));
    }
}