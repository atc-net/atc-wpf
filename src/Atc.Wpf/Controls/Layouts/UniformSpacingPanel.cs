// ReSharper disable LocalVariableHidesMember
namespace Atc.Wpf.Controls.Layouts;

[SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
public sealed partial class UniformSpacingPanel : Panel
{
    [DependencyProperty(
        DefaultValue = Orientation.Horizontal,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure,
        PropertyChangedCallback = nameof(OnOrientationChanged))]
    private Orientation orientation;

    [DependencyProperty(
        DefaultValue = default(VisualWrappingType),
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure)]
    private VisualWrappingType childWrapping;

    [DependencyProperty(
        DefaultValue = double.NaN,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure,
        ValidateValueCallback = nameof(IsSpacingValid))]
    private double spacing;

    [DependencyProperty(
        DefaultValue = double.NaN,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure,
        ValidateValueCallback = nameof(IsSpacingValid))]
    private double horizontalSpacing;

    [DependencyProperty(
        DefaultValue = double.NaN,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure,
        ValidateValueCallback = nameof(IsSpacingValid))]
    private double verticalSpacing;

    [DependencyProperty(
        DefaultValue = double.NaN,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure,
        ValidateValueCallback = nameof(IsWidthHeightValid))]
    private double itemWidth;

    [DependencyProperty(
        DefaultValue = double.NaN,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure,
        ValidateValueCallback = nameof(IsWidthHeightValid))]
    private double itemHeight;

    [DependencyProperty(
        DefaultValue = HorizontalAlignment.Stretch,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure)]
    private HorizontalAlignment? itemHorizontalAlignment;

    [DependencyProperty(
        DefaultValue = VerticalAlignment.Stretch,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure)]
    private VerticalAlignment? itemVerticalAlignment;

    private static void OnOrientationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var p = (UniformSpacingPanel)d;
        p.orientation = (Orientation)e.NewValue;
    }

    private static bool IsWidthHeightValid(
        object value)
    {
        var v = (double)value;
        return double.IsNaN(v) || (v >= 0.0d && !double.IsPositiveInfinity(v));
    }

    private static bool IsSpacingValid(
        object value)
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

    [SuppressMessage("", "MA0084:Local variable should not hide field", Justification = "OK.")]
    [SuppressMessage("", "S1117:Local variable should not hide field", Justification = "OK.")]
    protected override Size MeasureOverride(
        Size availableSize)
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

                if (GreaterThan(curLineSize.U + sz.U + spacingSize.U, uvConstraint.U))
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
                    itemWidthSet ? itemWidth : child.DesiredSize.Width,
                    itemHeightSet ? itemHeight : child.DesiredSize.Height);

                curLineSize.U += isFirst ? sz.U : sz.U + spacingSize.U;
                curLineSize.V = System.Math.Max(sz.V, curLineSize.V);

                isFirst = false;
            }
        }

        panelSize.U = System.Math.Max(curLineSize.U, panelSize.U);
        panelSize.V += curLineSize.V;

        return new Size(panelSize.Width, panelSize.Height);
    }

    protected override Size ArrangeOverride(
        Size finalSize)
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