// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelWellKnownColorSelector.
/// </summary>
public partial class LabelWellKnownColorSelector : ILabelComboBoxBase
{
    public static new readonly DependencyProperty LabelTextProperty = DependencyProperty.Register(
        nameof(LabelText),
        typeof(string),
        typeof(LabelWellKnownColorSelector),
        new FrameworkPropertyMetadata(
            Miscellaneous.Color,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            OnLabelTextChanged));

    public static readonly DependencyProperty ShowAsteriskOnMandatoryProperty = DependencyProperty.Register(
        nameof(ShowAsteriskOnMandatory),
        typeof(bool),
        typeof(LabelWellKnownColorSelector),
        new PropertyMetadata(defaultValue: true));

    public bool ShowAsteriskOnMandatory
    {
        get => (bool)GetValue(ShowAsteriskOnMandatoryProperty);
        set => SetValue(ShowAsteriskOnMandatoryProperty, value);
    }

    public static readonly DependencyProperty IsMandatoryProperty = DependencyProperty.Register(
        nameof(IsMandatory),
        typeof(bool),
        typeof(LabelWellKnownColorSelector),
        new PropertyMetadata(defaultValue: false));

    public bool IsMandatory
    {
        get => (bool)GetValue(IsMandatoryProperty);
        set => SetValue(IsMandatoryProperty, value);
    }

    public static readonly DependencyProperty MandatoryColorProperty = DependencyProperty.Register(
        nameof(MandatoryColor),
        typeof(SolidColorBrush),
        typeof(LabelWellKnownColorSelector),
        new PropertyMetadata(new SolidColorBrush(Colors.Red)));

    public SolidColorBrush MandatoryColor
    {
        get => (SolidColorBrush)GetValue(MandatoryColorProperty);
        set => SetValue(MandatoryColorProperty, value);
    }

    public static readonly DependencyProperty ValidationColorProperty = DependencyProperty.Register(
        nameof(ValidationColor),
        typeof(SolidColorBrush),
        typeof(LabelWellKnownColorSelector),
        new PropertyMetadata(new SolidColorBrush(Colors.OrangeRed)));

    public SolidColorBrush ValidationColor
    {
        get => (SolidColorBrush)GetValue(ValidationColorProperty);
        set => SetValue(ValidationColorProperty, value);
    }

    public static readonly DependencyProperty ValidationTextProperty = DependencyProperty.Register(
        nameof(ValidationText),
        typeof(string),
        typeof(LabelWellKnownColorSelector),
        new PropertyMetadata(default(string)));

    public string ValidationText
    {
        get => (string)GetValue(ValidationTextProperty);
        set => SetValue(ValidationTextProperty, value);
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
        new PropertyMetadata(defaultValue: false));

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
            OnSelectedKeyChanged));

    public string SelectedKey
    {
        get => (string)GetValue(SelectedKeyProperty);
        set => SetValue(SelectedKeyProperty, value);
    }

    public static event EventHandler<SelectedKeyEventArgs>? SelectedKeyChanged;

    public LabelWellKnownColorSelector()
    {
        InitializeComponent();

        CultureManager.UiCultureChanged += OnCultureManagerUiCultureChanged;
    }

    private void OnCultureManagerUiCultureChanged(
        object? sender,
        UiCultureEventArgs e)
    {
        if (string.IsNullOrEmpty(LabelText))
        {
            LabelText = Miscellaneous.Color;
        }
        else
        {
            var s = Miscellaneous.ResourceManager.GetString(LabelText, e.OldCulture);
            if (s is not null && s.Equals(LabelText, StringComparison.Ordinal))
            {
                LabelText = Miscellaneous.Color;
            }
        }
    }

    private static void OnLabelTextChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var labelWellKnownColorSelector = (LabelWellKnownColorSelector)d;

        if (string.IsNullOrEmpty(labelWellKnownColorSelector.LabelText))
        {
            labelWellKnownColorSelector.LabelText = Miscellaneous.Color;
        }
    }

    private static void OnSelectedKeyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var labelWellKnownColorSelector = (LabelWellKnownColorSelector)d;

        if (labelWellKnownColorSelector.IsMandatory &&
            string.IsNullOrWhiteSpace(labelWellKnownColorSelector.SelectedKey) &&
            e.OldValue is not null)
        {
            labelWellKnownColorSelector.ValidationText = "Field is required";
            return;
        }

        labelWellKnownColorSelector.ValidationText = string.Empty;

        var identifier = labelWellKnownColorSelector.Tag is null
            ? "Color"
            : labelWellKnownColorSelector.Tag.ToString();

        SelectedKeyChanged?.Invoke(
            sender: null,
            new SelectedKeyEventArgs(
                identifier!,
                e.NewValue?.ToString(),
                e.OldValue?.ToString()));
    }
}