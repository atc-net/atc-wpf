namespace Atc.Wpf.Controls.Layouts;

/// <summary>
/// A responsive panel that adapts its layout based on available width using configurable breakpoints.
/// Supports auto-calculated columns based on MinItemWidth, explicit column counts per breakpoint,
/// visibility control, and item ordering per breakpoint.
/// </summary>
[SuppressMessage("Design", "MA0051:Method is too long", Justification = "Complex layout algorithm.")]
public class ResponsivePanel : Panel
{
    /// <summary>
    /// Maximum width for extra-small breakpoint (default: 576px).
    /// </summary>
    public static readonly int XsMaxWidth = 576;

    /// <summary>
    /// Maximum width for small breakpoint (default: 768px).
    /// </summary>
    public static readonly int SmMaxWidth = 768;

    /// <summary>
    /// Maximum width for medium breakpoint (default: 992px).
    /// </summary>
    public static readonly int MdMaxWidth = 992;

    /// <summary>
    /// Maximum width for large breakpoint (default: 1200px).
    /// </summary>
    public static readonly int LgMaxWidth = 1200;

    /// <summary>
    /// Default number of columns in a 12-column grid.
    /// </summary>
    public static readonly int MaxColumns = 12;

    /// <summary>
    /// Identifies the Gap dependency property.
    /// </summary>
    public static readonly DependencyProperty GapProperty = DependencyProperty.Register(
        nameof(Gap),
        typeof(double),
        typeof(ResponsivePanel),
        new FrameworkPropertyMetadata(
            0.0,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange),
        IsGapValid);

    /// <summary>
    /// Identifies the RowGap dependency property.
    /// </summary>
    public static readonly DependencyProperty RowGapProperty = DependencyProperty.Register(
        nameof(RowGap),
        typeof(double),
        typeof(ResponsivePanel),
        new FrameworkPropertyMetadata(
            double.NaN,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange),
        IsGapValid);

    /// <summary>
    /// Identifies the ColumnGap dependency property.
    /// </summary>
    public static readonly DependencyProperty ColumnGapProperty = DependencyProperty.Register(
        nameof(ColumnGap),
        typeof(double),
        typeof(ResponsivePanel),
        new FrameworkPropertyMetadata(
            double.NaN,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange),
        IsGapValid);

    /// <summary>
    /// Identifies the MinItemWidth dependency property.
    /// </summary>
    public static readonly DependencyProperty MinItemWidthProperty = DependencyProperty.Register(
        nameof(MinItemWidth),
        typeof(double),
        typeof(ResponsivePanel),
        new FrameworkPropertyMetadata(
            double.NaN,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange),
        IsMinItemWidthValid);

    /// <summary>
    /// Identifies the ColumnsXs dependency property.
    /// </summary>
    public static readonly DependencyProperty ColumnsXsProperty = DependencyProperty.Register(
        nameof(ColumnsXs),
        typeof(int),
        typeof(ResponsivePanel),
        new FrameworkPropertyMetadata(
            1,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange),
        IsColumnCountValid);

    /// <summary>
    /// Identifies the ColumnsSm dependency property.
    /// </summary>
    public static readonly DependencyProperty ColumnsSmProperty = DependencyProperty.Register(
        nameof(ColumnsSm),
        typeof(int),
        typeof(ResponsivePanel),
        new FrameworkPropertyMetadata(
            2,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange),
        IsColumnCountValid);

    /// <summary>
    /// Identifies the ColumnsMd dependency property.
    /// </summary>
    public static readonly DependencyProperty ColumnsMdProperty = DependencyProperty.Register(
        nameof(ColumnsMd),
        typeof(int),
        typeof(ResponsivePanel),
        new FrameworkPropertyMetadata(
            3,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange),
        IsColumnCountValid);

    /// <summary>
    /// Identifies the ColumnsLg dependency property.
    /// </summary>
    public static readonly DependencyProperty ColumnsLgProperty = DependencyProperty.Register(
        nameof(ColumnsLg),
        typeof(int),
        typeof(ResponsivePanel),
        new FrameworkPropertyMetadata(
            4,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange),
        IsColumnCountValid);

    /// <summary>
    /// Identifies the ColumnsXl dependency property.
    /// </summary>
    public static readonly DependencyProperty ColumnsXlProperty = DependencyProperty.Register(
        nameof(ColumnsXl),
        typeof(int),
        typeof(ResponsivePanel),
        new FrameworkPropertyMetadata(
            6,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange),
        IsColumnCountValid);

