// ReSharper disable MemberCanBePrivate.Global
namespace Atc.Wpf.Controls.Layouts;

/// <summary>
/// A CSS Flexbox-inspired layout panel for WPF.
/// Supports direction, justify-content, align-items, wrap, gap,
/// and per-child grow/shrink/basis attached properties.
/// </summary>
[SuppressMessage("Design", "MA0051:Method is too long", Justification = "Complex layout algorithm.")]
public partial class FlexPanel : Panel
{
    #region Dependency Properties

    /// <summary>
    /// Gets or sets the direction in which children are laid out.
    /// </summary>
    [DependencyProperty(
        DefaultValue = FlexDirection.Row,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange)]
    private FlexDirection direction;

    /// <summary>
    /// Gets or sets how children are justified along the main axis.
    /// </summary>
    [DependencyProperty(
        DefaultValue = FlexJustify.Start,
        Flags = FrameworkPropertyMetadataOptions.AffectsArrange)]
    private FlexJustify justifyContent;

    /// <summary>
    /// Gets or sets how children are aligned along the cross axis.
    /// </summary>
    [DependencyProperty(
        DefaultValue = FlexAlign.Stretch,
        Flags = FrameworkPropertyMetadataOptions.AffectsArrange)]
    private FlexAlign alignItems;

    /// <summary>
    /// Gets or sets whether children should wrap when they overflow.
    /// </summary>
    [DependencyProperty(
        DefaultValue = FlexWrap.NoWrap,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange)]
    private FlexWrap wrap;

    /// <summary>
    /// Gets or sets the gap between children (both row and column).
    /// </summary>
    [DependencyProperty(
        DefaultValue = 0.0,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange,
        ValidateValueCallback = nameof(IsGapValid))]
    private double gap;

    /// <summary>
    /// Gets or sets the gap between rows (cross-axis gap when wrapping).
    /// </summary>
    [DependencyProperty(
        DefaultValue = double.NaN,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange,
        ValidateValueCallback = nameof(IsGapValid))]
    private double rowGap;

    /// <summary>
    /// Gets or sets the gap between columns (main-axis gap).
    /// </summary>
    [DependencyProperty(
        DefaultValue = double.NaN,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange,
        ValidateValueCallback = nameof(IsGapValid))]
    private double columnGap;

    #endregion

    #region Attached Properties

    // The Get/Set methods of the attached properties are hand-written below: each has its own documentation,
    // which the generator cannot reproduce.
    [AttachedProperty(
        DefaultValue = 0.0,
        Flags = FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange,
        ValidateValueCallback = nameof(IsGrowShrinkValid),
        GenerateClrAccessors = false)]
    private static double grow;

    [AttachedProperty(
        DefaultValue = 1.0,
        Flags = FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange,
        ValidateValueCallback = nameof(IsGrowShrinkValid),
        GenerateClrAccessors = false)]
    private static double shrink;

    [AttachedProperty(
        DefaultValue = double.NaN,
        Flags = FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange,
        ValidateValueCallback = nameof(IsBasisValid),
        GenerateClrAccessors = false)]
    private static double basis;

    [AttachedProperty(
        DefaultValue = FlexAlign.Auto,
        Flags = FrameworkPropertyMetadataOptions.AffectsParentArrange,
        GenerateClrAccessors = false)]
    private static FlexAlign alignSelf;

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
        var mainGap = ColumnGap;
        return double.IsNaN(mainGap) ? Gap : mainGap;
    }

    private double GetCrossGap()
    {
        var crossGap = RowGap;
        return double.IsNaN(crossGap) ? Gap : crossGap;
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
        var childBasis = GetBasis(child);
        if (!double.IsNaN(childBasis))
        {
            return childBasis;
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
        var wrapMode = Wrap;

        var availableMain = GetMainSize(availableSize, isHorizontal);

        // Measure all children first
        foreach (UIElement child in children)
        {
            child.Measure(availableSize);
        }

        // Build flex lines
        var lines = BuildFlexLines(children, availableMain, mainGap, isHorizontal, wrapMode);

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
        var wrapMode = Wrap;
        var justify = JustifyContent;
        var align = AlignItems;

        var availableMain = GetMainSize(finalSize, isHorizontal);

        // Build flex lines
        var lines = BuildFlexLines(children, availableMain, mainGap, isHorizontal, wrapMode);

        // Handle WrapReverse
        if (wrapMode == FlexWrap.WrapReverse)
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
            ArrangeLine(line, crossOffset, availableMain, isHorizontal, isReversed, mainGap, justify, align);
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
                var childBasis = GetChildBasis(child, isHorizontal);
                var childGrow = GetGrow(child);
                childSizes[i] = childBasis + ((childGrow / line.TotalGrow) * freeSpace);
            }
        }
        else if (freeSpace < 0 && line.TotalShrink > 0)
        {
            // Distribute negative free space according to shrink factors
            var shrinkAmount = -freeSpace;
            for (var i = 0; i < childCount; i++)
            {
                var child = lineChildren[i];
                var childBasis = GetChildBasis(child, isHorizontal);
                var childShrink = GetShrink(child);
                var scaledShrink = childShrink * childBasis;
                var reduction = (scaledShrink / line.TotalShrink) * shrinkAmount;
                childSizes[i] = System.Math.Max(0, childBasis - reduction);
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