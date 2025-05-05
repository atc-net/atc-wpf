// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls.LabelControls;

[SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "OK.")]
public partial class LabelCountrySelector : ILabelCountrySelector
{
    [DependencyProperty(DefaultValue = DropDownFirstItemType.None)]
    private DropDownFirstItemType dropDownFirstItemType;

    [DependencyProperty(DefaultValue = RenderFlagIndicatorType.Flat16)]
    private RenderFlagIndicatorType renderFlagIndicatorType;

    [DependencyProperty(DefaultValue = true)]
    private bool useOnlySupportedCountries;

    [DependencyProperty]
    private string defaultCultureIdentifier;

    // Note: DependencyProperty-SourceGenerator don't support "coerceValueCallback / isAnimationProhibited" correctly for now
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

    // Note: DependencyProperty-SourceGenerator don't support "coerceValueCallback / isAnimationProhibited" correctly for now
    public string SelectedKey
    {
        get => (string)GetValue(SelectedKeyProperty);
        set => SetValue(SelectedKeyProperty, value);
    }

    public event EventHandler<ValueChangedEventArgs<string?>>? SelectorChanged;

    public event EventHandler<ValueChangedEventArgs<string?>>? SelectorLostFocusInvalid;

    public LabelCountrySelector()
    {
        InitializeComponent();
    }

    public string GetKey()
    {
        var key = SelectedKey;
        if (string.IsNullOrEmpty(key))
        {
            key = this.FindChild<CountrySelector>()?.SelectedKey ?? string.Empty;
        }

        return key;
    }

    public override bool IsValid()
    {
        ValidateValue(default, this, GetKey(), raiseEvents: false);
        return string.IsNullOrEmpty(ValidationText);
    }

    private static void ValidateValue(
        DependencyPropertyChangedEventArgs e,
        LabelCountrySelector control,
        string? selectedKey,
        bool raiseEvents)
    {
        if (control.IsMandatory)
        {
            if (string.IsNullOrWhiteSpace(selectedKey))
            {
                control.ValidationText = Validations.FieldIsRequired;
                if (raiseEvents)
                {
                    OnSelectorLostFocusFireInvalidEvent(control, e);
                }

                return;
            }

            if (selectedKey == ((int)control.DropDownFirstItemType).ToString(GlobalizationConstants.EnglishCultureInfo))
            {
                control.ValidationText = string.Format(
                    CultureInfo.CurrentUICulture,
                    Validations.PleaseSelect1,
                    Atc.Resources.Country._Country.ToLower(Thread.CurrentThread.CurrentUICulture));

                if (raiseEvents)
                {
                    OnSelectorLostFocusFireInvalidEvent(control, e);
                }

                return;
            }
        }

        control.ValidationText = string.Empty;

        if (!raiseEvents)
        {
            return;
        }

        control.SelectorChanged?.Invoke(
            control,
            new ValueChangedEventArgs<string?>(
                control.Identifier,
                e.OldValue?.ToString(),
                selectedKey));
    }

    private static void OnSelectorLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelCountrySelector)d;

        ValidateValue(e, control, control.SelectedKey, raiseEvents: true);
    }

    private void OnSelectorChanged(
        object? sender,
        ValueChangedEventArgs<string?> e)
    {
        Debug.WriteLine($"LabelCountrySelector - Change to: {e.NewValue}");
        ValidateValue(default, this, e.NewValue, raiseEvents: false);
    }

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
            new ValueChangedEventArgs<string?>(
                ControlHelper.GetIdentifier(control),
                oldValue,
                newValue));
    }
}