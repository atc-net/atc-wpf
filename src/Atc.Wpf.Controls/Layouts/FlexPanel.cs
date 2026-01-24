// ReSharper disable MemberCanBePrivate.Global
namespace Atc.Wpf.Controls.Layouts;

/// <summary>
/// A CSS Flexbox-inspired layout panel for WPF.
/// Supports direction, justify-content, align-items, wrap, gap,
/// and per-child grow/shrink/basis attached properties.
/// </summary>
[SuppressMessage("Design", "MA0051:Method is too long", Justification = "Complex layout algorithm.")]
public class FlexPanel : Panel
{
    #region Dependency Properties

    /// <summary>
    /// Identifies the Direction dependency property.
    /// </summary>
    public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register(
        nameof(Direction),
        typeof(FlexDirection),
        typeof(FlexPanel),
        new FrameworkPropertyMetadata(FlexDirection.Row, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

    /// <summary>
    /// Identifies the JustifyContent dependency property.
    /// </summary>
    public static readonly DependencyProperty JustifyContentProperty = DependencyProperty.Register(
        nameof(JustifyContent),
        typeof(FlexJustify),
        typeof(FlexPanel),
        new FrameworkPropertyMetadata(FlexJustify.Start, FrameworkPropertyMetadataOptions.AffectsArrange));

    /// <summary>
    /// Identifies the AlignItems dependency property.
    /// </summary>
    public static readonly DependencyProperty AlignItemsProperty = DependencyProperty.Register(
        nameof(AlignItems),
        typeof(FlexAlign),
        typeof(FlexPanel),
        new FrameworkPropertyMetadata(FlexAlign.Stretch, FrameworkPropertyMetadataOptions.AffectsArrange));

    /// <summary>
    /// Identifies the Wrap dependency property.
    /// </summary>
    public static readonly DependencyProperty WrapProperty = DependencyProperty.Register(
        nameof(Wrap),
        typeof(FlexWrap),
        typeof(FlexPanel),
        new FrameworkPropertyMetadata(FlexWrap.NoWrap, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

    /// <summary>
    /// Identifies the Gap dependency property.
    /// </summary>
    public static readonly DependencyProperty GapProperty = DependencyProperty.Register(
        nameof(Gap),
        typeof(double),
        typeof(FlexPanel),
        new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange),
        IsGapValid);

    /// <summary>
    /// Identifies the RowGap dependency property.
    /// </summary>
    public static readonly DependencyProperty RowGapProperty = DependencyProperty.Register(
        nameof(RowGap),
        typeof(double),
        typeof(FlexPanel),
        new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange),
        IsGapValid);

    /// <summary>
    /// Identifies the ColumnGap dependency property.
    /// </summary>
    public static readonly DependencyProperty ColumnGapProperty = DependencyProperty.Register(
        nameof(ColumnGap),
        typeof(double),
        typeof(FlexPanel),
        new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange),
        IsGapValid);

    #endregion

    #region Attached Properties

    /// <summary>
    /// Identifies the Grow attached property.
    /// </summary>
    public static readonly DependencyProperty GrowProperty = DependencyProperty.RegisterAttached(
        "Grow",
        typeof(double),
        typeof(FlexPanel),
        new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange),
        IsGrowShrinkValid);

