// ReSharper disable InvertIf
namespace Atc.Wpf.Controls.Layouts;

using Math = System.Math;

/// <summary>
/// A virtualizing panel that arranges items in a staggered/masonry layout.
/// Items are placed in the column with the least height, creating a waterfall effect.
/// Supports virtualization for large item collections through IScrollInfo.
/// </summary>
/// <remarks>
/// Use this panel inside an ItemsControl with VirtualizingPanel.IsVirtualizing="True"
/// for best performance with large collections.
/// </remarks>
public sealed partial class VirtualizingStaggeredPanel : VirtualizingPanel, IScrollInfo
{
    private const double ScrollLineSize = 16;
    private const double ScrollWheelSize = 48;

    private readonly Dictionary<int, double> itemHeightCache = [];
    private readonly List<double> columnHeights = [];

    private double itemWidth;
    private int columnCount;
    private double totalHeight;
    private Size extent;
    private Size viewport;
    private Point offset;
    private ScrollViewer? scrollOwner;

    [DependencyProperty(
        DefaultValue = 250d,
        PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private double desiredItemWidth;

    [DependencyProperty(
        DefaultValue = "default(Thickness)",
        PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private Thickness padding;

    [DependencyProperty(
        DefaultValue = 0d,
        PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private double horizontalSpacing;

    [DependencyProperty(
        DefaultValue = 0d,
        PropertyChangedCallback = nameof(OnLayoutPropertyChanged))]
    private double verticalSpacing;

    private static void OnLayoutPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var panel = (VirtualizingStaggeredPanel)d;
        panel.itemHeightCache.Clear();
        panel.InvalidateMeasure();
    }

    #region IScrollInfo Implementation

    /// <inheritdoc />
    public bool CanHorizontallyScroll { get; set; }

    /// <inheritdoc />
    public bool CanVerticallyScroll { get; set; }

    /// <inheritdoc />
    public double ExtentHeight => extent.Height;

    /// <inheritdoc />
    public double ExtentWidth => extent.Width;

    /// <inheritdoc />
    public double HorizontalOffset => offset.X;

    /// <inheritdoc />
    public double VerticalOffset => offset.Y;

    /// <inheritdoc />
    public double ViewportHeight => viewport.Height;

    /// <inheritdoc />
    public double ViewportWidth => viewport.Width;

    /// <inheritdoc />
    public ScrollViewer? ScrollOwner
    {
        get => scrollOwner;
        set => scrollOwner = value;
    }

    /// <inheritdoc />
    public void LineUp() => SetVerticalOffset(offset.Y - ScrollLineSize);

    /// <inheritdoc />
    public void LineDown() => SetVerticalOffset(offset.Y + ScrollLineSize);

    /// <inheritdoc />
    public void LineLeft() => SetHorizontalOffset(offset.X - ScrollLineSize);

    /// <inheritdoc />
    public void LineRight() => SetHorizontalOffset(offset.X + ScrollLineSize);

    /// <inheritdoc />
    public void PageUp() => SetVerticalOffset(offset.Y - viewport.Height);

    /// <inheritdoc />
    public void PageDown() => SetVerticalOffset(offset.Y + viewport.Height);

    /// <inheritdoc />
    public void PageLeft() => SetHorizontalOffset(offset.X - viewport.Width);

    /// <inheritdoc />
    public void PageRight() => SetHorizontalOffset(offset.X + viewport.Width);

    /// <inheritdoc />
    public void MouseWheelUp() => SetVerticalOffset(offset.Y - ScrollWheelSize);

    /// <inheritdoc />
    public void MouseWheelDown() => SetVerticalOffset(offset.Y + ScrollWheelSize);

    /// <inheritdoc />
    public void MouseWheelLeft() => SetHorizontalOffset(offset.X - ScrollWheelSize);

    /// <inheritdoc />
    public void MouseWheelRight() => SetHorizontalOffset(offset.X + ScrollWheelSize);

    /// <inheritdoc />
    public void SetHorizontalOffset(double newOffset)
    {
        newOffset = Math.Max(0, Math.Min(newOffset, extent.Width - viewport.Width));
        if (Math.Abs(offset.X - newOffset) > 0.001)
        {
            offset.X = newOffset;
            InvalidateMeasure();
            scrollOwner?.InvalidateScrollInfo();
        }
    }

    /// <inheritdoc />
    public void SetVerticalOffset(double newOffset)
    {
        newOffset = Math.Max(0, Math.Min(newOffset, extent.Height - viewport.Height));
        if (Math.Abs(offset.Y - newOffset) > 0.001)
        {
            offset.Y = newOffset;
            InvalidateMeasure();
            scrollOwner?.InvalidateScrollInfo();
        }
    }

    /// <inheritdoc />
    public Rect MakeVisible(Visual visual, Rect rectangle)
    {
        if (visual is not UIElement element)
        {
            return Rect.Empty;
        }

        var transform = visual.TransformToAncestor(this);
        var elementBounds = transform.TransformBounds(rectangle);

        if (elementBounds.Top < offset.Y)
        {
            SetVerticalOffset(elementBounds.Top);
        }
        else if (elementBounds.Bottom > offset.Y + viewport.Height)
        {
            SetVerticalOffset(elementBounds.Bottom - viewport.Height);
        }

        return rectangle;
    }

    #endregion

    /// <inheritdoc />
    protected override Size MeasureOverride(Size availableSize)
    {
        var itemsControl = ItemsControl.GetItemsOwner(this);
        if (itemsControl is null)
        {
            return MeasureNonVirtualized(availableSize);
        }

        var itemCount = itemsControl.Items.Count;
        if (itemCount == 0)
        {
            UpdateScrollInfo(availableSize, new Size(0, 0));
            return new Size(0, 0);
        }

        var generator = ItemContainerGenerator;
        if (generator is null)
        {
            return MeasureNonVirtualized(availableSize);
        }

        CalculateLayout(availableSize);

        columnHeights.Clear();
        for (var i = 0; i < columnCount; i++)
        {
            columnHeights.Add(0);
        }

        var visibleStart = offset.Y;
        var visibleEnd = offset.Y + availableSize.Height;

        var positions = new List<(int Index, int Column, double Top)>();

        for (var i = 0; i < itemCount; i++)
        {
            var columnIndex = GetShortestColumnIndex();
            var top = columnHeights[columnIndex];

            if (!itemHeightCache.TryGetValue(i, out var itemHeight))
            {
                itemHeight = 100;
            }

            var bottom = top + itemHeight;

            var isVisible = bottom >= visibleStart && top <= visibleEnd;

            if (isVisible || !itemHeightCache.ContainsKey(i))
            {
                positions.Add((i, columnIndex, top));
            }

            columnHeights[columnIndex] = bottom + VerticalSpacing;
        }

        totalHeight = columnHeights.Count > 0 ? columnHeights.Max() : 0;
        if (totalHeight > VerticalSpacing)
        {
            totalHeight -= VerticalSpacing;
        }

        var children = InternalChildren;
        var startPos = generator.GeneratorPositionFromIndex(positions.Count > 0 ? positions[0].Index : 0);

        using (generator.StartAt(startPos, GeneratorDirection.Forward, allowStartAtRealizedItem: true))
        {
            var childIndex = startPos.Offset == 0 ? startPos.Index : startPos.Index + 1;

            foreach (var (itemIndex, columnIndex, top) in positions)
            {
                var child = (UIElement)generator.GenerateNext(out var isNewlyRealized);

                if (isNewlyRealized)
                {
                    if (childIndex >= children.Count)
                    {
                        AddInternalChild(child);
                    }
                    else
                    {
                        InsertInternalChild(childIndex, child);
                    }

                    generator.PrepareItemContainer(child);
                }

                child.Measure(new Size(itemWidth, availableSize.Height));

                if (!itemHeightCache.ContainsKey(itemIndex) ||
                    Math.Abs(itemHeightCache[itemIndex] - child.DesiredSize.Height) > 0.001)
                {
                    itemHeightCache[itemIndex] = child.DesiredSize.Height;
                }

                childIndex++;
            }
        }

        CleanupUnusedContainers(positions.Select(p => p.Index).ToHashSet());

        var extentSize = new Size(
            availableSize.Width,
            totalHeight + Padding.Top + Padding.Bottom);

        UpdateScrollInfo(availableSize, extentSize);

        return availableSize;
    }

    /// <inheritdoc />
    protected override Size ArrangeOverride(Size finalSize)
    {
        var itemsControl = ItemsControl.GetItemsOwner(this);
        if (itemsControl is null)
        {
            return ArrangeNonVirtualized(finalSize);
        }

        var itemCount = itemsControl.Items.Count;
        if (itemCount == 0)
        {
            return finalSize;
        }

        CalculateLayout(finalSize);

        columnHeights.Clear();
        for (var i = 0; i < columnCount; i++)
        {
            columnHeights.Add(0);
        }

        var horizontalOffset = Padding.Left;
        var totalContentWidth = (itemWidth * columnCount) + (HorizontalSpacing * (columnCount - 1));

        switch (HorizontalAlignment)
        {
            case HorizontalAlignment.Right:
                horizontalOffset += finalSize.Width - totalContentWidth - Padding.Right;
                break;
            case HorizontalAlignment.Center:
                horizontalOffset += (finalSize.Width - totalContentWidth) / 2;
                break;
        }

        var itemPositions = new Dictionary<int, (int Column, double Top)>();

        for (var i = 0; i < itemCount; i++)
        {
            var columnIndex = GetShortestColumnIndex();
            var top = columnHeights[columnIndex];

            itemPositions[i] = (columnIndex, top);

            if (itemHeightCache.TryGetValue(i, out var cachedHeight))
            {
                columnHeights[columnIndex] = top + cachedHeight + VerticalSpacing;
            }
            else
            {
                columnHeights[columnIndex] = top + 100 + VerticalSpacing;
            }
        }

        var generator = ItemContainerGenerator;
        if (generator is null)
        {
            return ArrangeNonVirtualized(finalSize);
        }

        var children = InternalChildren;

        for (var childIndex = 0; childIndex < children.Count; childIndex++)
        {
            var child = children[childIndex];
            var itemIndex = generator.IndexFromGeneratorPosition(new GeneratorPosition(childIndex, 0));

            if (itemIndex < 0 || !itemPositions.TryGetValue(itemIndex, out var position))
            {
                continue;
            }

            var (columnIndex, top) = position;
            var left = horizontalOffset + (columnIndex * (itemWidth + HorizontalSpacing));
            var adjustedTop = Padding.Top + top - offset.Y;

            child.Arrange(new Rect(left, adjustedTop, itemWidth, child.DesiredSize.Height));
        }

        return finalSize;
    }

    private void CalculateLayout(Size availableSize)
    {
        var availableWidth = availableSize.Width - Padding.Left - Padding.Right;

        itemWidth = Math.Min(DesiredItemWidth, availableWidth);
        columnCount = Math.Max(1, (int)Math.Floor((availableWidth + HorizontalSpacing) / (itemWidth + HorizontalSpacing)));

        var totalWidth = (itemWidth * columnCount) + (HorizontalSpacing * (columnCount - 1));
        if (totalWidth > availableWidth && columnCount > 1)
        {
            columnCount--;
        }

        if (HorizontalAlignment == HorizontalAlignment.Stretch && columnCount > 0)
        {
            var occupiedSpacing = (columnCount - 1) * HorizontalSpacing;
            itemWidth = (availableWidth - occupiedSpacing) / columnCount;
        }
    }

    private int GetShortestColumnIndex()
    {
        if (columnHeights.Count == 0)
        {
            return 0;
        }

        var minIndex = 0;
        var minHeight = columnHeights[0];

        for (var i = 1; i < columnHeights.Count; i++)
        {
            if (columnHeights[i] < minHeight)
            {
                minIndex = i;
                minHeight = columnHeights[i];
            }
        }

        return minIndex;
    }

    private void CleanupUnusedContainers(HashSet<int> visibleIndices)
    {
        var generator = ItemContainerGenerator;
        var children = InternalChildren;

        for (var i = children.Count - 1; i >= 0; i--)
        {
            var position = new GeneratorPosition(i, 0);
            var itemIndex = generator.IndexFromGeneratorPosition(position);

            if (!visibleIndices.Contains(itemIndex))
            {
                generator.Remove(position, 1);
                RemoveInternalChildRange(i, 1);
            }
        }
    }

    private void UpdateScrollInfo(Size viewportSize, Size extentSize)
    {
        var changed = false;

        if (Math.Abs(viewport.Width - viewportSize.Width) > 0.001 ||
            Math.Abs(viewport.Height - viewportSize.Height) > 0.001)
        {
            viewport = viewportSize;
            changed = true;
        }

        if (Math.Abs(extent.Width - extentSize.Width) > 0.001 ||
            Math.Abs(extent.Height - extentSize.Height) > 0.001)
        {
            extent = extentSize;
            changed = true;
        }

        if (offset.Y > extent.Height - viewport.Height)
        {
            offset.Y = Math.Max(0, extent.Height - viewport.Height);
            changed = true;
        }

        if (changed)
        {
            scrollOwner?.InvalidateScrollInfo();
        }
    }

    private Size MeasureNonVirtualized(Size availableSize)
    {
        if (InternalChildren.Count == 0)
        {
            return new Size(0, 0);
        }

        CalculateLayout(availableSize);

        columnHeights.Clear();
        for (var i = 0; i < columnCount; i++)
        {
            columnHeights.Add(0);
        }

        foreach (UIElement child in InternalChildren)
        {
            var columnIndex = GetShortestColumnIndex();
            child.Measure(new Size(itemWidth, availableSize.Height));
            columnHeights[columnIndex] += child.DesiredSize.Height + VerticalSpacing;
        }

        var maxHeight = columnHeights.Count > 0 ? columnHeights.Max() : 0;
        if (maxHeight > VerticalSpacing)
        {
            maxHeight -= VerticalSpacing;
        }

        return new Size(availableSize.Width, maxHeight + Padding.Top + Padding.Bottom);
    }

    private Size ArrangeNonVirtualized(Size finalSize)
    {
        CalculateLayout(finalSize);

        columnHeights.Clear();
        for (var i = 0; i < columnCount; i++)
        {
            columnHeights.Add(0);
        }

        var horizontalOffset = Padding.Left;
        var totalContentWidth = (itemWidth * columnCount) + (HorizontalSpacing * (columnCount - 1));

        switch (HorizontalAlignment)
        {
            case HorizontalAlignment.Right:
                horizontalOffset += finalSize.Width - totalContentWidth - Padding.Right;
                break;
            case HorizontalAlignment.Center:
                horizontalOffset += (finalSize.Width - totalContentWidth) / 2;
                break;
        }

        foreach (UIElement child in InternalChildren)
        {
            var columnIndex = GetShortestColumnIndex();
            var left = horizontalOffset + (columnIndex * (itemWidth + HorizontalSpacing));
            var top = Padding.Top + columnHeights[columnIndex];

            child.Arrange(new Rect(left, top, itemWidth, child.DesiredSize.Height));

            columnHeights[columnIndex] += child.DesiredSize.Height + VerticalSpacing;
        }

        return finalSize;
    }
}