    /// <summary>
    /// Identifies the SpanXs attached property.
    /// </summary>
    public static readonly DependencyProperty SpanXsProperty = DependencyProperty.RegisterAttached(
        "SpanXs",
        typeof(int),
        typeof(ResponsivePanel),
        new FrameworkPropertyMetadata(
            0,
            FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange),
        IsSpanValid);

    /// <summary>
    /// Identifies the SpanSm attached property.
    /// </summary>
    public static readonly DependencyProperty SpanSmProperty = DependencyProperty.RegisterAttached(
        "SpanSm",
        typeof(int),
        typeof(ResponsivePanel),
        new FrameworkPropertyMetadata(
            0,
            FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange),
        IsSpanValid);

    /// <summary>
    /// Identifies the SpanMd attached property.
    /// </summary>
    public static readonly DependencyProperty SpanMdProperty = DependencyProperty.RegisterAttached(
        "SpanMd",
        typeof(int),
        typeof(ResponsivePanel),
        new FrameworkPropertyMetadata(
            0,
            FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange),
        IsSpanValid);

    /// <summary>
    /// Identifies the SpanLg attached property.
    /// </summary>
    public static readonly DependencyProperty SpanLgProperty = DependencyProperty.RegisterAttached(
        "SpanLg",
        typeof(int),
        typeof(ResponsivePanel),
        new FrameworkPropertyMetadata(
            0,
            FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange),
        IsSpanValid);

    /// <summary>
    /// Identifies the SpanXl attached property.
    /// </summary>
    public static readonly DependencyProperty SpanXlProperty = DependencyProperty.RegisterAttached(
        "SpanXl",
        typeof(int),
        typeof(ResponsivePanel),
        new FrameworkPropertyMetadata(
            0,
            FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange),
        IsSpanValid);

    /// <summary>
    /// Identifies the VisibleFrom attached property.
    /// </summary>
    public static readonly DependencyProperty VisibleFromProperty = DependencyProperty.RegisterAttached(
        "VisibleFrom",
        typeof(ResponsiveBreakpoint?),
        typeof(ResponsivePanel),
        new FrameworkPropertyMetadata(
            null,
            FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange));

    /// <summary>
    /// Identifies the HiddenFrom attached property.
    /// </summary>
    public static readonly DependencyProperty HiddenFromProperty = DependencyProperty.RegisterAttached(
        "HiddenFrom",
        typeof(ResponsiveBreakpoint?),
        typeof(ResponsivePanel),
        new FrameworkPropertyMetadata(
            null,
            FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange));

    /// <summary>
    /// Identifies the OrderXs attached property.
    /// </summary>
    public static readonly DependencyProperty OrderXsProperty = DependencyProperty.RegisterAttached(
        "OrderXs",
        typeof(int),
        typeof(ResponsivePanel),
        new FrameworkPropertyMetadata(
            int.MaxValue,
            FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange));

    /// <summary>
    /// Identifies the OrderSm attached property.
    /// </summary>
    public static readonly DependencyProperty OrderSmProperty = DependencyProperty.RegisterAttached(
        "OrderSm",
        typeof(int),
        typeof(ResponsivePanel),
        new FrameworkPropertyMetadata(
            int.MaxValue,
            FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange));

    /// <summary>
    /// Identifies the OrderMd attached property.
    /// </summary>
    public static readonly DependencyProperty OrderMdProperty = DependencyProperty.RegisterAttached(
        "OrderMd",
        typeof(int),
        typeof(ResponsivePanel),
        new FrameworkPropertyMetadata(
            int.MaxValue,
            FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange));

    /// <summary>
    /// Identifies the OrderLg attached property.
    /// </summary>
    public static readonly DependencyProperty OrderLgProperty = DependencyProperty.RegisterAttached(
        "OrderLg",
        typeof(int),
        typeof(ResponsivePanel),
        new FrameworkPropertyMetadata(
            int.MaxValue,
            FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange));

    /// <summary>
    /// Identifies the OrderXl attached property.
    /// </summary>
    public static readonly DependencyProperty OrderXlProperty = DependencyProperty.RegisterAttached(
        "OrderXl",
        typeof(int),
        typeof(ResponsivePanel),
        new FrameworkPropertyMetadata(
            int.MaxValue,
            FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange));

