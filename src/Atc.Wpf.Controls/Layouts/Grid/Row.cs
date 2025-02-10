namespace Atc.Wpf.Controls.Layouts.Grid;

public sealed class Row : Panel
{
    private ColLayoutType layoutStatus;
    private double maxChildDesiredHeight;
    private double totalAutoWidth;

    public static readonly DependencyProperty GutterProperty = DependencyProperty.Register(
        nameof(Gutter),
        typeof(double),
        typeof(Row),
        new PropertyMetadata(
            0.0,
            propertyChangedCallback: null,
            OnGutterCoerce),
        IsInRangeOfPosDoubleIncludeZero);

    private static object OnGutterCoerce(
        DependencyObject d,
        object baseValue)
        => IsInRangeOfPosDoubleIncludeZero(baseValue)
            ? baseValue
            : 0.0;

    public double Gutter
    {
        get => (double)GetValue(GutterProperty);
        set => SetValue(GutterProperty, value);
    }

    protected override Size MeasureOverride(
        Size availableSize)
    {
        var totalCellCount = 0;
        var totalRowCount = 1;
        var gutterHalf = Gutter / 2;
        totalAutoWidth = 0;

        foreach (var child in InternalChildren.OfType<Col>())
        {
            child.Margin = new Thickness(gutterHalf);
            child.Measure(availableSize);
            var childDesiredSize = child.DesiredSize;

            if (maxChildDesiredHeight < childDesiredSize.Height)
            {
                maxChildDesiredHeight = childDesiredSize.Height;
            }

            var cellCount = child.GetLayoutCellCount(layoutStatus);
            totalCellCount += cellCount;

            if (totalCellCount > ColLayout.ColMaxCellCount)
            {
                totalCellCount = cellCount;
                totalRowCount++;
            }

            if (cellCount == 0 || child.IsFixed)
            {
                totalAutoWidth += childDesiredSize.Width;
            }
        }

        return new Size(0, (maxChildDesiredHeight * totalRowCount) - Gutter);
    }

    protected override Size ArrangeOverride(
        Size finalSize)
    {
        var totalCellCount = 0;
        var gutterHalf = Gutter / 2;
        var itemWidth = (finalSize.Width - totalAutoWidth + Gutter) / ColLayout.ColMaxCellCount;
        itemWidth = System.Math.Max(0, itemWidth);

        var childBounds = new Rect(-gutterHalf, -gutterHalf, 0, maxChildDesiredHeight);
        layoutStatus = ColLayout.GetLayoutStatus(finalSize.Width);

        foreach (var child in InternalChildren.OfType<Col>())
        {
            var cellCount = child.GetLayoutCellCount(layoutStatus);
            totalCellCount += cellCount;

            var childWidth = cellCount > 0 ? cellCount * itemWidth : child.DesiredSize.Width;

            childBounds.Width = childWidth;
            childBounds.X += child.Offset * itemWidth;
            if (totalCellCount > ColLayout.ColMaxCellCount)
            {
                childBounds.X = -gutterHalf;
                childBounds.Y += maxChildDesiredHeight;
                totalCellCount = cellCount;
            }

            child.Arrange(childBounds);
            childBounds.X += childWidth;
        }

        return finalSize;
    }

    private static bool IsInRangeOfPosDoubleIncludeZero(
        object value)
    {
        var v = (double)value;
        return !(double.IsNaN(v) || double.IsInfinity(v)) && v >= 0;
    }
}