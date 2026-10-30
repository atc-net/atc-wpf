#pragma warning disable IDE0130
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// Lays out the chips of a <see cref="MultiSelectComboBox"/>: on one line with a trailing "+N" element for the chips that
/// do not fit (or are beyond <see cref="MaxVisibleChips"/>), or wrapped over as many lines as needed.
/// </summary>
/// <remarks>
/// The last child is the "+N" element; the others are the chips in display order. <see cref="TotalCount"/> can be larger
/// than the number of chip children (the control only creates as many chips as can be shown on one line).
/// Hidden chips are arranged to an empty rectangle, so they neither render nor take hits.
/// </remarks>
internal sealed class MultiSelectChipPanel : Panel
{
    private int? maxVisibleChips;
    private bool isWrapping;
    private int totalCount;

    /// <summary>
    /// Gets or sets the most chips shown on one line; <see langword="null"/> shows as many as fit.
    /// </summary>
    public int? MaxVisibleChips
    {
        get => maxVisibleChips;
        set
        {
            maxVisibleChips = value;
            InvalidateMeasure();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the chips wrap over several lines (no "+N").
    /// </summary>
    public bool IsWrapping
    {
        get => isWrapping;
        set
        {
            isWrapping = value;
            InvalidateMeasure();
        }
    }

    /// <summary>
    /// Gets or sets the number of ticked items, shown or not.
    /// </summary>
    public int TotalCount
    {
        get => totalCount;
        set
        {
            totalCount = value;
            InvalidateMeasure();
        }
    }

    /// <summary>
    /// Gets or sets the callback that updates the "+N" element for a number of hidden chips (0 = none hidden).
    /// </summary>
    public Action<int>? UpdateOverflow { get; set; }

    /// <summary>
    /// Gets the number of chips shown after the last measure.
    /// </summary>
    public int VisibleCount { get; private set; }

    /// <summary>
    /// Gets the number of ticked items not shown as a chip after the last measure.
    /// </summary>
    public int HiddenCount { get; private set; }

    private int ChipCount => System.Math.Max(0, InternalChildren.Count - 1);

    private UIElement? OverflowElement
        => InternalChildren.Count > 0
            ? InternalChildren[InternalChildren.Count - 1]
            : null;

    /// <inheritdoc />
    protected override Size MeasureOverride(Size availableSize)
    {
        var chipCount = ChipCount;
        var childConstraint = new Size(double.PositiveInfinity, availableSize.Height);
        for (var i = 0; i < chipCount; i++)
        {
            InternalChildren[i].Measure(IsWrapping ? new Size(availableSize.Width, availableSize.Height) : childConstraint);
        }

        return IsWrapping
            ? MeasureWrapped(availableSize, chipCount)
            : MeasureOneLine(availableSize, chipCount);
    }

    /// <inheritdoc />
    protected override Size ArrangeOverride(Size finalSize)
    {
        var chipCount = ChipCount;
        var overflow = OverflowElement;

        if (IsWrapping)
        {
            ArrangeWrapped(finalSize, chipCount);
        }
        else
        {
            var x = 0d;
            for (var i = 0; i < VisibleCount && i < chipCount; i++)
            {
                var child = InternalChildren[i];
                var size = child.DesiredSize;
                child.Arrange(new Rect(x, (finalSize.Height - size.Height) / 2, size.Width, size.Height));
                x += size.Width;
            }

            for (var i = VisibleCount; i < chipCount; i++)
            {
                InternalChildren[i].Arrange(default);
            }

            if (overflow is not null && HiddenCount > 0)
            {
                var size = overflow.DesiredSize;
                overflow.Arrange(new Rect(x, (finalSize.Height - size.Height) / 2, size.Width, size.Height));
                return finalSize;
            }
        }

        overflow?.Arrange(default);
        return finalSize;
    }

    private Size MeasureOneLine(
        Size availableSize,
        int chipCount)
    {
        var overflow = OverflowElement;
        var limit = System.Math.Min(chipCount, TotalCount);
        if (MaxVisibleChips is { } max)
        {
            limit = System.Math.Min(limit, System.Math.Max(0, max));
        }

        var widths = new double[limit + 1];
        for (var i = 0; i < limit; i++)
        {
            widths[i + 1] = widths[i] + InternalChildren[i].DesiredSize.Width;
        }

        var visible = limit;
        var width = 0d;
        for (; visible >= 0; visible--)
        {
            var hidden = TotalCount - visible;
            width = widths[visible] + MeasureOverflow(overflow, hidden);
            if (width <= availableSize.Width || visible == 0)
            {
                break;
            }
        }

        visible = System.Math.Max(0, visible);
        VisibleCount = visible;
        HiddenCount = TotalCount - visible;

        var height = 0d;
        for (var i = 0; i < visible; i++)
        {
            height = System.Math.Max(height, InternalChildren[i].DesiredSize.Height);
        }

        if (overflow is not null && HiddenCount > 0)
        {
            height = System.Math.Max(height, overflow.DesiredSize.Height);
        }

        return new Size(
            double.IsPositiveInfinity(availableSize.Width) ? width : System.Math.Min(width, availableSize.Width),
            height);
    }

    private double MeasureOverflow(
        UIElement? overflow,
        int hidden)
    {
        UpdateOverflow?.Invoke(hidden);
        if (overflow is null || hidden <= 0)
        {
            return 0;
        }

        overflow.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        return overflow.DesiredSize.Width;
    }

    private Size MeasureWrapped(
        Size availableSize,
        int chipCount)
    {
        UpdateOverflow?.Invoke(0);
        OverflowElement?.Measure(default);

        var lineWidth = 0d;
        var lineHeight = 0d;
        var width = 0d;
        var height = 0d;
        for (var i = 0; i < chipCount; i++)
        {
            var size = InternalChildren[i].DesiredSize;
            if (lineWidth > 0 && lineWidth + size.Width > availableSize.Width)
            {
                width = System.Math.Max(width, lineWidth);
                height += lineHeight;
                lineWidth = 0;
                lineHeight = 0;
            }

            lineWidth += size.Width;
            lineHeight = System.Math.Max(lineHeight, size.Height);
        }

        width = System.Math.Max(width, lineWidth);
        height += lineHeight;
        VisibleCount = chipCount;
        HiddenCount = System.Math.Max(0, TotalCount - chipCount);
        return new Size(
            double.IsPositiveInfinity(availableSize.Width) ? width : System.Math.Min(width, availableSize.Width),
            height);
    }

    private void ArrangeWrapped(
        Size finalSize,
        int chipCount)
    {
        var x = 0d;
        var y = 0d;
        var lineHeight = 0d;
        for (var i = 0; i < chipCount; i++)
        {
            var child = InternalChildren[i];
            var size = child.DesiredSize;
            if (x > 0 && x + size.Width > finalSize.Width)
            {
                x = 0;
                y += lineHeight;
                lineHeight = 0;
            }

            child.Arrange(new Rect(x, y, size.Width, size.Height));
            x += size.Width;
            lineHeight = System.Math.Max(lineHeight, size.Height);
        }
    }
}