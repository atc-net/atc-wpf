namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// Specifies the per-item position override for a <see cref="TimelineItem"/>.
/// </summary>
public enum TimelineItemPosition
{
    /// <summary>
    /// Uses the position determined by the parent <see cref="Timeline"/> container.
    /// </summary>
    Default,

    /// <summary>
    /// Forces this item to the left side.
    /// </summary>
    Left,

    /// <summary>
    /// Forces this item to the right side.
    /// </summary>
    Right,
}

/// <summary>
/// Represents a single event in a <see cref="Timeline"/> control.
/// </summary>
[ContentProperty(nameof(Content))]
public sealed partial class TimelineItem : ContentControl
{
    /// <summary>
    /// Custom content displayed inside the dot (e.g., an icon or checkmark).
    /// </summary>
    [DependencyProperty]
    private object? dotContent;

    /// <summary>
    /// An optional template for rendering the <see cref="DotContent"/>.
    /// </summary>
    [DependencyProperty]
    private DataTemplate? dotTemplate;

    /// <summary>
    /// The brush used to fill the dot for this item.
    /// When null, falls back to the parent <see cref="Timeline.DotBrush"/>.
    /// </summary>
    [DependencyProperty]
    private Brush? dotBrush;

    /// <summary>
    /// The diameter of the dot for this item.
    /// </summary>
    [DependencyProperty(DefaultValue = 12d)]
    private double dotSize;

    /// <summary>
    /// Content displayed on the opposite side of the main content in Alternate mode.
    /// </summary>
    [DependencyProperty]
    private object? oppositeContent;

    /// <summary>
    /// An optional template for rendering the <see cref="OppositeContent"/>.
    /// </summary>
    [DependencyProperty]
    private DataTemplate? oppositeContentTemplate;

    /// <summary>
    /// Per-item line color override.
    /// When null, falls back to the parent <see cref="Timeline.LineBrush"/>.
    /// </summary>
    [DependencyProperty]
    private Brush? lineStroke;

    /// <summary>
    /// Per-item position override.
    /// </summary>
    [DependencyProperty(DefaultValue = TimelineItemPosition.Default)]
    private TimelineItemPosition position;

    /// <summary>
    /// Indicates whether this is the last item in the timeline.
    /// When true, the connecting line after this item is hidden.
    /// Set automatically by the parent <see cref="Timeline"/> container.
    /// </summary>
    [DependencyProperty(DefaultValue = false)]
    private bool isLast;

    static TimelineItem()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(TimelineItem),
            new FrameworkPropertyMetadata(typeof(TimelineItem)));
    }
}