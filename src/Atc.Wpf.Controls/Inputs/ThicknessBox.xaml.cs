// ReSharper disable InconsistentNaming
namespace Atc.Wpf.Controls.Inputs;

/// <summary>
/// A control for editing <see cref="Thickness"/> values with four separate input boxes
/// for Left, Top, Right, and Bottom values.
/// </summary>
public partial class ThicknessBox
{
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<double?>))]
    private static readonly RoutedEvent valueChanged;

    [DependencyProperty]
    private bool hideUpDownButtons;

    [DependencyProperty(
        DefaultValue = PropertyDefaultValueConstants.MaxValue,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private double maximum;

    [DependencyProperty(
        DefaultValue = PropertyDefaultValueConstants.MinValue,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private double minimum;

    [DependencyProperty(
        PropertyChangedCallback = nameof(OnValuePropertyChanged),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        IsAnimationProhibited = true)]
    private Thickness value;

    [DependencyProperty(
        PropertyChangedCallback = nameof(OnLeftPropertyChanged),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        IsAnimationProhibited = true)]
    private double left;

    [DependencyProperty(
        PropertyChangedCallback = nameof(OnTopPropertyChanged),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        IsAnimationProhibited = true)]
    private double top;

    [DependencyProperty(
        PropertyChangedCallback = nameof(OnRightPropertyChanged),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        IsAnimationProhibited = true)]
    private double right;

    [DependencyProperty(
        PropertyChangedCallback = nameof(OnBottomPropertyChanged),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        IsAnimationProhibited = true)]
    private double bottom;

    /// <summary>
    /// Occurs when the Left value has changed.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<double?>>? ValueLeftChanged;

    /// <summary>
    /// Occurs when the Top value has changed.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<double?>>? ValueTopChanged;

    /// <summary>
    /// Occurs when the Right value has changed.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<double?>>? ValueRightChanged;

    /// <summary>
    /// Occurs when the Bottom value has changed.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<double?>>? ValueBottomChanged;

    private bool suppressValueSync;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThicknessBox"/> class.
    /// </summary>
    public ThicknessBox()
    {
        InitializeComponent();
    }

    private void OnValueLeftChanged(
        object sender,
        RoutedPropertyChangedEventArgs<double?> e)
    {
        if (e.OldValue is null || e.NewValue is null)
        {
            return;
        }

        ValueLeftChanged?.Invoke(
            this,
            new ValueChangedEventArgs<double?>(
                ControlHelper.GetIdentifier(this),
                e.OldValue,
                e.NewValue));
    }

    private void OnValueTopChanged(
        object sender,
        RoutedPropertyChangedEventArgs<double?> e)
    {
        if (e.OldValue is null || e.NewValue is null)
        {
            return;
        }

        ValueTopChanged?.Invoke(
            this,
            new ValueChangedEventArgs<double?>(
                ControlHelper.GetIdentifier(this),
                e.OldValue,
                e.NewValue));
    }

    private void OnValueRightChanged(
        object sender,
        RoutedPropertyChangedEventArgs<double?> e)
    {
        if (e.OldValue is null || e.NewValue is null)
        {
            return;
        }

        ValueRightChanged?.Invoke(
            this,
            new ValueChangedEventArgs<double?>(
                ControlHelper.GetIdentifier(this),
                e.OldValue,
                e.NewValue));
    }

    private void OnValueBottomChanged(
        object sender,
        RoutedPropertyChangedEventArgs<double?> e)
    {
        if (e.OldValue is null || e.NewValue is null)
        {
            return;
        }

        ValueBottomChanged?.Invoke(
            this,
            new ValueChangedEventArgs<double?>(
                ControlHelper.GetIdentifier(this),
                e.OldValue,
                e.NewValue));
    }

    private static void OnValuePropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (ThicknessBox)d;
        if (control.suppressValueSync)
        {
            return;
        }

        var newValue = (Thickness)e.NewValue;
        control.suppressValueSync = true;
        try
        {
            control.Left = newValue.Left;
            control.Top = newValue.Top;
            control.Right = newValue.Right;
            control.Bottom = newValue.Bottom;
        }
        finally
        {
            control.suppressValueSync = false;
        }
    }

    private static void OnLeftPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (ThicknessBox)d;
        control.SyncValueFromComponents();
    }

    private static void OnTopPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (ThicknessBox)d;
        control.SyncValueFromComponents();
    }

    private static void OnRightPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (ThicknessBox)d;
        control.SyncValueFromComponents();
    }

    private static void OnBottomPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (ThicknessBox)d;
        control.SyncValueFromComponents();
    }

    private void SyncValueFromComponents()
    {
        if (suppressValueSync)
        {
            return;
        }

        suppressValueSync = true;

        try
        {
            var oldValue = Value;
            var newValue = new Thickness(Left, Top, Right, Bottom);
            Value = newValue;

            RaiseEvent(new RoutedPropertyChangedEventArgs<double?>(
                oldValue.Left,
                newValue.Left,
                ValueChangedEvent));
        }
        finally
        {
            suppressValueSync = false;
        }
    }
}