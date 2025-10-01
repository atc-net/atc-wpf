namespace Atc.Wpf.Controls.BaseControls;

public partial class PixelSizeBox
{
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<int>))]
    private static readonly RoutedEvent valueWidthChanged;

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

    public event EventHandler<ValueChangedEventArgs<int?>>? ValueWidthLostFocus;

    public event EventHandler<ValueChangedEventArgs<int?>>? ValueHeightLostFocus;

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