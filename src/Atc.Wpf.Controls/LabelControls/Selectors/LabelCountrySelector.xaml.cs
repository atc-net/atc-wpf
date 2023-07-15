// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelCountrySelector.
/// </summary>
public partial class LabelCountrySelector : ILabelCountrySelector
{
    public static readonly DependencyProperty DropDownFirstItemTypeProperty = DependencyProperty.Register(
        nameof(DropDownFirstItemType),
        typeof(DropDownFirstItemType),
        typeof(LabelCountrySelector),
        new PropertyMetadata(DropDownFirstItemType.None));

    public DropDownFirstItemType DropDownFirstItemType
    {
        get => (DropDownFirstItemType)GetValue(DropDownFirstItemTypeProperty);
        set => SetValue(DropDownFirstItemTypeProperty, value);
    }

    public static readonly DependencyProperty RenderFlagIndicatorTypeTypeProperty = DependencyProperty.Register(
        nameof(RenderFlagIndicatorType),
        typeof(RenderFlagIndicatorType),
        typeof(LabelCountrySelector),
        new PropertyMetadata(RenderFlagIndicatorType.Flat16));

    public RenderFlagIndicatorType RenderFlagIndicatorType
    {
        get => (RenderFlagIndicatorType)GetValue(RenderFlagIndicatorTypeTypeProperty);
        set => SetValue(RenderFlagIndicatorTypeTypeProperty, value);
    }

    public static readonly DependencyProperty UseOnlySupportedCountriesProperty = DependencyProperty.Register(
        nameof(UseOnlySupportedCountries),
        typeof(bool),
        typeof(LabelCountrySelector),
        new PropertyMetadata(defaultValue: true));

    public bool UseOnlySupportedCountries
    {
        get => (bool)GetValue(UseOnlySupportedCountriesProperty);
        set => SetValue(UseOnlySupportedCountriesProperty, value);
    }

    public static readonly DependencyProperty DefaultCultureIdentifierProperty = DependencyProperty.Register(
        nameof(DefaultCultureIdentifier),
        typeof(string),
        typeof(LabelCountrySelector),
        new PropertyMetadata(default));

    public string? DefaultCultureIdentifier
    {
        get => (string?)GetValue(DefaultCultureIdentifierProperty);
        set => SetValue(DefaultCultureIdentifierProperty, value);
    }

    public static readonly DependencyProperty SelectedKeyProperty = DependencyProperty.Register(
        nameof(SelectedKey),
        typeof(string),
        typeof(LabelCountrySelector),
        new FrameworkPropertyMetadata(
            string.Empty,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            OnSelectorLostFocus,
            coerceValueCallback: null,
            isAnimationProhibited: true,
            UpdateSourceTrigger.LostFocus));

    public string SelectedKey
    {
        get => (string)GetValue(SelectedKeyProperty);
        set => SetValue(SelectedKeyProperty, value);
    }

    public event EventHandler<ChangedStringEventArgs>? SelectorLostFocusInvalid;

    public LabelCountrySelector()
    {
        InitializeComponent();
    }

    public override bool IsValid()
    {
        ValidateValue(default, this, raiseEvents: false);
        return string.IsNullOrEmpty(ValidationText);
    }

    private static void OnSelectorLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelCountrySelector)d;

        ValidateValue(e, control, raiseEvents: true);
    }

    [SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "OK.")]
    private void OnSelectorChanged(
        object? sender,
        ChangedStringEventArgs e)
    {
        var control = this;

        Debug.WriteLine($"LabelCountrySelector - Change to: {e.NewValue}");

        if (control.IsMandatory &&
            string.IsNullOrWhiteSpace(e.NewValue))
        {
            control.ValidationText = Validations.FieldIsRequired;
            return;
        }

        var newLcid = NumberHelper.ParseToInt(e.NewValue!);

        if (newLcid <= 0)
        {
            control.ValidationText = string.Format(CultureInfo.CurrentUICulture, Validations.PlaseSelect, Atc.Resources.Country._Country.ToLower(Thread.CurrentThread.CurrentUICulture));
            return;
        }

        control.ValidationText = string.Empty;
    }

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK - 'this' cant be used in a static method.")]
    [SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "OK.")]
    private static void ValidateValue(
        DependencyPropertyChangedEventArgs e,
        LabelCountrySelector control,
        bool raiseEvents)
    {
        if (control.IsMandatory &&
            string.IsNullOrWhiteSpace(control.SelectedKey))
        {
            control.ValidationText = Validations.FieldIsRequired;
            if (raiseEvents)
            {
                OnSelectorLostFocusFireInvalidEvent(control, e);
            }

            return;
        }

        if (control.SelectedKey.StartsWith('-'))
        {
            control.ValidationText = string.Format(CultureInfo.CurrentUICulture, Validations.PlaseSelect, Atc.Resources.Country._Country.ToLower(Thread.CurrentThread.CurrentUICulture));
            if (raiseEvents)
            {
                OnSelectorLostFocusFireInvalidEvent(control, e);
            }

            return;
        }

        control.ValidationText = string.Empty;
    }

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK - 'this' cant be used in a static method.")]
    private static void OnSelectorLostFocusFireInvalidEvent(
        LabelCountrySelector control,
        DependencyPropertyChangedEventArgs e)
    {
        var oldValue = e.OldValue is null
            ? string.Empty
            : e.OldValue.ToString();

        var newValue = e.NewValue is null
            ? string.Empty
            : e.NewValue.ToString();

        control.SelectorLostFocusInvalid?.Invoke(
            control,
            new ChangedStringEventArgs(
                ControlHelper.GetIdentifier(control),
                oldValue,
                newValue));
    }
}