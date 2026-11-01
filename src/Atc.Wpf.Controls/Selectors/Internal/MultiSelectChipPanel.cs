#pragma warning disable IDE0130
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// Lays out the chips of a <see cref="MultiSelectComboBox"/> or a <see cref="TokenBox"/>: on one line with a trailing
/// "+N" element for the chips that do not fit (or are beyond <see cref="MaxVisibleChips"/>), or wrapped over as many
/// lines as needed. An optional <see cref="TrailingElement"/> (the text box of a <see cref="TokenBox"/>) follows the
/// chips and takes the rest of the (last) line.
/// </summary>
/// <remarks>
/// The children are the chips in display order, then the "+N" element, then the trailing element (when set).
/// <see cref="TotalCount"/> can be larger than the number of chip children (the owner only creates as many chips as
/// can be shown); in wrapping mode the "+N" element then follows the chips. Hidden chips are arranged to an empty
/// rectangle, so they neither render nor take hits.
/// </remarks>
internal sealed class MultiSelectChipPanel : Panel
{
    private int? maxVisibleChips;
    private bool isWrapping;
    private int totalCount;
    private UIElement? trailingElement;
    private double minTrailingWidth = 40;

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
    /// Gets or sets a value indicating whether the chips wrap over several lines (no "+N" unless fewer chip children
    /// than <see cref="TotalCount"/> exist).
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
    /// Gets or sets the number of chips, shown or not.
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
    /// Gets or sets the element placed after the chips and the "+N" element, stretched over the rest of the line.
    /// </summary>
    public UIElement? TrailingElement
    {
        get => trailingElement;
        set
        {
            if (ReferenceEquals(trailingElement, value))
            {
                return;
            }

            if (trailingElement is not null)
            {
                InternalChildren.Remove(trailingElement);
            }

            trailingElement = value;
            if (value is not null)
            {
                InternalChildren.Add(value);
            }

            InvalidateMeasure();
        }
    }

    /// <summary>
    /// Gets or sets the least width the <see cref="TrailingElement"/> gets; with less room left on a wrapped line it
    /// moves to a new line.
    /// </summary>
    public double MinTrailingWidth
    {
        get => minTrailingWidth;
        set
        {
            minTrailingWidth = value;
            InvalidateMeasure();
        }
    }

    /// <summary>
    /// Gets or sets the callback that updates the "+N" element for a number of hidden chips (0 = none hidden).
    /// </summary>
    public Action<int>? UpdateOverflow { get; set; }

    /// <summary>
    /// Gets or sets the factory of the panel's automation peer; <see langword="null"/> for none.
    /// </summary>
    public Func<AutomationPeer?>? AutomationPeerFactory { get; set; }

    /// <summary>
    /// Gets the number of chips shown after the last measure.
    /// </summary>
    public int VisibleCount { get; private set; }

    /// <summary>
    /// Gets the number of chips not shown after the last measure.
    /// </summary>
    public int HiddenCount { get; private set; }

    /// <summary>
    /// Gets the number of chip children.
    /// </summary>
    public int ChipCount
        => System.Math.Max(0, InternalChildren.Count - 1 - (trailingElement is null ? 0 : 1));

    /// <summary>
    /// Gets the "+N" element.
    /// </summary>
    public UIElement? OverflowElement
    {
        get
        {
            var index = InternalChildren.Count - 1 - (trailingElement is null ? 0 : 1);
            return index >= 0 ? InternalChildren[index] : null;
        }
    }

    /// <summary>
    /// Removes every chip child (keeps the "+N" and trailing elements).
    /// </summary>
    public void ClearChips()
    {
        var count = ChipCount;
        if (count > 0)
        {
            InternalChildren.RemoveRange(0, count);
        }
    }

    /// <summary>
    /// Appends a chip child after the existing chips.
    /// </summary>
    public void AddChip(UIElement chip)
        => InternalChildren.Insert(ChipCount, chip);

    /// <inheritdoc />
    protected override AutomationPeer? OnCreateAutomationPeer()
        => AutomationPeerFactory?.Invoke() ?? base.OnCreateAutomationPeer();

