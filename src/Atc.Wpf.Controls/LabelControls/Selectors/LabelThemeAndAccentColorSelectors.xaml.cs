// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls.LabelControls;

public partial class LabelThemeAndAccentColorSelectors
{
    [DependencyProperty(DefaultValue = Orientation.Horizontal)]
    private Orientation labelControlOrientation;

    [DependencyProperty(DefaultValue = RenderColorIndicatorType.Square)]
    private RenderColorIndicatorType renderColorIndicatorType;

    public LabelThemeAndAccentColorSelectors()
    {
        InitializeComponent();
    }
}