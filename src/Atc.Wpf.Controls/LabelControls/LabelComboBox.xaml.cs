using Atc.Wpf.Controls.LabelControls.EventArgs;

namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelComboBox.
/// </summary>
public partial class LabelComboBox : ILabelComboBox
{
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation),
        typeof(Orientation),
        typeof(LabelComboBox),
        new PropertyMetadata(default(Orientation)));

    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public static readonly DependencyProperty ShowAsteriskOnMandatoryProperty = DependencyProperty.Register(
        nameof(ShowAsteriskOnMandatory),
        typeof(bool),
        typeof(LabelComboBox),
        new PropertyMetadata(defaultValue: true));

    public bool ShowAsteriskOnMandatory
    {
        get => (bool)GetValue(ShowAsteriskOnMandatoryProperty);
        set => SetValue(ShowAsteriskOnMandatoryProperty, value);
    }

    public static readonly DependencyProperty IsMandatoryProperty = DependencyProperty.Register(
        nameof(IsMandatory),
        typeof(bool),
        typeof(LabelComboBox),
        new PropertyMetadata(defaultValue: false));

    public bool IsMandatory
    {
        get => (bool)GetValue(IsMandatoryProperty);
        set => SetValue(IsMandatoryProperty, value);
    }

    public static readonly DependencyProperty MandatoryColorProperty = DependencyProperty.Register(
        nameof(MandatoryColor),
        typeof(SolidColorBrush),
        typeof(LabelComboBox),
        new PropertyMetadata(new SolidColorBrush(Colors.Red)));

    public SolidColorBrush MandatoryColor
    {
        get => (SolidColorBrush)GetValue(MandatoryColorProperty);
        set => SetValue(MandatoryColorProperty, value);
    }

    public static readonly DependencyProperty LabelTextProperty = DependencyProperty.Register(
        nameof(LabelText),
        typeof(string),
        typeof(LabelComboBox),
        new PropertyMetadata(string.Empty));

    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public static readonly DependencyProperty ValidationColorProperty = DependencyProperty.Register(
        nameof(ValidationColor),
        typeof(SolidColorBrush),
        typeof(LabelComboBox),
        new PropertyMetadata(new SolidColorBrush(Colors.OrangeRed)));

    public SolidColorBrush ValidationColor
    {
        get => (SolidColorBrush)GetValue(ValidationColorProperty);
        set => SetValue(ValidationColorProperty, value);
    }

    public static readonly DependencyProperty InformationTextProperty = DependencyProperty.Register(
        nameof(InformationText),
        typeof(string),
        typeof(LabelComboBox),
        new PropertyMetadata(string.Empty));

    public string InformationText
    {
        get => (string)GetValue(InformationTextProperty);
        set => SetValue(InformationTextProperty, value);
    }

    public static readonly DependencyProperty InformationColorProperty = DependencyProperty.Register(
        nameof(InformationColor),
        typeof(Color),
        typeof(LabelComboBox),
        new PropertyMetadata(Colors.DodgerBlue));

    public Color InformationColor
    {
        get => (Color)GetValue(InformationColorProperty);
        set => SetValue(InformationColorProperty, value);
    }

    public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
        nameof(Items),
        typeof(Dictionary<string, string>),
        typeof(LabelComboBox),
        new PropertyMetadata(default(Dictionary<string, string>)));

    public Dictionary<string, string> Items
    {
        get => (Dictionary<string, string>)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public static readonly DependencyProperty SelectedKeyProperty = DependencyProperty.Register(
        nameof(SelectedKey),
        typeof(string),
        typeof(LabelComboBox),
        new PropertyMetadata(
            default(string),
            OnSelectedKeyChanged));

    public string SelectedKey
    {
        get => (string)GetValue(SelectedKeyProperty);
        set => SetValue(SelectedKeyProperty, value);
    }

    public static event EventHandler<SelectedKeyEventArgs>? SelectedKeyChanged;

    public LabelComboBox()
    {
        InitializeComponent();

        DataContext = this;

        Items = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "Key1", "Value1" },
            { "Key2", "Value2" },
            { "Key3", "Value3" },
        };
    }

    private static void OnSelectedKeyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var identifier = string.Empty;
        if (d is LabelComboBox labelComboBox)
        {
            identifier = labelComboBox.Tag is null
                ? labelComboBox.LabelText
                : labelComboBox.Tag.ToString();
        }

        SelectedKeyChanged?.Invoke(
            sender: null,
            new SelectedKeyEventArgs(
                identifier!,
                e.NewValue?.ToString(),
                e.OldValue?.ToString()));
    }
}