// ReSharper disable InvertIf
namespace Atc.Wpf.Controls.Layouts;

public class StaggeredPanel : Panel
{
    private double itemWidth;

    public static readonly DependencyProperty DesiredItemWidthProperty = DependencyProperty.Register(
        nameof(DesiredItemWidth),
        typeof(double),
        typeof(StaggeredPanel),
        new PropertyMetadata(
            250d,
            (sender, _) => ((StaggeredPanel)sender).InvalidateMeasure()));

    public double DesiredItemWidth
    {
        get => (double)GetValue(DesiredItemWidthProperty);
        set => SetValue(DesiredItemWidthProperty, value);
    }

    public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register(
        nameof(Padding),
        typeof(Thickness),
        typeof(StaggeredPanel),
        new PropertyMetadata(
            default(Thickness),
            (sender, _) => ((StaggeredPanel)sender).InvalidateMeasure()));

    public Thickness Padding
    {
        get => (Thickness)GetValue(PaddingProperty);
        set => SetValue(PaddingProperty, value);
    }

    public static readonly DependencyProperty HorizontalSpacingProperty = DependencyProperty.Register(
        nameof(HorizontalSpacing),
        typeof(double),
        typeof(StaggeredPanel),
        new PropertyMetadata(
            0d,
            (sender, _) => ((StaggeredPanel)sender).InvalidateMeasure()));

    public double HorizontalSpacing
    {
        get => (double)GetValue(HorizontalSpacingProperty);
        set => SetValue(HorizontalSpacingProperty, value);
    }

    public static readonly DependencyProperty VerticalSpacingProperty = DependencyProperty.Register(
        nameof(VerticalSpacing),
        typeof(double),
        typeof(StaggeredPanel),
        new PropertyMetadata(
            0d,
            (sender, _) => ((StaggeredPanel)sender).InvalidateMeasure()));

    public double VerticalSpacing
    {
        get => (double)GetValue(VerticalSpacingProperty);
        set => SetValue(VerticalSpacingProperty, value);
    }

    static StaggeredPanel()
    {
        HorizontalAlignmentProperty.OverrideMetadata(
            typeof(StaggeredPanel),
            new FrameworkPropertyMetadata(OnHorizontalAlignmentChanged));
    }

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
        var numItems = System.Math.Max(1, (int)System.Math.Floor(availableWidth / (itemWidth + HorizontalSpacing)));

        var totalWidth = itemWidth + ((numItems - 1) * (itemWidth + HorizontalSpacing));
        if (totalWidth > availableWidth)
        {
            numItems--;
        }
        else if (double.IsInfinity(availableWidth))
        {
            availableWidth = totalWidth;
        }

        if (HorizontalAlignment == HorizontalAlignment.Stretch)
        {
            var occupiedSpacing = (numItems - 1) * HorizontalSpacing;
            if (availableWidth < occupiedSpacing)
            {
                occupiedSpacing = availableWidth;
            }

            availableWidth -= occupiedSpacing;
            itemWidth = availableWidth / numItems;
        }

        var itemHeights = new double[numItems];
        var itemsPerColumn = new double[numItems];

        for (var i = 0; i < Children.Count; i++)
        {
            var itemIndex = GetItemIndex(itemHeights);

            var child = Children[i];
            child.Measure(new Size(itemWidth, availableHeight));
            var elementSize = child.DesiredSize;
            itemHeights[itemIndex] += elementSize.Height + (itemsPerColumn[itemIndex] > 0 ? VerticalSpacing : 0);
            itemsPerColumn[itemIndex]++;
        }

        var desiredHeight = itemHeights.Max();

        return new Size(availableWidth, desiredHeight);
    }

    protected override Size ArrangeOverride(
        Size finalSize)
    {
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
        var itemsPerColumn = new double[numColumns];

        for (var i = 0; i < Children.Count; i++)
        {
            var columnIndex = GetItemIndex(columnHeights);

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

    private static int GetItemIndex(
        IReadOnlyList<double> itemHeights)
    {
        var columnIndex = 0;
        var height = itemHeights[0];

        for (var j = 1; j < itemHeights.Count; j++)
        {
            if (itemHeights[j] < height)
            {
                columnIndex = j;
                height = itemHeights[j];
            }
        }

        return columnIndex;
    }
}