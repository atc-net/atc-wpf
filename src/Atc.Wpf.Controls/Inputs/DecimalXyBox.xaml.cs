// ReSharper disable InconsistentNaming
namespace Atc.Wpf.Controls.Inputs;

public partial class DecimalXyBox
{
    /// <summary>Occurs when the value in the X input box changes.</summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<decimal>))]
    private static readonly RoutedEvent valueXChanged;

    /// <summary>Occurs when the value in the Y input box changes.</summary>
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<decimal>))]
    private static readonly RoutedEvent valueYChanged;

    [DependencyProperty(DefaultValue = false)]
    private bool hideUpDownButtons;

    [DependencyProperty(
        DefaultValue = 2,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)]
    private int decimalPlaces;

    [DependencyProperty(
        DefaultValue = PropertyDefaultValueConstants.MinValue,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private decimal minimum;

    [DependencyProperty(
        DefaultValue = PropertyDefaultValueConstants.MaxValue,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private decimal maximum;

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

    /// <summary>Occurs when the <see cref="ValueX"/> property changes, with the control identifier and the old and new values.</summary>
    public event EventHandler<ValueChangedEventArgs<decimal?>>? ValueXLostFocus;

    /// <summary>Occurs when the <see cref="ValueY"/> property changes, with the control identifier and the old and new values.</summary>
    public event EventHandler<ValueChangedEventArgs<decimal?>>? ValueYLostFocus;

    /// <summary>Initializes a new instance of the <see cref="DecimalXyBox"/> class.</summary>
    public DecimalXyBox()
    {
        InitializeComponent();
    }

    private void OnValueXChanged(
        object sender,
        RoutedPropertyChangedEventArgs<double?> e)
    {
        if (e.OldValue is null || e.NewValue is null)
        {
            return;
        }

        RaiseEvent(
            new RoutedPropertyChangedEventArgs<decimal>(
                (decimal)e.OldValue,
                (decimal)e.NewValue,
                ValueXChangedEvent));
    }

    private void OnValueYChanged(
        object sender,
        RoutedPropertyChangedEventArgs<double?> e)
    {
        if (e.OldValue is null || e.NewValue is null)
        {
            return;
        }

        RaiseEvent(
            new RoutedPropertyChangedEventArgs<decimal>(
                (decimal)e.OldValue,
                (decimal)e.NewValue,
                ValueYChangedEvent));
    }

    private static void OnValueXLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (DecimalXyBox)d;

        control.ValueXLostFocus?.Invoke(
            control,
            new ValueChangedEventArgs<decimal?>(
                ControlHelper.GetIdentifier(control),
                (decimal)e.OldValue,
                (decimal)e.NewValue));
    }

    private static void OnValueYLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (DecimalXyBox)d;

        control.ValueYLostFocus?.Invoke(
            control,
            new ValueChangedEventArgs<decimal?>(
                ControlHelper.GetIdentifier(control),
                (decimal)e.OldValue,
                (decimal)e.NewValue));
    }
}