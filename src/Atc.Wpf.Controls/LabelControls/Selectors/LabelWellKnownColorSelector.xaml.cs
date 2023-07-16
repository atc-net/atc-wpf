// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelWellKnownColorSelector.
/// </summary>
public partial class LabelWellKnownColorSelector : ILabelComboBoxBase
{
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

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK - 'this' cant be used in a static method.")]
    private static void OnSelectedKeyLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelWellKnownColorSelector)d;
        var newValue = e.NewValue?.ToString();
        var oldValue = e.OldValue?.ToString();

        if (string.IsNullOrEmpty(newValue) &&
            string.IsNullOrEmpty(oldValue))
        {
            return;
        }

        if (control.IsMandatory &&
            string.IsNullOrWhiteSpace(control.SelectedKey) &&
            e.OldValue is not null)
        {
            control.ValidationText = Miscellaneous.FieldIsRequired;
            return;
        }

        control.ValidationText = string.Empty;

        control.SelectedKeyChanged?.Invoke(
            control,
            new ChangedStringEventArgs(
                control.Identifier,
                oldValue,
                newValue));
    }
}