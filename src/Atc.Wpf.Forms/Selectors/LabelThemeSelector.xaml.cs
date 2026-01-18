// ReSharper disable CheckNamespace
namespace Atc.Wpf.Forms;

public partial class LabelThemeSelector
{
    [DependencyProperty(DefaultValue = RenderColorIndicatorType.Square)]
    private RenderColorIndicatorType renderColorIndicatorType;

    public LabelThemeSelector()
    {
        InitializeComponent();
    }
}