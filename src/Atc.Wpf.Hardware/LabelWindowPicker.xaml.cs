// ReSharper disable InvertIf
namespace Atc.Wpf.Hardware;

[SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
public partial class LabelWindowPicker : ILabelWindowPicker
{
    [DependencyProperty]
    private TopLevelWindowInfo? value;

    [DependencyProperty(DefaultValue = "")]
    private string watermarkText;

    [DependencyProperty(DefaultValue = true)]
    private bool showRefreshButton;

    [DependencyProperty(DefaultValue = true)]
    private bool autoRefreshOnDeviceChange;

    [DependencyProperty(DefaultValue = false)]
    private bool clearValueOnDisconnect;

    [DependencyProperty(DefaultValue = true)]
    private bool autoRebindOnReconnect;

    [DependencyProperty(DefaultValue = false)]
    private bool autoSelectFirstAvailable;

    public event EventHandler<ValueChangedEventArgs<TopLevelWindowInfo?>>? LostFocusValid;

    public event EventHandler<ValueChangedEventArgs<TopLevelWindowInfo?>>? LostFocusInvalid;

    public LabelWindowPicker()
    {
        InitializeComponent();
    }

    public override bool IsValid()
    {
        Validate(
            this,
            new RoutedPropertyChangedEventArgs<TopLevelWindowInfo?>(oldValue: null, newValue: Value),
            raiseEvents: false);

        return string.IsNullOrEmpty(ValidationText);
    }

    private void OnValueChanged(
        object sender,
        RoutedPropertyChangedEventArgs<TopLevelWindowInfo?> e)
    {
        if (e.OldValue is not null)
        {
            e.OldValue.PropertyChanged -= OnValueStatePropertyChanged;
        }

        if (e.NewValue is not null)
        {
            e.NewValue.PropertyChanged += OnValueStatePropertyChanged;
        }

        Validate(this, e, raiseEvents: true);
    }

    /// <summary>
    /// Re-validates when the selected window's <see cref="DeviceState"/> changes
    /// (e.g. it transitions to <see cref="DeviceState.InUse"/> after an in-use probe,
    /// or to <see cref="DeviceState.Disconnected"/> when the window is closed).
    /// Without this hook, the validation text only refreshes when a different
    /// window is selected.
    /// </summary>
    private void OnValueStatePropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(TopLevelWindowInfo.State))
        {
            return;
        }

        Validate(
            this,
            new RoutedPropertyChangedEventArgs<TopLevelWindowInfo?>(Value, Value),
            raiseEvents: false);
    }

    private static void Validate(
        LabelWindowPicker control,
        RoutedPropertyChangedEventArgs<TopLevelWindowInfo?> e,
        bool raiseEvents)
    {
        control.ValidationText = string.Empty;

        if (control is { IsMandatory: true, Value: null })
        {
            control.ValidationText = Validations.DeviceIsRequired;
            if (raiseEvents)
            {
                OnLostFocusFireInvalidEvent(control, e);
            }

            return;
        }

        if (control.Value is { State: DeviceState.Disconnected })
        {
            control.ValidationText = Validations.DeviceNoLongerAvailable;
            if (raiseEvents)
            {
                OnLostFocusFireInvalidEvent(control, e);
            }

            return;
        }

        if (control.Value is { State: DeviceState.InUse })
        {
            control.ValidationText = Miscellaneous.DeviceInUse;
            return;
        }

        OnLostFocusFireValidEvent(control, e);
    }

    private static void OnLostFocusFireValidEvent(
        LabelWindowPicker control,
        RoutedPropertyChangedEventArgs<TopLevelWindowInfo?> e)
    {
        control.LostFocusValid?.Invoke(
            control,
            new ValueChangedEventArgs<TopLevelWindowInfo?>(
                ControlHelper.GetIdentifier(control),
                e.OldValue,
                e.NewValue));
    }

    private static void OnLostFocusFireInvalidEvent(
        LabelWindowPicker control,
        RoutedPropertyChangedEventArgs<TopLevelWindowInfo?> e)
    {
        control.LostFocusInvalid?.Invoke(
            control,
            new ValueChangedEventArgs<TopLevelWindowInfo?>(
                ControlHelper.GetIdentifier(control),
                e.OldValue,
                e.NewValue));
    }
}