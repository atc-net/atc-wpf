// ReSharper disable UseCollectionExpression
// ReSharper disable UnusedMember.Local
namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>A visual separator line for dividing content sections, with optional inline content.</summary>
[ContentProperty(nameof(Content))]
public sealed partial class Divider : Control
{
    [DependencyProperty]
    private object? content;

    [DependencyProperty(DefaultValue = default(Orientation))]
    private Orientation orientation;

    [DependencyProperty]
    private DataTemplate? contentTemplate;

    [DependencyProperty]
    private string? contentStringFormat;

    [DependencyProperty]
    private DataTemplateSelector? contentTemplateSelector;

    [DependencyProperty]
    private Brush? lineStroke;

    [DependencyProperty(DefaultValue = 1.0)]
    private double lineStrokeThickness;

    [DependencyProperty(DefaultValue = $"new {nameof(DoubleCollection)}()")]
    private DoubleCollection lineStrokeDashArray = new();
}