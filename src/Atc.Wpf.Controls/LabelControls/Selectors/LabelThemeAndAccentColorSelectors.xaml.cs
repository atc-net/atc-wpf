// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelThemeAndAccentColorSelectors.
/// </summary>
public partial class LabelThemeAndAccentColorSelectors
{
    public static readonly DependencyProperty LabelControlOrientationProperty = DependencyProperty.Register(
        nameof(LabelControlOrientation),
        typeof(Orientation),
        typeof(LabelThemeAndAccentColorSelectors),
        new PropertyMetadata(default(Orientation)));

    public Orientation LabelControlOrientation
    {
        get => (Orientation)GetValue(LabelControlOrientationProperty);
        set => SetValue(LabelControlOrientationProperty, value);
    }

    public static readonly DependencyProperty RenderColorIndicatorTypeProperty = DependencyProperty.Register(
        nameof(RenderColorIndicatorType),
        typeof(RenderColorIndicatorType),
        typeof(LabelThemeAndAccentColorSelectors),
        new PropertyMetadata(RenderColorIndicatorType.Square));

    public RenderColorIndicatorType RenderColorIndicatorType
    {
        get => (RenderColorIndicatorType)GetValue(RenderColorIndicatorTypeProperty);
        set => SetValue(RenderColorIndicatorTypeProperty, value);
    }

    public LabelThemeAndAccentColorSelectors()
    {
        InitializeComponent();
    }
}