    /// <inheritdoc />
    protected override Size MeasureOverride(Size availableSize)
    {
        var chipCount = ChipCount;
        var childConstraint = new Size(double.PositiveInfinity, availableSize.Height);
        for (var i = 0; i < chipCount; i++)
        {
            InternalChildren[i].Measure(IsWrapping ? new Size(availableSize.Width, availableSize.Height) : childConstraint);
        }

        trailingElement?.Measure(new Size(double.PositiveInfinity, availableSize.Height));

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
            return finalSize;
        }

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
            x += size.Width;
        }
        else
        {
            overflow?.Arrange(default);
        }

        if (trailingElement is not null)
        {
            var size = trailingElement.DesiredSize;
            var width = System.Math.Max(TrailingWidth, finalSize.Width - x);
            trailingElement.Arrange(new Rect(x, (finalSize.Height - size.Height) / 2, width, size.Height));
        }

        return finalSize;
    }

    private double TrailingWidth
        => trailingElement is null
            ? 0
            : System.Math.Max(MinTrailingWidth, trailingElement.DesiredSize.Width);

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

        var trailingWidth = TrailingWidth;
        var visible = limit;
        var width = 0d;
        for (; visible >= 0; visible--)
        {
            var hidden = TotalCount - visible;
            width = widths[visible] + MeasureOverflow(overflow, hidden) + trailingWidth;
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

        if (trailingElement is not null)
        {
            height = System.Math.Max(height, trailingElement.DesiredSize.Height);
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
        VisibleCount = System.Math.Min(chipCount, TotalCount);
        HiddenCount = System.Math.Max(0, TotalCount - VisibleCount);
        var overflow = OverflowElement;
        if (MeasureOverflow(overflow, HiddenCount) <= 0)
        {
            overflow?.Measure(default);
        }

        var lines = LayOutWrapped(availableSize.Width, chipCount);
        var width = 0d;
        var height = 0d;
        foreach (var line in lines)
        {
            width = System.Math.Max(width, line.Width);
            height += line.Height;
        }

        return new Size(
            double.IsPositiveInfinity(availableSize.Width) ? width : System.Math.Min(width, availableSize.Width),
            height);
    }

    private void ArrangeWrapped(
        Size finalSize,
        int chipCount)
    {
        for (var i = VisibleCount; i < chipCount; i++)
        {
            InternalChildren[i].Arrange(default);
        }

        var overflow = OverflowElement;
        if (HiddenCount <= 0)
        {
            overflow?.Arrange(default);
        }

        var y = 0d;
        foreach (var line in LayOutWrapped(finalSize.Width, chipCount))
        {
            foreach (var (element, x, width) in line.Items)
            {
                var size = element.DesiredSize;
                element.Arrange(new Rect(x, y + ((line.Height - size.Height) / 2), width, size.Height));
            }

            y += line.Height;
        }
    }

    /// <summary>
    /// Splits the shown chips, the "+N" element and the trailing element into lines.
    /// </summary>
    private List<WrappedLine> LayOutWrapped(
        double availableWidth,
        int chipCount)
    {
        var elements = new List<UIElement>();
        for (var i = 0; i < VisibleCount && i < chipCount; i++)
        {
            elements.Add(InternalChildren[i]);
        }

        if (HiddenCount > 0 && OverflowElement is { } overflow)
        {
            elements.Add(overflow);
        }

        var lines = new List<WrappedLine>();
        var current = new WrappedLine();
        foreach (var element in elements)
        {
            var size = element.DesiredSize;
            if (current.Width > 0 && current.Width + size.Width > availableWidth)
            {
                lines.Add(current);
                current = new WrappedLine();
            }

            current.Items.Add((element, current.Width, size.Width));
            current.Width += size.Width;
            current.Height = System.Math.Max(current.Height, size.Height);
        }

        if (trailingElement is not null)
        {
            var trailingWidth = TrailingWidth;
            if (current.Width > 0 && current.Width + trailingWidth > availableWidth)
            {
                lines.Add(current);
                current = new WrappedLine();
            }

            var width = double.IsPositiveInfinity(availableWidth)
                ? trailingWidth
                : System.Math.Max(trailingWidth, availableWidth - current.Width);
            current.Items.Add((trailingElement, current.Width, width));
            current.Width += trailingWidth;
            current.Height = System.Math.Max(current.Height, trailingElement.DesiredSize.Height);
        }

        if (current.Items.Count > 0)
        {
            lines.Add(current);
        }

        return lines;
    }

    private sealed class WrappedLine
    {
        public List<(UIElement Element, double X, double Width)> Items { get; } = [];

        public double Width { get; set; }

        public double Height { get; set; }
    }
}