// ReSharper disable CheckNamespace
namespace Atc.Wpf.Forms;

public partial class LabelAccentColorSelector
{
    [DependencyProperty(DefaultValue = RenderColorIndicatorType.Square)]
    private RenderColorIndicatorType renderColorIndicatorType;

    public LabelAccentColorSelector()
    {
        InitializeComponent();
    }
}