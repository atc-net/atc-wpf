// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls.LabelControls;

[SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "OK.")]
public partial class LabelWellKnownColorSelector : ILabelWellKnownColorSelector
{
    private bool isFirstOnSelectedKeyLostFocus;
    private string? lastSelectedKey;

    public static readonly DependencyProperty DropDownFirstItemTypeProperty = DependencyProperty.Register(
        nameof(DropDownFirstItemType),
        typeof(DropDownFirstItemType),
        typeof(LabelWellKnownColorSelector),
        new PropertyMetadata(DropDownFirstItemType.None));

    public DropDownFirstItemType DropDownFirstItemType
    {
        get => (DropDownFirstItemType)GetValue(DropDownFirstItemTypeProperty);
        set => SetValue(DropDownFirstItemTypeProperty, value);
    }

    public static readonly DependencyProperty RenderColorIndicatorTypeProperty = DependencyProperty.Register(
        nameof(RenderColorIndicatorType),
        typeof(RenderColorIndicatorType),
        typeof(LabelWellKnownColorSelector),
        new PropertyMetadata(RenderColorIndicatorType.Square));

    public RenderColorIndicatorType RenderColorIndicatorType
    {
        get => (RenderColorIndicatorType)GetValue(RenderColorIndicatorTypeProperty);
        set => SetValue(RenderColorIndicatorTypeProperty, value);
    }

    public static readonly DependencyProperty ShowHexCodeProperty = DependencyProperty.Register(
        nameof(ShowHexCode),
        typeof(bool),
        typeof(LabelWellKnownColorSelector),
        new PropertyMetadata(defaultValue: BooleanBoxes.FalseBox));

    public bool ShowHexCode
    {
        get => (bool)GetValue(ShowHexCodeProperty);
        set => SetValue(ShowHexCodeProperty, value);
    }

    public static readonly DependencyProperty UseOnlyBasicColorsProperty = DependencyProperty.Register(
        nameof(UseOnlyBasicColors),
        typeof(bool),
        typeof(LabelWellKnownColorSelector),
        new PropertyMetadata(defaultValue: false));

    public bool UseOnlyBasicColors
    {
        get => (bool)GetValue(UseOnlyBasicColorsProperty);
        set => SetValue(UseOnlyBasicColorsProperty, value);
    }

    public static readonly DependencyProperty DefaultColorNameProperty = DependencyProperty.Register(
        nameof(DefaultColorName),
        typeof(string),
        typeof(LabelWellKnownColorSelector),
        new PropertyMetadata(default));

    public string? DefaultColorName
    {
        get => (string?)GetValue(DefaultColorNameProperty);
        set => SetValue(DefaultColorNameProperty, value);
    }

    public static readonly DependencyProperty SelectedKeyProperty = DependencyProperty.Register(
        nameof(SelectedKey),
        typeof(string),
        typeof(LabelWellKnownColorSelector),
        new FrameworkPropertyMetadata(
            default(string),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            OnSelectedKeyLostFocus,
            coerceValueCallback: null,
            isAnimationProhibited: true,
            UpdateSourceTrigger.LostFocus));

    public string SelectedKey
    {
        get => (string)GetValue(SelectedKeyProperty);
        set => SetValue(SelectedKeyProperty, value);
    }

    public event EventHandler<ValueChangedEventArgs<string?>>? SelectorChanged;

    public event EventHandler<ValueChangedEventArgs<string?>>? SelectorLostFocusInvalid;

    public LabelWellKnownColorSelector()
    {
        InitializeComponent();

        isFirstOnSelectedKeyLostFocus = true;
        if (Constants.DefaultLabelControlLabel.Equals(LabelText, StringComparison.Ordinal))
        {
            LabelText = Miscellaneous.Color;
        }

        CultureManager.UiCultureChanged += OnUiCultureChanged;
    }

    public override bool IsValid()
    {
        var validateKey = SelectedKey;
        if (string.IsNullOrEmpty(validateKey))
        {
            validateKey = this.FindChild<WellKnownColorSelector>()?.SelectedKey ?? string.Empty;
        }

        ValidateValue(default, this, validateKey, raiseEvents: false);
        return string.IsNullOrEmpty(ValidationText);
    }

    private static void ValidateValue(
        DependencyPropertyChangedEventArgs e,
        LabelWellKnownColorSelector control,
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
                    Wpf.Resources.ColorNames._Color.ToLower(Thread.CurrentThread.CurrentUICulture));

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

    private void OnUiCultureChanged(
        object? sender,
        UiCultureEventArgs e)
    {
        var s = Miscellaneous.ResourceManager.GetString(LabelText, e.OldCulture);
        if (s is not null && s.Equals(LabelText, StringComparison.Ordinal))
        {
            LabelText = Miscellaneous.Color;
        }
    }

    private static void OnSelectedKeyLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelWellKnownColorSelector)d;

        if (control.SelectedKey is null ||
            control.SelectedKey == control.lastSelectedKey)
        {
            return;
        }

        control.lastSelectedKey = control.SelectedKey;
        if (control.isFirstOnSelectedKeyLostFocus)
        {
            control.isFirstOnSelectedKeyLostFocus = false;
            return;
        }

        ValidateValue(e, control, control.SelectedKey, raiseEvents: true);
    }

    private void OnSelectorChanged(
        object? sender,
        ValueChangedEventArgs<string?> e)
    {
        Debug.WriteLine($"LabelWellKnownColorSelector - Change to: {e.NewValue}");
        ValidateValue(default, this, e.NewValue, raiseEvents: false);
    }

    private static void OnSelectorLostFocusFireInvalidEvent(
        LabelWellKnownColorSelector control,
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