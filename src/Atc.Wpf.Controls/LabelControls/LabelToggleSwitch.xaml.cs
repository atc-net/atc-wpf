using Atc.Wpf.Controls.BaseControls;

namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelToggleSwitch.
/// </summary>
public partial class LabelToggleSwitch
{
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
    nameof(Orientation),
    typeof(Orientation),
    typeof(LabelToggleSwitch),
    new PropertyMetadata(default(Orientation)));

    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public static readonly DependencyProperty LabelTextProperty = DependencyProperty.Register(
        nameof(LabelText),
        typeof(string),
        typeof(LabelToggleSwitch),
        new PropertyMetadata(defaultValue: string.Empty));

    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public static readonly DependencyProperty HideValidationTextAreaProperty = DependencyProperty.Register(
        nameof(HideValidationTextArea),
        typeof(bool),
        typeof(LabelToggleSwitch),
        new PropertyMetadata(defaultValue: false));

    public bool HideValidationTextArea
    {
        get => (bool)GetValue(HideValidationTextAreaProperty);
        set => SetValue(HideValidationTextAreaProperty, value);
    }

    public static readonly DependencyProperty InformationTextProperty = DependencyProperty.Register(
        nameof(InformationText),
        typeof(string),
        typeof(LabelToggleSwitch),
        new PropertyMetadata(defaultValue: string.Empty));

    public string InformationText
    {
        get => (string)GetValue(InformationTextProperty);
        set => SetValue(InformationTextProperty, value);
    }

    public static readonly DependencyProperty InformationColorProperty = DependencyProperty.Register(
        nameof(InformationColor),
        typeof(Color),
        typeof(LabelToggleSwitch),
        new PropertyMetadata(Colors.DodgerBlue));

    public Color InformationColor
    {
        get => (Color)GetValue(InformationColorProperty);
        set => SetValue(InformationColorProperty, value);
    }

    public static readonly DependencyProperty ContentDirectionProperty = DependencyProperty.Register(
        nameof(ContentDirection),
        typeof(FlowDirection),
        typeof(LabelToggleSwitch),
        new PropertyMetadata(FlowDirection.RightToLeft));

    public FlowDirection ContentDirection
    {
        get => (FlowDirection)GetValue(ContentDirectionProperty);
        set => SetValue(ContentDirectionProperty, value);
    }

    public static readonly DependencyProperty OffTextProperty = DependencyProperty.Register(
        nameof(OffText),
        typeof(string),
        typeof(LabelToggleSwitch),
        new FrameworkPropertyMetadata(
            "Off",
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            propertyChangedCallback: null));

    public string OffText
    {
        get => (string)GetValue(OffTextProperty);
        set => SetValue(OffTextProperty, value);
    }

    public static readonly DependencyProperty OnTextProperty = DependencyProperty.Register(
        nameof(OnText),
        typeof(string),
        typeof(LabelToggleSwitch),
        new FrameworkPropertyMetadata(
            "On",
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            propertyChangedCallback: null));

    public string OnText
    {
        get => (string)GetValue(OnTextProperty);
        set => SetValue(OnTextProperty, value);
    }

    public static readonly DependencyProperty IsOnProperty = DependencyProperty.Register(
        nameof(IsOn),
        typeof(bool),
        typeof(LabelToggleSwitch),
        new FrameworkPropertyMetadata(
            defaultValue: false,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            propertyChangedCallback: null));

    public bool IsOn
    {
        get => (bool)GetValue(IsOnProperty);
        set => SetValue(IsOnProperty, value);
    }

    public LabelToggleSwitch()
    {
        InitializeComponent();
    }
}