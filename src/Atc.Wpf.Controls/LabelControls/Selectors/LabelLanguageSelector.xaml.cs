// ReSharper disable CheckNamespace
// ReSharper disable InvertIf
namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelLanguageSelector.
/// </summary>
public partial class LabelLanguageSelector : ILabelLanguageSelector
{
    public static readonly DependencyProperty DropDownFirstItemTypeProperty = DependencyProperty.Register(
        nameof(DropDownFirstItemType),
        typeof(DropDownFirstItemType),
        typeof(LabelLanguageSelector),
        new PropertyMetadata(DropDownFirstItemType.None));

    public DropDownFirstItemType DropDownFirstItemType
    {
        get => (DropDownFirstItemType)GetValue(DropDownFirstItemTypeProperty);
        set => SetValue(DropDownFirstItemTypeProperty, value);
    }

    public static readonly DependencyProperty RenderFlagIndicatorTypeTypeProperty = DependencyProperty.Register(
        nameof(RenderFlagIndicatorType),
        typeof(RenderFlagIndicatorType),
        typeof(LabelLanguageSelector),
        new PropertyMetadata(RenderFlagIndicatorType.Flat16));

    public RenderFlagIndicatorType RenderFlagIndicatorType
    {
        get => (RenderFlagIndicatorType)GetValue(RenderFlagIndicatorTypeTypeProperty);
        set => SetValue(RenderFlagIndicatorTypeTypeProperty, value);
    }

    public static readonly DependencyProperty UseOnlySupportedLanguagesProperty = DependencyProperty.Register(
        nameof(UseOnlySupportedLanguages),
        typeof(bool),
        typeof(LabelLanguageSelector),
        new PropertyMetadata(defaultValue: true));

    public bool UseOnlySupportedLanguages
    {
        get => (bool)GetValue(UseOnlySupportedLanguagesProperty);
        set => SetValue(UseOnlySupportedLanguagesProperty, value);
    }

    public static readonly DependencyProperty DefaultCultureIdentifierProperty = DependencyProperty.Register(
        nameof(DefaultCultureIdentifier),
        typeof(string),
        typeof(LabelLanguageSelector),
        new PropertyMetadata(default));

    public string? DefaultCultureIdentifier
    {
        get => (string?)GetValue(DefaultCultureIdentifierProperty);
        set => SetValue(DefaultCultureIdentifierProperty, value);
    }

    public static readonly DependencyProperty SelectedKeyProperty = DependencyProperty.Register(
        nameof(SelectedKey),
        typeof(string),
        typeof(LabelLanguageSelector),
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

    public static readonly DependencyProperty UpdateUiCultureOnChangeEventProperty = DependencyProperty.Register(
        nameof(UpdateUiCultureOnChangeEvent),
        typeof(bool),
        typeof(LabelLanguageSelector),
        new PropertyMetadata(defaultValue: true));

    public bool UpdateUiCultureOnChangeEvent
    {
        get => (bool)GetValue(UpdateUiCultureOnChangeEventProperty);
        set => SetValue(UpdateUiCultureOnChangeEventProperty, value);
    }

    public event EventHandler<ChangedStringEventArgs>? SelectorLostFocusInvalid;

    public LabelLanguageSelector()
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
        var control = (LabelLanguageSelector)d;

        ValidateValue(e, control, raiseEvents: true);
    }

    [SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "OK.")]
    private void OnSelectorChanged(
        object? sender,
        ChangedStringEventArgs e)
    {
        var control = this;

        Debug.WriteLine($"LabelLanguageSelector - Change to: {e.NewValue}");

        if (control.IsMandatory &&
            string.IsNullOrWhiteSpace(e.NewValue))
        {
            control.ValidationText = Validations.FieldIsRequired;
            return;
        }

        var newLcid = NumberHelper.ParseToInt(e.NewValue!);

        if (newLcid <= 0)
        {
            control.ValidationText = string.Format(CultureInfo.CurrentUICulture, Validations.PlaseSelect, Atc.Resources.Language._Language.ToLower(Thread.CurrentThread.CurrentUICulture));
            return;
        }

        control.SelectedKey = e.NewValue!;
        control.ValidationText = string.Empty;
    }

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK - 'this' cant be used in a static method.")]
    [SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "OK.")]
    private static void ValidateValue(
        DependencyPropertyChangedEventArgs e,
        LabelLanguageSelector control,
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
            control.ValidationText = string.Format(CultureInfo.CurrentUICulture, Validations.PlaseSelect, Atc.Resources.Language._Language.ToLower(Thread.CurrentThread.CurrentUICulture));
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
        LabelLanguageSelector control,
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