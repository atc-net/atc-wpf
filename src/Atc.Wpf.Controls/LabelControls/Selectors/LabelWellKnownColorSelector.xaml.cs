// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelWellKnownColorSelector.
/// </summary>
public partial class LabelWellKnownColorSelector : ILabelComboBoxBase
{
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

    public event EventHandler<ChangedStringEventArgs>? SelectedKeyChanged;

    public event EventHandler<ChangedStringEventArgs>? SelectorLostFocusInvalid;

    public LabelWellKnownColorSelector()
    {
        InitializeComponent();

        if (Constants.DefaultLabelControlLabel.Equals(LabelText, StringComparison.Ordinal))
        {
            LabelText = Miscellaneous.Color;
        }

        CultureManager.UiCultureChanged += OnCultureManagerUiCultureChanged;
    }

    private void OnCultureManagerUiCultureChanged(
        object? sender,
        UiCultureEventArgs e)
    {
        var s = Miscellaneous.ResourceManager.GetString(LabelText, e.OldCulture);
        if (s is not null && s.Equals(LabelText, StringComparison.Ordinal))
        {
            LabelText = Miscellaneous.Color;
        }
    }

    public override bool IsValid()
    {
        ValidateValue(default, this, raiseEvents: false);
        return string.IsNullOrEmpty(ValidationText);
    }

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK - 'this' cant be used in a static method.")]
    private static void OnSelectedKeyLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelWellKnownColorSelector)d;

        ValidateValue(e, control, raiseEvents: true);
    }

    [SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "OK.")]
    private void OnSelectorChanged(
        object? sender,
        ChangedStringEventArgs e)
    {
        var control = this;

        Debug.WriteLine($"LabelWellKnownColorSelector - Change to: {e.NewValue}");

        if (control.IsMandatory &&
            string.IsNullOrWhiteSpace(e.NewValue))
        {
            control.ValidationText = Validations.FieldIsRequired;
            return;
        }

        //var newLcid = NumberHelper.ParseToInt(e.NewValue!);

        //if (newLcid <= 0)
        //{
        //    control.ValidationText = string.Format(CultureInfo.CurrentUICulture, Validations.PlaseSelect, Atc.Resources.Country._Country.ToLower(Thread.CurrentThread.CurrentUICulture));
        //    return;
        //}

        control.SelectedKey = e.NewValue!;
        control.ValidationText = string.Empty;
    }

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK - 'this' cant be used in a static method.")]
    [SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "OK.")]
    private static void ValidateValue(
        DependencyPropertyChangedEventArgs e,
        LabelWellKnownColorSelector control,
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
            control.ValidationText = string.Format(CultureInfo.CurrentUICulture, Validations.PlaseSelect, Wpf.Resources.ColorNames._Color.ToLower(Thread.CurrentThread.CurrentUICulture));
            if (raiseEvents)
            {
                OnSelectorLostFocusFireInvalidEvent(control, e);
            }

            return;
        }

        control.ValidationText = string.Empty;

        var newValue = e.NewValue?.ToString();
        var oldValue = e.OldValue?.ToString();

        control.SelectedKeyChanged?.Invoke(
            control,
            new ChangedStringEventArgs(
                control.Identifier,
                oldValue,
                newValue));
    }

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK - 'this' cant be used in a static method.")]
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
            new ChangedStringEventArgs(
                ControlHelper.GetIdentifier(control),
                oldValue,
                newValue));
    }
}

//var newValue = e.NewValue?.ToString();
//var oldValue = e.OldValue?.ToString();

//if (string.IsNullOrEmpty(newValue) &&
//    string.IsNullOrEmpty(oldValue))
//{
//    return;
//}

//if (control.IsMandatory &&
//    string.IsNullOrWhiteSpace(control.SelectedKey) &&
//    e.OldValue is not null)
//{
//    control.ValidationText = Miscellaneous.FieldIsRequired;
//    return;
//}

//control.ValidationText = string.Empty;

//control.SelectedKeyChanged?.Invoke(
//    control,
//    new ChangedStringEventArgs(
//        control.Identifier,
//        oldValue,
//        newValue));
