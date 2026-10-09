// ReSharper disable CheckNamespace
namespace Atc.Wpf.Forms;

public partial class LabelThemeAndAccentColorSelectors
{
    [DependencyProperty(DefaultValue = Orientation.Horizontal)]
    private Orientation labelControlOrientation;

    [DependencyProperty(DefaultValue = RenderColorIndicatorType.Square)]
    private RenderColorIndicatorType renderColorIndicatorType;

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelThemeAndAccentColorSelectors"/> class.
    /// </summary>
    public LabelThemeAndAccentColorSelectors()
    {
        InitializeComponent();
    }
}