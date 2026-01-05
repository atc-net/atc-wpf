// ReSharper disable InvertIf
// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
namespace Atc.Wpf.Controls.Layouts;

/// <summary>
/// A panel that arranges items in a staggered/masonry layout.
/// Items are placed in the column with the least height, creating a waterfall effect.
/// </summary>
/// <remarks>
/// <para>
/// This panel measures and arranges all children, which is suitable for small to medium collections.
/// For large collections (hundreds or thousands of items), consider using
/// <see cref="VirtualizingStaggeredPanel"/> which implements virtualization.
/// </para>
/// <para>
/// The number of columns is automatically calculated based on <see cref="DesiredItemWidth"/>
/// and the available width. When <see cref="FrameworkElement.HorizontalAlignment"/> is set to
/// <see cref="HorizontalAlignment.Stretch"/>, columns will expand to fill available space.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// &lt;atc:StaggeredPanel DesiredItemWidth="200" HorizontalSpacing="10" VerticalSpacing="10"&gt;
///     &lt;Border Height="100" Background="Red"/&gt;
///     &lt;Border Height="150" Background="Green"/&gt;
///     &lt;Border Height="120" Background="Blue"/&gt;
/// &lt;/atc:StaggeredPanel&gt;
/// </code>
/// </example>
public sealed partial class StaggeredPanel : Panel
{
    private double itemWidth;

    /// <summary>
    /// Gets or sets the desired width for each column.
    /// The actual width may be larger if <see cref="FrameworkElement.HorizontalAlignment"/>
    /// is set to <see cref="HorizontalAlignment.Stretch"/>.
    /// </summary>
    /// <value>The desired column width in device-independent units. Default is 250.</value>
    [DependencyProperty(
        DefaultValue = 250,
        PropertyChangedCallback = nameof(OnInvalidateMeasure))]
    private double desiredItemWidth;

    /// <summary>
    /// Gets or sets the padding inside the panel.
    /// </summary>
    /// <value>The padding as a <see cref="Thickness"/>. Default is 0 on all sides.</value>
    [DependencyProperty(
        DefaultValue = "default(Thickness)",
        PropertyChangedCallback = nameof(OnInvalidateMeasure))]
    private Thickness padding;

    /// <summary>
    /// Gets or sets the horizontal spacing between columns.
    /// </summary>
    /// <value>The horizontal spacing in device-independent units. Default is 0.</value>
    [DependencyProperty(
        DefaultValue = 0,
        PropertyChangedCallback = nameof(OnInvalidateMeasure))]
    private double horizontalSpacing;

    /// <summary>
    /// Gets or sets the vertical spacing between items in a column.
    /// </summary>
    /// <value>The vertical spacing in device-independent units. Default is 0.</value>
    [DependencyProperty(
        DefaultValue = 0,
        PropertyChangedCallback = nameof(OnInvalidateMeasure))]
    private double verticalSpacing;

    private static void OnInvalidateMeasure(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var staggeredPanel = (StaggeredPanel)d;
        staggeredPanel.InvalidateMeasure();
    }

    static StaggeredPanel()
    {
        HorizontalAlignmentProperty.OverrideMetadata(
            typeof(StaggeredPanel),
            new FrameworkPropertyMetadata(OnHorizontalAlignmentChanged));
    }

    /// <inheritdoc />
    protected override Size MeasureOverride(
        Size availableSize)
    {
        if (Children.Count == 0)
        {
            return new Size(0, 0);
        }

        var availableWidth = availableSize.Width - Padding.Left - Padding.Right;
        var availableHeight = availableSize.Height - Padding.Top - Padding.Bottom;

        itemWidth = System.Math.Min(DesiredItemWidth, availableWidth);
        var numColumns = System.Math.Max(1, (int)System.Math.Floor(availableWidth / (itemWidth + HorizontalSpacing)));

        var totalWidth = itemWidth + ((numColumns - 1) * (itemWidth + HorizontalSpacing));
        if (totalWidth > availableWidth)
        {
            numColumns--;
        }
        else if (double.IsInfinity(availableWidth))
        {
            availableWidth = totalWidth;
        }

        if (HorizontalAlignment == HorizontalAlignment.Stretch)
        {
            var occupiedSpacing = (numColumns - 1) * HorizontalSpacing;
            if (availableWidth < occupiedSpacing)
            {
                occupiedSpacing = availableWidth;
            }

            availableWidth -= occupiedSpacing;
            itemWidth = availableWidth / numColumns;
        }

        var columnHeights = new double[numColumns];
        var itemsPerColumn = new int[numColumns];

        for (var i = 0; i < Children.Count; i++)
        {
            var columnIndex = GetShortestColumnIndex(columnHeights);

            var child = Children[i];
            child.Measure(new Size(itemWidth, availableHeight));
            var elementSize = child.DesiredSize;
            columnHeights[columnIndex] += elementSize.Height + (itemsPerColumn[columnIndex] > 0 ? VerticalSpacing : 0);
            itemsPerColumn[columnIndex]++;
        }

        var desiredHeight = columnHeights.Length > 0 ? columnHeights.Max() : 0;

        return new Size(availableWidth, desiredHeight);
    }

    /// <inheritdoc />
    protected override Size ArrangeOverride(
        Size finalSize)
    {
        if (Children.Count == 0)
        {
            return finalSize;
        }

        var horizontalOffset = Padding.Left;
        var verticalOffset = Padding.Top;
        var numColumns = System.Math.Max(1, (int)System.Math.Floor((finalSize.Width + HorizontalSpacing) / (itemWidth + HorizontalSpacing)));

        var totalWidth = itemWidth + ((numColumns - 1) * (itemWidth + HorizontalSpacing));
        if (totalWidth > finalSize.Width)
        {
            numColumns--;
            totalWidth = itemWidth + ((numColumns - 1) * (itemWidth + HorizontalSpacing));
        }

        switch (HorizontalAlignment)
        {
            case HorizontalAlignment.Right:
                horizontalOffset += finalSize.Width - totalWidth;
                break;
            case HorizontalAlignment.Center:
                horizontalOffset += (finalSize.Width - totalWidth) / 2;
                break;
        }

        var columnHeights = new double[numColumns];
        var itemsPerColumn = new int[numColumns];

        for (var i = 0; i < Children.Count; i++)
        {
            var columnIndex = GetShortestColumnIndex(columnHeights);

            var child = Children[i];
            var elementSize = child.DesiredSize;
            var elementHeight = elementSize.Height;

            var itemHorizontalOffset = horizontalOffset + (itemWidth * columnIndex) + (HorizontalSpacing * columnIndex);
            var itemVerticalOffset = columnHeights[columnIndex] + verticalOffset + (VerticalSpacing * itemsPerColumn[columnIndex]);

            var bounds = new Rect(itemHorizontalOffset, itemVerticalOffset, itemWidth, elementHeight);
            child.Arrange(bounds);

            columnHeights[columnIndex] += elementSize.Height;
            itemsPerColumn[columnIndex]++;
        }

        return base.ArrangeOverride(finalSize);
    }

    private static void OnHorizontalAlignmentChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not StaggeredPanel panel)
        {
            return;
        }

        panel.InvalidateMeasure();
    }

    private static int GetShortestColumnIndex(
        IReadOnlyList<double> columnHeights)
    {
        var columnIndex = 0;
        var height = columnHeights[0];

        for (var j = 1; j < columnHeights.Count; j++)
        {
            if (columnHeights[j] < height)
            {
                columnIndex = j;
                height = columnHeights[j];
            }
        }

        return columnIndex;
    }
}