    /// <summary>
    /// Identifies the Shrink attached property.
    /// </summary>
    public static readonly DependencyProperty ShrinkProperty = DependencyProperty.RegisterAttached(
        "Shrink",
        typeof(double),
        typeof(FlexPanel),
        new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange),
        IsGrowShrinkValid);

    /// <summary>
    /// Identifies the Basis attached property.
    /// </summary>
    public static readonly DependencyProperty BasisProperty = DependencyProperty.RegisterAttached(
        "Basis",
        typeof(double),
        typeof(FlexPanel),
        new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange),
        IsBasisValid);

    /// <summary>
    /// Identifies the AlignSelf attached property.
    /// </summary>
    public static readonly DependencyProperty AlignSelfProperty = DependencyProperty.RegisterAttached(
        "AlignSelf",
        typeof(FlexAlign),
        typeof(FlexPanel),
        new FrameworkPropertyMetadata(FlexAlign.Auto, FrameworkPropertyMetadataOptions.AffectsParentArrange));

    /// <summary>
    /// Gets the Grow value for a child element.
    /// </summary>
    public static double GetGrow(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (double)element.GetValue(GrowProperty);
    }

    /// <summary>
    /// Sets the Grow value for a child element.
    /// </summary>
    public static void SetGrow(
        DependencyObject element,
        double value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(GrowProperty, value);
    }

    /// <summary>
    /// Gets the Shrink value for a child element.
    /// </summary>
    public static double GetShrink(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (double)element.GetValue(ShrinkProperty);
    }

    /// <summary>
    /// Sets the Shrink value for a child element.
    /// </summary>
    public static void SetShrink(
        DependencyObject element,
        double value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(ShrinkProperty, value);
    }

    /// <summary>
    /// Gets the Basis value for a child element.
    /// </summary>
    public static double GetBasis(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (double)element.GetValue(BasisProperty);
    }

    /// <summary>
    /// Sets the Basis value for a child element.
    /// </summary>
    public static void SetBasis(
        DependencyObject element,
        double value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(BasisProperty, value);
    }

    /// <summary>
    /// Gets the AlignSelf value for a child element.
    /// </summary>
    public static FlexAlign GetAlignSelf(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (FlexAlign)element.GetValue(AlignSelfProperty);
    }

    /// <summary>
    /// Sets the AlignSelf value for a child element.
    /// </summary>
    public static void SetAlignSelf(
        DependencyObject element,
        FlexAlign value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(AlignSelfProperty, value);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the direction in which children are laid out.
    /// </summary>
    public FlexDirection Direction
    {
        get => (FlexDirection)GetValue(DirectionProperty);
        set => SetValue(DirectionProperty, value);
    }

    /// <summary>
    /// Gets or sets how children are justified along the main axis.
    /// </summary>
    public FlexJustify JustifyContent
    {
        get => (FlexJustify)GetValue(JustifyContentProperty);
        set => SetValue(JustifyContentProperty, value);
    }

    /// <summary>
    /// Gets or sets how children are aligned along the cross axis.
    /// </summary>
    public FlexAlign AlignItems
    {
        get => (FlexAlign)GetValue(AlignItemsProperty);
        set => SetValue(AlignItemsProperty, value);
    }

    /// <summary>
    /// Gets or sets whether children should wrap when they overflow.
    /// </summary>
    public FlexWrap Wrap
    {
        get => (FlexWrap)GetValue(WrapProperty);
        set => SetValue(WrapProperty, value);
    }

    /// <summary>
    /// Gets or sets the gap between children (both row and column).
    /// </summary>
    public double Gap
    {
        get => (double)GetValue(GapProperty);
        set => SetValue(GapProperty, value);
    }

    /// <summary>
    /// Gets or sets the gap between rows (cross-axis gap when wrapping).
    /// </summary>
    public double RowGap
    {
        get => (double)GetValue(RowGapProperty);
        set => SetValue(RowGapProperty, value);
    }

    /// <summary>
    /// Gets or sets the gap between columns (main-axis gap).
    /// </summary>
    public double ColumnGap
    {
        get => (double)GetValue(ColumnGapProperty);
        set => SetValue(ColumnGapProperty, value);
    }

    #endregion

    #region Validation

    private static bool IsGapValid(object value)
        => value is double gap && (double.IsNaN(gap) || gap >= 0);

    private static bool IsGrowShrinkValid(object value)
        => value is double v && v >= 0;

    private static bool IsBasisValid(object value)
        => value is double v && (double.IsNaN(v) || v >= 0);

    #endregion

    #region Layout Helpers

    private double GetMainGap()
    {
        var columnGap = ColumnGap;
        return double.IsNaN(columnGap) ? Gap : columnGap;
    }

    private double GetCrossGap()
    {
        var rowGap = RowGap;
        return double.IsNaN(rowGap) ? Gap : rowGap;
    }

    private bool IsHorizontal()
        => Direction is FlexDirection.Row or FlexDirection.RowReverse;

    private bool IsReversed()
        => Direction is FlexDirection.RowReverse or FlexDirection.ColumnReverse;

    private static double GetMainSize(
        Size size,
        bool isHorizontal)
        => isHorizontal ? size.Width : size.Height;

    private static double GetCrossSize(
        Size size,
        bool isHorizontal)
        => isHorizontal ? size.Height : size.Width;

    private static Size CreateSize(
        double main,
        double cross,
        bool isHorizontal)
        => isHorizontal ? new Size(main, cross) : new Size(cross, main);

    private double GetChildBasis(
        UIElement child,
        bool isHorizontal)
    {
        var basis = GetBasis(child);
        if (!double.IsNaN(basis))
        {
            return basis;
        }

        return GetMainSize(child.DesiredSize, isHorizontal);
    }

    #endregion

    #region FlexLine Helper Class

    private sealed class FlexLine
    {
        public List<UIElement> Children { get; } = [];

        public double TotalBasis { get; set; }

        public double TotalGrow { get; set; }

        public double TotalShrink { get; set; }

        public double CrossSize { get; set; }
    }

    #endregion

    #region MeasureOverride

    /// <inheritdoc />
    protected override Size MeasureOverride(Size availableSize)
    {
        var children = InternalChildren;
        if (children.Count == 0)
        {
            return default;
        }

        var isHorizontal = IsHorizontal();
        var mainGap = GetMainGap();
        var crossGap = GetCrossGap();
        var wrap = Wrap;

        var availableMain = GetMainSize(availableSize, isHorizontal);

        // Measure all children first
        foreach (UIElement child in children)
        {
            child.Measure(availableSize);
        }

        // Build flex lines
        var lines = BuildFlexLines(children, availableMain, mainGap, isHorizontal, wrap);

        // Calculate total size
        double totalMain = 0;
        double totalCross = 0;

        for (var i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            totalMain = System.Math.Max(totalMain, line.TotalBasis);
            totalCross += line.CrossSize;
            if (i > 0)
            {
                totalCross += crossGap;
            }
        }

        return CreateSize(totalMain, totalCross, isHorizontal);
    }

    private List<FlexLine> BuildFlexLines(
        UIElementCollection children,
        double availableMain,
        double mainGap,
        bool isHorizontal,
        FlexWrap wrap)
    {
        var lines = new List<FlexLine>();
        var currentLine = new FlexLine();

        foreach (UIElement child in children)
        {
            if (child.Visibility == Visibility.Collapsed)
            {
                continue;
            }

            var childBasis = GetChildBasis(child, isHorizontal);
            var childCross = GetCrossSize(child.DesiredSize, isHorizontal);
            var gapToAdd = currentLine.Children.Count > 0 ? mainGap : 0;

            // Check if we need to wrap
            if (wrap != FlexWrap.NoWrap &&
                currentLine.Children.Count > 0 &&
                !double.IsPositiveInfinity(availableMain) &&
                currentLine.TotalBasis + gapToAdd + childBasis > availableMain)
            {
                lines.Add(currentLine);
                currentLine = new FlexLine();
                gapToAdd = 0;
            }

            currentLine.Children.Add(child);
            currentLine.TotalBasis += childBasis + gapToAdd;
            currentLine.TotalGrow += GetGrow(child);
            currentLine.TotalShrink += GetShrink(child) * childBasis;
            currentLine.CrossSize = System.Math.Max(currentLine.CrossSize, childCross);
        }

        if (currentLine.Children.Count > 0)
        {
            lines.Add(currentLine);
        }

        return lines;
    }

    #endregion

    #region ArrangeOverride

    /// <inheritdoc />
    [SuppressMessage("", "MA0084:Local variable should not hide field", Justification = "OK.")]
    protected override Size ArrangeOverride(Size finalSize)
    {
        var children = InternalChildren;
        if (children.Count == 0)
        {
            return finalSize;
        }

        var isHorizontal = IsHorizontal();
        var isReversed = IsReversed();
        var mainGap = GetMainGap();
        var crossGap = GetCrossGap();
        var wrap = Wrap;
        var justifyContent = JustifyContent;
        var alignItems = AlignItems;

        var availableMain = GetMainSize(finalSize, isHorizontal);

        // Build flex lines
        var lines = BuildFlexLines(children, availableMain, mainGap, isHorizontal, wrap);

        // Handle WrapReverse
        if (wrap == FlexWrap.WrapReverse)
        {
            lines.Reverse();
        }

        // Calculate total cross size and remaining space
        double totalCrossSize = 0;
        for (var i = 0; i < lines.Count; i++)
        {
            totalCrossSize += lines[i].CrossSize;
            if (i > 0)
            {
                totalCrossSize += crossGap;
            }
        }

        // Arrange each line
        double crossOffset = 0;

        foreach (var line in lines)
        {
            ArrangeLine(line, crossOffset, availableMain, isHorizontal, isReversed, mainGap, justifyContent, alignItems);
            crossOffset += line.CrossSize + crossGap;
        }

        return finalSize;
    }

    private void ArrangeLine(
        FlexLine line,
        double crossOffset,
        double availableMain,
        bool isHorizontal,
        bool isReversed,
        double mainGap,
        FlexJustify justifyContent,
        FlexAlign alignItems)
    {
        var lineChildren = line.Children;
        var childCount = lineChildren.Count;
        if (childCount == 0)
        {
            return;
        }

        // Calculate the content size (sum of bases + gaps)
        double contentMain = 0;
        for (var i = 0; i < childCount; i++)
        {
            contentMain += GetChildBasis(lineChildren[i], isHorizontal);
            if (i > 0)
            {
                contentMain += mainGap;
            }
        }

        var freeSpace = availableMain - contentMain;
        var childSizes = new double[childCount];

        // Calculate sizes based on grow/shrink
        if (freeSpace > 0 && line.TotalGrow > 0)
        {
            // Distribute positive free space according to grow factors
            for (var i = 0; i < childCount; i++)
            {
                var child = lineChildren[i];
                var basis = GetChildBasis(child, isHorizontal);
                var grow = GetGrow(child);
                childSizes[i] = basis + ((grow / line.TotalGrow) * freeSpace);
            }
        }
        else if (freeSpace < 0 && line.TotalShrink > 0)
        {
            // Distribute negative free space according to shrink factors
            var shrinkAmount = -freeSpace;
            for (var i = 0; i < childCount; i++)
            {
                var child = lineChildren[i];
                var basis = GetChildBasis(child, isHorizontal);
                var shrink = GetShrink(child);
                var scaledShrink = shrink * basis;
                var reduction = (scaledShrink / line.TotalShrink) * shrinkAmount;
                childSizes[i] = System.Math.Max(0, basis - reduction);
            }
        }
        else
        {
            // No grow/shrink needed
            for (var i = 0; i < childCount; i++)
            {
                childSizes[i] = GetChildBasis(lineChildren[i], isHorizontal);
            }
        }

        // Calculate justify content positioning
        var (startOffset, itemSpacing) = CalculateJustifyOffsets(
            justifyContent,
            availableMain,
            childSizes,
            mainGap,
            childCount);

        // Reverse order if needed
        if (isReversed)
        {
            Array.Reverse(childSizes);
            var reversed = new List<UIElement>(lineChildren.Count);
            for (var i = lineChildren.Count - 1; i >= 0; i--)
            {
                reversed.Add(lineChildren[i]);
            }

            lineChildren = reversed;
        }

        // Arrange children
        var mainOffset = startOffset;

        for (var i = 0; i < childCount; i++)
        {
            var child = lineChildren[i];
            var mainSize = childSizes[i];
            var childCrossSize = GetCrossSize(child.DesiredSize, isHorizontal);

            // Determine alignment for this child
            var childAlign = GetAlignSelf(child);
            if (childAlign == FlexAlign.Auto)
            {
                childAlign = alignItems;
            }

            // Calculate cross position and size
            var (childCrossOffset, finalCrossSize) = CalculateAlignPosition(
                childAlign,
                crossOffset,
                line.CrossSize,
                childCrossSize);

            // Create the rect
            var rect = isHorizontal
                ? new Rect(mainOffset, childCrossOffset, mainSize, finalCrossSize)
                : new Rect(childCrossOffset, mainOffset, finalCrossSize, mainSize);

            child.Arrange(rect);

            mainOffset += mainSize + itemSpacing;
        }
    }

    private static (double StartOffset, double ItemSpacing) CalculateJustifyOffsets(
        FlexJustify justify,
        double availableMain,
        double[] childSizes,
        double mainGap,
        int childCount)
    {
        var totalChildSize = childSizes.Sum();
        var totalGaps = System.Math.Max(0, childCount - 1) * mainGap;
        var freeSpace = availableMain - totalChildSize - totalGaps;

        return justify switch
        {
            FlexJustify.Start => (0, mainGap),
            FlexJustify.End => (System.Math.Max(0, freeSpace), mainGap),
            FlexJustify.Center => (System.Math.Max(0, freeSpace / 2), mainGap),
            FlexJustify.SpaceBetween => childCount <= 1
                ? (0, mainGap)
                : (0, mainGap + (System.Math.Max(0, freeSpace) / (childCount - 1))),
            FlexJustify.SpaceAround => childCount == 0
                ? (0, mainGap)
                : (System.Math.Max(0, freeSpace / (childCount * 2)), mainGap + (System.Math.Max(0, freeSpace) / childCount)),
            FlexJustify.SpaceEvenly => childCount == 0
                ? (0, mainGap)
                : (System.Math.Max(0, freeSpace / (childCount + 1)), System.Math.Max(mainGap, freeSpace / (childCount + 1))),
            _ => (0, mainGap),
        };
    }

    private static (double Offset, double Size) CalculateAlignPosition(
        FlexAlign align,
        double lineOffset,
        double lineSize,
        double childSize)
        => align switch
        {
            FlexAlign.Start => (lineOffset, childSize),
            FlexAlign.End => (lineOffset + lineSize - childSize, childSize),
            FlexAlign.Center => (lineOffset + ((lineSize - childSize) / 2), childSize),
            FlexAlign.Stretch => (lineOffset, lineSize),
            FlexAlign.Baseline => (lineOffset, childSize), // Simplified - treat as Start
            _ => (lineOffset, lineSize), // Auto defaults to Stretch
        };

    #endregion
}