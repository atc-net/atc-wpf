// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelFontFamilySelector.
/// </summary>
public partial class LabelFontFamilySelector : ILabelFontFamilySelector
{
    public static readonly DependencyProperty DropDownFirstItemTypeProperty = DependencyProperty.Register(
        nameof(DropDownFirstItemType),
        typeof(DropDownFirstItemType),
        typeof(LabelFontFamilySelector),
        new PropertyMetadata(DropDownFirstItemType.None));

    public DropDownFirstItemType DropDownFirstItemType
    {
        get => (DropDownFirstItemType)GetValue(DropDownFirstItemTypeProperty);
        set => SetValue(DropDownFirstItemTypeProperty, value);
    }

    public static readonly DependencyProperty SelectedKeyProperty = DependencyProperty.Register(
        nameof(SelectedKey),
        typeof(string),
        typeof(LabelFontFamilySelector),
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

    public event EventHandler<ChangedStringEventArgs>? SelectorChanged;

    public event EventHandler<ChangedStringEventArgs>? SelectorLostFocusInvalid;

    public LabelFontFamilySelector()
    {
        InitializeComponent();
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

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK - 'this' cant be used in a static method.")]
    [SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "OK.")]
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
            new ChangedStringEventArgs(
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
        ChangedStringEventArgs e)
    {
        Debug.WriteLine($"LabelFontFamilySelector - Change to: {e.NewValue}");
        ValidateValue(default, this, e.NewValue, raiseEvents: false);
    }

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK - 'this' cant be used in a static method.")]
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
            new ChangedStringEventArgs(
                ControlHelper.GetIdentifier(control),
                oldValue,
                newValue));
    }
}