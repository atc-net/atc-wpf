// ReSharper disable CheckNamespace
namespace Atc.Wpf.Forms;

[SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "OK.")]
public partial class LabelFontFamilySelector : ILabelFontFamilySelector
{
    [DependencyProperty(DefaultValue = DropDownFirstItemType.None)]
    private DropDownFirstItemType dropDownFirstItemType;

    [DependencyProperty(
        DefaultValue = "",
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        PropertyChangedCallback = nameof(OnSelectorLostFocus),
        IsAnimationProhibited = true,
        DefaultUpdateSourceTrigger = UpdateSourceTrigger.LostFocus)]
    private string selectedKey;

    public event EventHandler<ValueChangedEventArgs<string?>>? SelectorChanged;

    public event EventHandler<ValueChangedEventArgs<string?>>? SelectorLostFocusInvalid;

    public LabelFontFamilySelector()
    {
        InitializeComponent();

        if (string.IsNullOrEmpty(LabelText))
        {
            LabelText = Miscellaneous.Font;
        }

        CultureManager.UiCultureChanged += OnUiCultureChanged;
    }

    private void OnUiCultureChanged(
        object? sender,
        UiCultureEventArgs e)
    {
        var oldTranslation = Miscellaneous.ResourceManager.GetString(nameof(Miscellaneous.Font), e.OldCulture);
        if (oldTranslation is not null && oldTranslation.Equals(LabelText, StringComparison.Ordinal))
        {
            LabelText = Miscellaneous.Font;
        }
    }

    public string GetKey()
    {
        var key = SelectedKey;
        if (string.IsNullOrEmpty(key))
        {
            key = this.FindChild<FontFamilySelector>()?.SelectedKey ?? string.Empty;
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
        LabelFontFamilySelector control,
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
                    Miscellaneous.Font.ToLower(Thread.CurrentThread.CurrentUICulture));

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
        var control = (LabelFontFamilySelector)d;

        ValidateValue(e, control, control.SelectedKey, raiseEvents: true);
    }

    private void OnSelectorChanged(
        object? sender,
        ValueChangedEventArgs<string?> e)
    {
        Debug.WriteLine($"LabelFontFamilySelector - Change to: {e.NewValue}");
        ValidateValue(default, this, e.NewValue, raiseEvents: false);
    }

    private static void OnSelectorLostFocusFireInvalidEvent(
        LabelFontFamilySelector control,
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