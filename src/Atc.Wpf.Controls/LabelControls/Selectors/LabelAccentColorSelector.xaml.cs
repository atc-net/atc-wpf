// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls.LabelControls;

public partial class LabelAccentColorSelector
{
    public static readonly DependencyProperty RenderColorIndicatorTypeProperty = DependencyProperty.Register(
        nameof(RenderColorIndicatorType),
        typeof(RenderColorIndicatorType),
        typeof(LabelAccentColorSelector),
        new PropertyMetadata(RenderColorIndicatorType.Square));

    public RenderColorIndicatorType RenderColorIndicatorType
    {
        get => (RenderColorIndicatorType)GetValue(RenderColorIndicatorTypeProperty);
        set => SetValue(RenderColorIndicatorTypeProperty, value);
    }

    public LabelAccentColorSelector()
    {
        InitializeComponent();
    }
}