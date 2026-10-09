// ReSharper disable LocalVariableHidesMember
namespace Atc.Wpf.Controls.Layouts;

/// <summary>A panel that arranges its children with uniform spacing, horizontally or vertically, with optional wrapping.</summary>
[SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
public sealed class UniformSpacingPanel : Panel
{
    /// <summary>Identifies the <see cref="Orientation"/> dependency property.</summary>
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation),
        typeof(Orientation),
        typeof(UniformSpacingPanel),
        new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure, OnOrientationChanged));

    /// <summary>Identifies the <see cref="ChildWrapping"/> dependency property.</summary>
    public static readonly DependencyProperty ChildWrappingProperty = DependencyProperty.Register(
        nameof(ChildWrapping),
        typeof(VisualWrappingType),
        typeof(UniformSpacingPanel),
        new FrameworkPropertyMetadata(default(VisualWrappingType), FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>Identifies the <see cref="Spacing"/> dependency property.</summary>
    public static readonly DependencyProperty SpacingProperty = DependencyProperty.Register(
        nameof(Spacing),
        typeof(double),
        typeof(UniformSpacingPanel),
        new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure),
        IsSpacingValid);

    /// <summary>Identifies the <see cref="HorizontalSpacing"/> dependency property.</summary>
    public static readonly DependencyProperty HorizontalSpacingProperty = DependencyProperty.Register(
        nameof(HorizontalSpacing),
        typeof(double),
        typeof(UniformSpacingPanel),
        new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure),
        IsSpacingValid);

    /// <summary>Identifies the <see cref="VerticalSpacing"/> dependency property.</summary>
    public static readonly DependencyProperty VerticalSpacingProperty = DependencyProperty.Register(
        nameof(VerticalSpacing),
        typeof(double),
        typeof(UniformSpacingPanel),
        new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure),
        IsSpacingValid);

    /// <summary>Identifies the <see cref="ItemWidth"/> dependency property.</summary>
    public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register(
        nameof(ItemWidth),
        typeof(double),
        typeof(UniformSpacingPanel),
        new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure),
        IsWidthHeightValid);

    /// <summary>Identifies the <see cref="ItemHeight"/> dependency property.</summary>
    public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register(
        nameof(ItemHeight),
        typeof(double),
        typeof(UniformSpacingPanel),
        new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure),
        IsWidthHeightValid);

    /// <summary>Identifies the <see cref="ItemHorizontalAlignment"/> dependency property.</summary>
    public static readonly DependencyProperty ItemHorizontalAlignmentProperty = DependencyProperty.Register(
        nameof(ItemHorizontalAlignment),
        typeof(HorizontalAlignment?),
        typeof(UniformSpacingPanel),
        new FrameworkPropertyMetadata(HorizontalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>Identifies the <see cref="ItemVerticalAlignment"/> dependency property.</summary>
    public static readonly DependencyProperty ItemVerticalAlignmentProperty = DependencyProperty.Register(
        nameof(ItemVerticalAlignment),
        typeof(VerticalAlignment?),
        typeof(UniformSpacingPanel),
        new FrameworkPropertyMetadata(VerticalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsMeasure));

    private Orientation orientation;

    /// <summary>Gets or sets the layout direction.</summary>
    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>Gets or sets how children wrap when they run out of space.</summary>
    public VisualWrappingType ChildWrapping
    {
        get => (VisualWrappingType)GetValue(ChildWrappingProperty);
        set => SetValue(ChildWrappingProperty, value);
    }

    /// <summary>Gets or sets the uniform spacing between items; sets both the horizontal and vertical spacing.</summary>
    public double Spacing
    {
        get => (double)GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    /// <summary>Gets or sets the horizontal gap between items.</summary>
    public double HorizontalSpacing
    {
        get => (double)GetValue(HorizontalSpacingProperty);
        set => SetValue(HorizontalSpacingProperty, value);
    }

    /// <summary>Gets or sets the vertical gap between items.</summary>
    public double VerticalSpacing
    {
        get => (double)GetValue(VerticalSpacingProperty);
        set => SetValue(VerticalSpacingProperty, value);
    }

    /// <summary>Gets or sets a fixed width for all items.</summary>
    public double ItemWidth
    {
        get => (double)GetValue(ItemWidthProperty);
        set => SetValue(ItemWidthProperty, value);
    }

    /// <summary>Gets or sets a fixed height for all items.</summary>
    public double ItemHeight
    {
        get => (double)GetValue(ItemHeightProperty);
        set => SetValue(ItemHeightProperty, value);
    }

    /// <summary>Gets or sets the horizontal alignment of the children.</summary>
    public HorizontalAlignment? ItemHorizontalAlignment
    {
        get => (HorizontalAlignment?)GetValue(ItemHorizontalAlignmentProperty);
        set => SetValue(ItemHorizontalAlignmentProperty, value);
    }

    /// <summary>Gets or sets the vertical alignment of the children.</summary>
    public VerticalAlignment? ItemVerticalAlignment
    {
        get => (VerticalAlignment?)GetValue(ItemVerticalAlignmentProperty);
        set => SetValue(ItemVerticalAlignmentProperty, value);
    }

    private static void OnOrientationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var p = (UniformSpacingPanel)d;
        p.orientation = (Orientation)e.NewValue;
    }

    private static bool IsWidthHeightValid(object value)
    {
        var v = (double)value;
        return double.IsNaN(v) || (v >= 0.0d && !double.IsPositiveInfinity(v));
    }

    private static bool IsSpacingValid(object value)
        => value is double spacing && (double.IsNaN(spacing) || spacing >= 0);

    private void ArrangeWrapLine(
        double v,
        double lineV,
        int start,
        int end,
        bool useItemU,
        double itemU,
        double space)
    {
        double u = 0;
        var isHorizontal = orientation == Orientation.Horizontal;

        var children = InternalChildren;
        for (var i = start; i < end; i++)
        {
            var child = children[i];
            if (child is null)
            {
                continue;
            }

            var childSize = new PanelUvSize(orientation, child.DesiredSize);
            var layoutSlotU = useItemU
                ? itemU
                : childSize.U;

            child.Arrange(isHorizontal
                ? new Rect(u, v, layoutSlotU, lineV)
                : new Rect(v, u, lineV, layoutSlotU));

            if (layoutSlotU > 0)
            {
                u += layoutSlotU + space;
            }
        }
    }

    private void ArrangeLine(
        double lineV,
        bool useItemU,
        double itemU,
        double space)
    {
        double u = 0;
        var isHorizontal = orientation == Orientation.Horizontal;

        var children = InternalChildren;
        for (var i = 0; i < children.Count; i++)
        {
            var child = children[i];
            if (child is null)
            {
                continue;
            }

            var childSize = new PanelUvSize(orientation, child.DesiredSize);
            var layoutSlotU = useItemU
                ? itemU
                : childSize.U;

            child.Arrange(isHorizontal
                ? new Rect(u, 0, layoutSlotU, lineV)
                : new Rect(0, u, lineV, layoutSlotU));

            if (layoutSlotU > 0)
            {
                u += layoutSlotU + space;
            }
        }
    }

    /// <inheritdoc />
    [SuppressMessage("", "MA0084:Local variable should not hide field", Justification = "OK.")]
    [SuppressMessage("", "S1117:Local variable should not hide field", Justification = "OK.")]
    protected override Size MeasureOverride(Size availableSize)
    {
        var curLineSize = new PanelUvSize(orientation);
        var panelSize = new PanelUvSize(orientation);
        var uvConstraint = new PanelUvSize(orientation, availableSize);
        var itemWidthSet = !double.IsNaN(ItemWidth);
        var itemHeightSet = !double.IsNaN(ItemHeight);
        var itemHorizontalAlignment = ItemHorizontalAlignment;
        var itemVerticalAlignment = ItemVerticalAlignment;
        var itemHorizontalAlignmentSet = itemHorizontalAlignment is not null;
        var itemVerticalAlignmentSet = itemVerticalAlignment is not null;
        var spacingSize = GetSpacingSize();

        var childConstraint = new Size(
            itemWidthSet ? ItemWidth : availableSize.Width,
            itemHeightSet ? ItemHeight : availableSize.Height);

        var children = InternalChildren;
        var isFirst = true;

        if (ChildWrapping == VisualWrappingType.Wrap)
        {
            for (int i = 0, count = children.Count; i < count; i++)
            {
                var child = children[i];
                if (child is null)
                {
                    continue;
                }

                if (itemHorizontalAlignmentSet)
                {
                    child.SetCurrentValue(HorizontalAlignmentProperty, ItemHorizontalAlignment);
                }

                if (itemVerticalAlignmentSet)
                {
                    child.SetCurrentValue(VerticalAlignmentProperty, ItemVerticalAlignment);
                }

                child.Measure(childConstraint);

                var sz = new PanelUvSize(
                    orientation,
                    itemWidthSet ? ItemWidth : child.DesiredSize.Width,
                    itemHeightSet ? ItemHeight : child.DesiredSize.Height);

                // Same rule as ArrangeOverride: the very first item gets no leading spacing.
                if (GreaterThan(curLineSize.U + (isFirst ? sz.U : sz.U + spacingSize.U), uvConstraint.U))
                {
                    panelSize.U = System.Math.Max(curLineSize.U, panelSize.U);
                    panelSize.V += curLineSize.V + spacingSize.V;
                    curLineSize = sz;

                    if (GreaterThan(sz.U, uvConstraint.U))
                    {
                        panelSize.U = System.Math.Max(sz.U, panelSize.U);
                        panelSize.V += sz.V + spacingSize.V;
                        curLineSize = new PanelUvSize(orientation);
                    }
                }
                else
                {
                    curLineSize.U += isFirst ? sz.U : sz.U + spacingSize.U;
                    curLineSize.V = System.Math.Max(sz.V, curLineSize.V);

                    isFirst = false;
                }
            }
        }
        else
        {
            var layoutSlotSize = availableSize;

            if (orientation == Orientation.Horizontal)
            {
                layoutSlotSize.Width = double.PositiveInfinity;
            }
            else
            {
                layoutSlotSize.Height = double.PositiveInfinity;
            }

            for (int i = 0, count = children.Count; i < count; ++i)
            {
                var child = children[i];
                if (child is null)
                {
                    continue;
                }

                if (itemHorizontalAlignmentSet)
                {
                    child.SetCurrentValue(HorizontalAlignmentProperty, itemHorizontalAlignment);
                }

                if (itemVerticalAlignmentSet)
                {
                    child.SetCurrentValue(VerticalAlignmentProperty, itemVerticalAlignment);
                }

                child.Measure(layoutSlotSize);

                var sz = new PanelUvSize(
                    orientation,
                    itemWidthSet ? ItemWidth : child.DesiredSize.Width,
                    itemHeightSet ? ItemHeight : child.DesiredSize.Height);

                // Same rule as ArrangeLine: zero-size (e.g. collapsed) children take no slot and no spacing.
                if (sz.U <= 0)
                {
                    curLineSize.V = System.Math.Max(sz.V, curLineSize.V);
                    continue;
                }

                curLineSize.U += isFirst ? sz.U : sz.U + spacingSize.U;
                curLineSize.V = System.Math.Max(sz.V, curLineSize.V);

                isFirst = false;
            }
        }

        panelSize.U = System.Math.Max(curLineSize.U, panelSize.U);
        panelSize.V += curLineSize.V;

        return new Size(panelSize.Width, panelSize.Height);
    }

    /// <inheritdoc />
    protected override Size ArrangeOverride(Size finalSize)
    {
        var firstInLine = 0;
        double accumulatedV = 0;
        var itemU = orientation == Orientation.Horizontal
            ? ItemWidth
            : ItemHeight;
        var curLineSize = new PanelUvSize(orientation);
        var uvFinalSize = new PanelUvSize(orientation, finalSize);
        var itemWidthSet = !double.IsNaN(ItemWidth);
        var itemHeightSet = !double.IsNaN(ItemHeight);
        var useItemU = orientation == Orientation.Horizontal
            ? itemWidthSet
            : itemHeightSet;
        var spacingSize = GetSpacingSize();

        var children = InternalChildren;
        var isFirst = true;

        if (ChildWrapping == VisualWrappingType.Wrap)
        {
            for (int i = 0, count = children.Count; i < count; i++)
            {
                var child = children[i];
                if (child is null)
                {
                    continue;
                }

                var sz = new PanelUvSize(
                    orientation,
                    itemWidthSet ? ItemWidth : child.DesiredSize.Width,
                    itemHeightSet ? ItemHeight : child.DesiredSize.Height);

                if (GreaterThan(curLineSize.U + (isFirst ? sz.U : sz.U + spacingSize.U), uvFinalSize.U))
                {
                    ArrangeWrapLine(accumulatedV, curLineSize.V, firstInLine, i, useItemU, itemU, spacingSize.U);

                    accumulatedV += curLineSize.V + spacingSize.V;
                    curLineSize = sz;

                    firstInLine = i;
                }
                else
                {
                    curLineSize.U += isFirst
                        ? sz.U
                        : sz.U + spacingSize.U;
                    curLineSize.V = System.Math.Max(sz.V, curLineSize.V);
                }

                isFirst = false;
            }

            if (firstInLine < children.Count)
            {
                ArrangeWrapLine(accumulatedV, curLineSize.V, firstInLine, children.Count, useItemU, itemU, spacingSize.U);
            }
        }
        else
        {
            ArrangeLine(uvFinalSize.V, useItemU, itemU, spacingSize.U);
        }

        return finalSize;
    }

    private PanelUvSize GetSpacingSize()
    {
        if (!double.IsNaN(Spacing))
        {
            return new PanelUvSize(orientation, Spacing, Spacing);
        }

        if (double.IsNaN(HorizontalSpacing))
        {
            HorizontalSpacing = 0;
        }

        if (double.IsNaN(VerticalSpacing))
        {
            VerticalSpacing = 0;
        }

        return new PanelUvSize(orientation, HorizontalSpacing, VerticalSpacing);
    }

    private static bool GreaterThan(
        double value1,
        double value2)
        => value1 > value2 && !value1.AreClose(value2);
}