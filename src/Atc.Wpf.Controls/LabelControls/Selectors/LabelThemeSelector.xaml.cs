// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls.LabelControls;

public partial class LabelThemeSelector
{
    public static readonly DependencyProperty RenderColorIndicatorTypeProperty = DependencyProperty.Register(
        nameof(RenderColorIndicatorType),
        typeof(RenderColorIndicatorType),
        typeof(LabelThemeSelector),
        new PropertyMetadata(RenderColorIndicatorType.Square));

    public RenderColorIndicatorType RenderColorIndicatorType
    {
        get => (RenderColorIndicatorType)GetValue(RenderColorIndicatorTypeProperty);
        set => SetValue(RenderColorIndicatorTypeProperty, value);
    }

    public LabelThemeSelector()
    {
        InitializeComponent();
    }
}