    /// <summary>
    /// Gets or sets the uniform gap between items (both row and column).
    /// </summary>
    [Category("Layout")]
    [Description("The uniform gap between items (both row and column).")]
    public double Gap
    {
        get => (double)GetValue(GapProperty);
        set => SetValue(GapProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertical gap between rows. Overrides Gap for vertical spacing.
    /// </summary>
    [Category("Layout")]
    [Description("The vertical gap between rows. Overrides Gap for vertical spacing.")]
    public double RowGap
    {
        get => (double)GetValue(RowGapProperty);
        set => SetValue(RowGapProperty, value);
    }

    /// <summary>
    /// Gets or sets the horizontal gap between columns. Overrides Gap for horizontal spacing.
    /// </summary>
    [Category("Layout")]
    [Description("The horizontal gap between columns. Overrides Gap for horizontal spacing.")]
    public double ColumnGap
    {
        get => (double)GetValue(ColumnGapProperty);
        set => SetValue(ColumnGapProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum item width for auto-calculating column count.
    /// When set, overrides explicit ColumnsXs/Sm/Md/Lg/Xl values.
    /// </summary>
    [Category("Layout")]
    [Description("The minimum item width for auto-calculating column count.")]
    public double MinItemWidth
    {
        get => (double)GetValue(MinItemWidthProperty);
        set => SetValue(MinItemWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of columns at extra-small breakpoint (less than 576px).
    /// </summary>
    [Category("Layout")]
    [Description("Number of columns at extra-small breakpoint (< 576px).")]
    public int ColumnsXs
    {
        get => (int)GetValue(ColumnsXsProperty);
        set => SetValue(ColumnsXsProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of columns at small breakpoint (576px to 767px).
    /// </summary>
    [Category("Layout")]
    [Description("Number of columns at small breakpoint (576-767px).")]
    public int ColumnsSm
    {
        get => (int)GetValue(ColumnsSmProperty);
        set => SetValue(ColumnsSmProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of columns at medium breakpoint (768px to 991px).
    /// </summary>
    [Category("Layout")]
    [Description("Number of columns at medium breakpoint (768-991px).")]
    public int ColumnsMd
    {
        get => (int)GetValue(ColumnsMdProperty);
        set => SetValue(ColumnsMdProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of columns at large breakpoint (992px to 1199px).
    /// </summary>
    [Category("Layout")]
    [Description("Number of columns at large breakpoint (992-1199px).")]
    public int ColumnsLg
    {
        get => (int)GetValue(ColumnsLgProperty);
        set => SetValue(ColumnsLgProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of columns at extra-large breakpoint (1200px and above).
    /// </summary>
    [Category("Layout")]
    [Description("Number of columns at extra-large breakpoint (>= 1200px).")]
    public int ColumnsXl
    {
        get => (int)GetValue(ColumnsXlProperty);
        set => SetValue(ColumnsXlProperty, value);
    }

    /// <summary>
    /// Gets the column span at extra-small breakpoint.
    /// </summary>
    public static int GetSpanXs(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (int)element.GetValue(SpanXsProperty);
    }

    /// <summary>
    /// Sets the column span at extra-small breakpoint.
    /// </summary>
    public static void SetSpanXs(DependencyObject element, int value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(SpanXsProperty, value);
    }

    /// <summary>
    /// Gets the column span at small breakpoint.
    /// </summary>
    public static int GetSpanSm(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (int)element.GetValue(SpanSmProperty);
    }

    /// <summary>
    /// Sets the column span at small breakpoint.
    /// </summary>
    public static void SetSpanSm(DependencyObject element, int value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(SpanSmProperty, value);
    }

    /// <summary>
    /// Gets the column span at medium breakpoint.
    /// </summary>
    public static int GetSpanMd(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (int)element.GetValue(SpanMdProperty);
    }

    /// <summary>
    /// Sets the column span at medium breakpoint.
    /// </summary>
    public static void SetSpanMd(DependencyObject element, int value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(SpanMdProperty, value);
    }

    /// <summary>
    /// Gets the column span at large breakpoint.
    /// </summary>
    public static int GetSpanLg(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (int)element.GetValue(SpanLgProperty);
    }

    /// <summary>
    /// Sets the column span at large breakpoint.
    /// </summary>
    public static void SetSpanLg(DependencyObject element, int value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(SpanLgProperty, value);
    }

    /// <summary>
    /// Gets the column span at extra-large breakpoint.
    /// </summary>
    public static int GetSpanXl(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (int)element.GetValue(SpanXlProperty);
    }

    /// <summary>
    /// Sets the column span at extra-large breakpoint.
    /// </summary>
    public static void SetSpanXl(DependencyObject element, int value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(SpanXlProperty, value);
    }

    /// <summary>
    /// Gets the breakpoint from which the element becomes visible.
    /// </summary>
    public static ResponsiveBreakpoint? GetVisibleFrom(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (ResponsiveBreakpoint?)element.GetValue(VisibleFromProperty);
    }

    /// <summary>
    /// Sets the breakpoint from which the element becomes visible.
    /// </summary>
    public static void SetVisibleFrom(DependencyObject element, ResponsiveBreakpoint? value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(VisibleFromProperty, value);
    }

    /// <summary>
    /// Gets the breakpoint from which the element becomes hidden.
    /// </summary>
    public static ResponsiveBreakpoint? GetHiddenFrom(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (ResponsiveBreakpoint?)element.GetValue(HiddenFromProperty);
    }

    /// <summary>
    /// Sets the breakpoint from which the element becomes hidden.
    /// </summary>
    public static void SetHiddenFrom(DependencyObject element, ResponsiveBreakpoint? value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(HiddenFromProperty, value);
    }

    /// <summary>
    /// Gets the display order at extra-small breakpoint.
    /// </summary>
    public static int GetOrderXs(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (int)element.GetValue(OrderXsProperty);
    }

    /// <summary>
    /// Sets the display order at extra-small breakpoint.
    /// </summary>
    public static void SetOrderXs(DependencyObject element, int value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(OrderXsProperty, value);
    }

    /// <summary>
    /// Gets the display order at small breakpoint.
    /// </summary>
    public static int GetOrderSm(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (int)element.GetValue(OrderSmProperty);
    }

    /// <summary>
    /// Sets the display order at small breakpoint.
    /// </summary>
    public static void SetOrderSm(DependencyObject element, int value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(OrderSmProperty, value);
    }

    /// <summary>
    /// Gets the display order at medium breakpoint.
    /// </summary>
    public static int GetOrderMd(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (int)element.GetValue(OrderMdProperty);
    }

    /// <summary>
    /// Sets the display order at medium breakpoint.
    /// </summary>
    public static void SetOrderMd(DependencyObject element, int value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(OrderMdProperty, value);
    }

    /// <summary>
    /// Gets the display order at large breakpoint.
    /// </summary>
    public static int GetOrderLg(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (int)element.GetValue(OrderLgProperty);
    }

    /// <summary>
    /// Sets the display order at large breakpoint.
    /// </summary>
    public static void SetOrderLg(DependencyObject element, int value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(OrderLgProperty, value);
    }

    /// <summary>
    /// Gets the display order at extra-large breakpoint.
    /// </summary>
    public static int GetOrderXl(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (int)element.GetValue(OrderXlProperty);
    }

    /// <summary>
    /// Sets the display order at extra-large breakpoint.
    /// </summary>
    public static void SetOrderXl(DependencyObject element, int value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(OrderXlProperty, value);
    }

    private static bool IsGapValid(object value)
        => value is double gap && (double.IsNaN(gap) || gap >= 0);

    private static bool IsMinItemWidthValid(object value)
        => value is double v && (double.IsNaN(v) || v > 0);

    private static bool IsColumnCountValid(object value)
        => value is int count && count >= 1 && count <= MaxColumns;

    private static bool IsSpanValid(object value)
        => value is int span && span >= 0 && span <= MaxColumns;

    /// <summary>
    /// Gets the current breakpoint based on the available width.
    /// </summary>
    public static ResponsiveBreakpoint GetBreakpoint(double width)
    {
        if (width < XsMaxWidth)
        {
            return ResponsiveBreakpoint.Xs;
        }

        if (width < SmMaxWidth)
        {
            return ResponsiveBreakpoint.Sm;
        }

        if (width < MdMaxWidth)
        {
            return ResponsiveBreakpoint.Md;
        }

        if (width < LgMaxWidth)
        {
            return ResponsiveBreakpoint.Lg;
        }

        return ResponsiveBreakpoint.Xl;
    }

    private double GetEffectiveRowGap()
        => double.IsNaN(RowGap) ? Gap : RowGap;

    private double GetEffectiveColumnGap()
        => double.IsNaN(ColumnGap) ? Gap : ColumnGap;

    private int GetColumnCountForBreakpoint(ResponsiveBreakpoint breakpoint)
    {
        return breakpoint switch
        {
            ResponsiveBreakpoint.Xs => ColumnsXs,
            ResponsiveBreakpoint.Sm => ColumnsSm,
            ResponsiveBreakpoint.Md => ColumnsMd,
            ResponsiveBreakpoint.Lg => ColumnsLg,
            ResponsiveBreakpoint.Xl => ColumnsXl,
            _ => ColumnsXl,
        };
    }

    private int GetColumnCount(double availableWidth, ResponsiveBreakpoint breakpoint)
    {
        // If MinItemWidth is set, auto-calculate columns
        if (!double.IsNaN(MinItemWidth) && MinItemWidth > 0)
        {
            var columnGap = GetEffectiveColumnGap();
            var columns = (int)System.Math.Floor((availableWidth + columnGap) / (MinItemWidth + columnGap));
            return System.Math.Max(1, System.Math.Min(columns, MaxColumns));
        }

        // Otherwise use breakpoint-specific column count
        return GetColumnCountForBreakpoint(breakpoint);
    }

    private static int GetSpanForBreakpoint(UIElement element, ResponsiveBreakpoint breakpoint)
    {
        var span = breakpoint switch
        {
            ResponsiveBreakpoint.Xs => GetSpanXs(element),
            ResponsiveBreakpoint.Sm => GetSpanSm(element),
            ResponsiveBreakpoint.Md => GetSpanMd(element),
            ResponsiveBreakpoint.Lg => GetSpanLg(element),
            ResponsiveBreakpoint.Xl => GetSpanXl(element),
            _ => 0,
        };

        // If span is 0 (default), fall back to smaller breakpoints or return 0 for full column width
        if (span == 0)
        {
            span = breakpoint switch
            {
                ResponsiveBreakpoint.Xl => GetSpanLg(element),
                ResponsiveBreakpoint.Lg => GetSpanMd(element),
                ResponsiveBreakpoint.Md => GetSpanSm(element),
                ResponsiveBreakpoint.Sm => GetSpanXs(element),
                _ => 0,
            };
        }

        return span;
    }

    private static int GetOrderForBreakpoint(UIElement element, ResponsiveBreakpoint breakpoint, int defaultOrder)
    {
        var order = breakpoint switch
        {
            ResponsiveBreakpoint.Xs => GetOrderXs(element),
            ResponsiveBreakpoint.Sm => GetOrderSm(element),
            ResponsiveBreakpoint.Md => GetOrderMd(element),
            ResponsiveBreakpoint.Lg => GetOrderLg(element),
            ResponsiveBreakpoint.Xl => GetOrderXl(element),
            _ => int.MaxValue,
        };

        // If order is MaxValue (default), fall back to smaller breakpoints or use default index
        if (order == int.MaxValue)
        {
            order = breakpoint switch
            {
                ResponsiveBreakpoint.Xl => GetOrderLg(element),
                ResponsiveBreakpoint.Lg => GetOrderMd(element),
                ResponsiveBreakpoint.Md => GetOrderSm(element),
                ResponsiveBreakpoint.Sm => GetOrderXs(element),
                _ => int.MaxValue,
            };
        }

        return order == int.MaxValue ? defaultOrder : order;
    }

    private static bool IsVisibleAtBreakpoint(UIElement element, ResponsiveBreakpoint breakpoint)
    {
        if (element.Visibility == Visibility.Collapsed)
        {
            return false;
        }

        var visibleFrom = GetVisibleFrom(element);
        var hiddenFrom = GetHiddenFrom(element);

        // Check VisibleFrom - element only visible at this breakpoint and larger
        if (visibleFrom.HasValue && breakpoint < visibleFrom.Value)
        {
            return false;
        }

        // Check HiddenFrom - element hidden at this breakpoint and larger
        if (hiddenFrom.HasValue && breakpoint >= hiddenFrom.Value)
        {
            return false;
        }

        return true;
    }

    private List<(UIElement Element, int Order, int Index)> GetOrderedVisibleChildren(
        ResponsiveBreakpoint breakpoint)
    {
        var result = new List<(UIElement Element, int Order, int Index)>();
        var children = InternalChildren;

        for (var i = 0; i < children.Count; i++)
        {
            var child = children[i];
            if (IsVisibleAtBreakpoint(child, breakpoint))
            {
                var order = GetOrderForBreakpoint(child, breakpoint, i);
                result.Add((child, order, i));
            }
        }

        // Sort by order, then by original index for stability
        result.Sort((a, b) =>
        {
            var orderCompare = a.Order.CompareTo(b.Order);
            return orderCompare != 0 ? orderCompare : a.Index.CompareTo(b.Index);
        });

        return result;
    }

    /// <inheritdoc/>
    protected override Size MeasureOverride(Size availableSize)
    {
        var children = InternalChildren;
        if (children.Count == 0)
        {
            return default;
        }

        var availableWidth = double.IsPositiveInfinity(availableSize.Width)
            ? SystemParameters.PrimaryScreenWidth
            : availableSize.Width;

        var breakpoint = GetBreakpoint(availableWidth);
        var columnCount = GetColumnCount(availableWidth, breakpoint);
        var columnGap = GetEffectiveColumnGap();
        var rowGap = GetEffectiveRowGap();

        // Calculate column width
        var totalGapWidth = (columnCount - 1) * columnGap;
        var columnWidth = (availableWidth - totalGapWidth) / columnCount;

        // Measure all children
        var childConstraint = new Size(columnWidth, double.PositiveInfinity);

        foreach (UIElement child in children)
        {
            child.Measure(childConstraint);
        }

        // Get ordered visible children
        var orderedChildren = GetOrderedVisibleChildren(breakpoint);

        // Calculate total height
        double currentRowHeight = 0;
        double totalHeight = 0;
        var currentColumn = 0;

        foreach (var (child, _, _) in orderedChildren)
        {
            var span = GetSpanForBreakpoint(child, breakpoint);
            var effectiveSpan = span > 0 ? System.Math.Min(span, columnCount) : 1;

            // Check if we need to wrap to next row
            if (currentColumn + effectiveSpan > columnCount)
            {
                totalHeight += currentRowHeight + rowGap;
                currentRowHeight = 0;
                currentColumn = 0;
            }

            currentRowHeight = System.Math.Max(currentRowHeight, child.DesiredSize.Height);
            currentColumn += effectiveSpan;
        }

        // Add the last row height
        totalHeight += currentRowHeight;

        return new Size(availableWidth, totalHeight);
    }

    /// <inheritdoc/>
    protected override Size ArrangeOverride(Size finalSize)
    {
        var children = InternalChildren;
        if (children.Count == 0)
        {
            return finalSize;
        }

        var breakpoint = GetBreakpoint(finalSize.Width);
        var columnCount = GetColumnCount(finalSize.Width, breakpoint);
        var columnGap = GetEffectiveColumnGap();
        var rowGap = GetEffectiveRowGap();

        // Calculate column width
        var totalGapWidth = (columnCount - 1) * columnGap;
        var columnWidth = (finalSize.Width - totalGapWidth) / columnCount;

        // Get ordered visible children
        var orderedChildren = GetOrderedVisibleChildren(breakpoint);

        // Build rows for arrangement
        var rows = new List<List<(UIElement Element, int Span)>>();
        var currentRow = new List<(UIElement Element, int Span)>();
        var currentColumn = 0;

        foreach (var (child, _, _) in orderedChildren)
        {
            var span = GetSpanForBreakpoint(child, breakpoint);
            var effectiveSpan = span > 0 ? System.Math.Min(span, columnCount) : 1;

            // Check if we need to wrap to next row
            if (currentColumn + effectiveSpan > columnCount)
            {
                if (currentRow.Count > 0)
                {
                    rows.Add(currentRow);
                }

                currentRow = [];
                currentColumn = 0;
            }

            currentRow.Add((child, effectiveSpan));
            currentColumn += effectiveSpan;
        }

        // Add the last row
        if (currentRow.Count > 0)
        {
            rows.Add(currentRow);
        }

        // Arrange children row by row
        double yOffset = 0;

        foreach (var row in rows)
        {
            double xOffset = 0;
            double rowHeight = 0;

            // First pass: calculate row height
            foreach (var (element, _) in row)
            {
                rowHeight = System.Math.Max(rowHeight, element.DesiredSize.Height);
            }

            // Second pass: arrange children
            foreach (var (element, span) in row)
            {
                var childWidth = (span * columnWidth) + ((span - 1) * columnGap);
                var rect = new Rect(xOffset, yOffset, childWidth, rowHeight);
                element.Arrange(rect);

                xOffset += childWidth + columnGap;
            }

            yOffset += rowHeight + rowGap;
        }

        // Hide non-visible children (arrange at zero size)
        var visibleElements = new HashSet<UIElement>(orderedChildren.Select(x => x.Element));

        foreach (UIElement child in children)
        {
            if (!visibleElements.Contains(child))
            {
                child.Arrange(new Rect(0, 0, 0, 0));
            }
        }

        return finalSize;
    }
}

