// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls.LabelControls;

public partial class LabelAccentColorSelector
{
    [DependencyProperty(DefaultValue = RenderColorIndicatorType.Square)]
    private RenderColorIndicatorType renderColorIndicatorType;

    public LabelAccentColorSelector()
    {
        InitializeComponent();
    }
}