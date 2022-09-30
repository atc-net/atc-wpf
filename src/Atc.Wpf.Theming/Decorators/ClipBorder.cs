namespace Atc.Wpf.Theming.Decorators;

/// <summary>
/// Represents a border whose contents are clipped within the bounds
/// of the border. The border may have rounded corners.
/// </summary>
[SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
public sealed class ClipBorder : Decorator
{
    private StreamGeometry? backgroundGeometryCache;
    private StreamGeometry? borderGeometryCache;

    public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register(
        nameof(BorderThickness),
        typeof(Thickness),
        typeof(ClipBorder),
        new FrameworkPropertyMetadata(
            new Thickness(0),
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender),
        OnValidateThickness);

    public Thickness BorderThickness
    {
        get => (Thickness)GetValue(BorderThicknessProperty);
        set => SetValue(BorderThicknessProperty, value);
    }

    private static bool OnValidateThickness(object? value)
    {
        var th = (Thickness)(value ?? default(Thickness));
        return th.IsValid(
            allowNegative: false,
            allowNaN: false,
            allowPositiveInfinity: false,
            allowNegativeInfinity: false);
    }

    public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register(
        nameof(Padding),
        typeof(Thickness),
        typeof(ClipBorder),
        new FrameworkPropertyMetadata(
            new Thickness(0),
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender),
        OnValidateThickness);

    public Thickness Padding
    {
        get => (Thickness)GetValue(PaddingProperty);
        set => SetValue(PaddingProperty, value);
    }

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        nameof(CornerRadius),
        typeof(CornerRadius),
        typeof(ClipBorder),
        new FrameworkPropertyMetadata(
            new CornerRadius(0),
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender),
        OnValidateCornerRadius);

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    private static bool OnValidateCornerRadius(object? value)
    {
        var cr = (CornerRadius)(value ?? default(CornerRadius));
        return cr.IsValid(
            allowNegative: false,
            allowNaN: false,
            allowPositiveInfinity: false,
            allowNegativeInfinity: false);
    }

    public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register(
        nameof(BorderBrush),
        typeof(Brush),
        typeof(ClipBorder),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

    public Brush? BorderBrush
    {
        get => (Brush?)GetValue(BorderBrushProperty);
        set => SetValue(BorderBrushProperty, value);
    }

    public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
        nameof(Background),
        typeof(Brush),
        typeof(ClipBorder),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

    public Brush? Background
    {
        get => (Brush?)GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    public static readonly DependencyProperty OptimizeClipRenderingProperty = DependencyProperty.Register(
        nameof(OptimizeClipRendering),
        typeof(bool),
        typeof(ClipBorder),
        new FrameworkPropertyMetadata(
            BooleanBoxes.FalseBox,
            FrameworkPropertyMetadataOptions.AffectsRender));

    public bool OptimizeClipRendering
    {
        get => (bool)GetValue(OptimizeClipRenderingProperty);
        set => SetValue(OptimizeClipRenderingProperty, BooleanBoxes.Box(value));
    }

    protected override Size MeasureOverride(
        Size constraint)
    {
        var child = Child;
        var desiredSize = new Size(0, 0);
        var borders = BorderThickness;

        // Compute the total size required
        var borderSize = borders.CollapseThickness();
        var paddingSize = Padding.CollapseThickness();

        // Does the ClipBorder have a child?
        if (child != null)
        {
            // Combine into total decorating size
            var combined = new Size(borderSize.Width + paddingSize.Width, borderSize.Height + paddingSize.Height);

            // Remove size of border only from child's reference size.
            var childConstraint = new Size(
                System.Math.Max(0.0, constraint.Width - combined.Width),
                System.Math.Max(0.0, constraint.Height - combined.Height));

            child.Measure(childConstraint);
            var childSize = child.DesiredSize;

            // Now use the returned size to drive our size, by adding back the margins, etc.
            desiredSize.Width = childSize.Width + combined.Width;
            desiredSize.Height = childSize.Height + combined.Height;
        }
        else
        {
            // Since there is no child, the border requires only the size occupied by the BorderThickness
            // and the Padding
            desiredSize = new Size(borderSize.Width + paddingSize.Width, borderSize.Height + paddingSize.Height);
        }

        return desiredSize;
    }

    protected override Size ArrangeOverride(
        Size arrangeSize)
    {
        var borders = BorderThickness;
        var boundRect = new Rect(arrangeSize);
        var innerRect = boundRect.Deflate(borders);
        var corners = CornerRadius;
        var padding = Padding;
        var childRect = innerRect.Deflate(padding);

        if (!boundRect.Width.IsZero() && !boundRect.Height.IsZero())
        {
            var outerBorderInfo = new BorderInfo(corners, borders, new Thickness(0), isOuterBorder: true);
            var borderGeometry = new StreamGeometry();

            using (var ctx = borderGeometry.Open())
            {
                GenerateGeometry(ctx, boundRect, outerBorderInfo);
            }

            borderGeometry.Freeze();
            borderGeometryCache = borderGeometry;
        }
        else
        {
            borderGeometryCache = null;
        }

        if (!innerRect.Width.IsZero() && !innerRect.Height.IsZero())
        {
            var innerBorderInfo = new BorderInfo(corners, borders, new Thickness(0), isOuterBorder: false);
            var backgroundGeometry = new StreamGeometry();

            using (var ctx = backgroundGeometry.Open())
            {
                GenerateGeometry(ctx, innerRect, innerBorderInfo);
            }

            backgroundGeometry.Freeze();
            backgroundGeometryCache = backgroundGeometry;
        }
        else
        {
            backgroundGeometryCache = null;
        }

        var child = Child;
        if (child != null)
        {
            child.Arrange(childRect);

            var clipGeometry = new StreamGeometry();
            var childBorderInfo = new BorderInfo(corners, borders, padding, isOuterBorder: false);
            using (var ctx = clipGeometry.Open())
            {
                GenerateGeometry(ctx, new Rect(0, 0, childRect.Width, childRect.Height), childBorderInfo);
            }

            clipGeometry.Freeze();
            child.Clip = clipGeometry;
        }

        return arrangeSize;
    }

    protected override void OnRender(
        DrawingContext drawingContext)
    {
        ArgumentNullException.ThrowIfNull(drawingContext);

        var borders = BorderThickness;
        var borderBrush = BorderBrush;
        var bgBrush = Background;
        var borderGeometry = borderGeometryCache;
        var backgroundGeometry = backgroundGeometryCache;
        var optimizeClipRendering = OptimizeClipRendering;

        if (optimizeClipRendering)
        {
            drawingContext.DrawGeometry(borderBrush, pen: null, borderGeometry);
            return;
        }

        if (borderBrush is not null &&
            !borders.IsZero() &&
            bgBrush is not null)
        {
            if (borderBrush.IsEqualTo(bgBrush))
            {
                drawingContext.DrawGeometry(borderBrush, pen: null, borderGeometry);
            }
            else if (borderBrush.IsOpaqueSolidColorBrush() && bgBrush.IsOpaqueSolidColorBrush())
            {
                drawingContext.DrawGeometry(borderBrush, pen: null, borderGeometry);
                drawingContext.DrawGeometry(bgBrush, pen: null, backgroundGeometry);
            }
            else if (borderBrush.IsOpaqueSolidColorBrush())
            {
                if (borderGeometry is null ||
                    backgroundGeometry is null)
                {
                    return;
                }

                var borderOutlinePath = borderGeometry.GetOutlinedPathGeometry();
                var backgroundOutlinePath = backgroundGeometry.GetOutlinedPathGeometry();
                var borderOutlineGeometry = Geometry.Combine(borderOutlinePath, backgroundOutlinePath, GeometryCombineMode.Exclude, transform: null);

                drawingContext.DrawGeometry(bgBrush, pen: null, borderGeometry);
                drawingContext.DrawGeometry(borderBrush, pen: null, borderOutlineGeometry);
            }
            else
            {
                if (borderGeometry is null ||
                    backgroundGeometry is null)
                {
                    return;
                }

                var borderOutlinePath = borderGeometry.GetOutlinedPathGeometry();
                var backgroundOutlinePath = backgroundGeometry.GetOutlinedPathGeometry();
                var borderOutlineGeometry = Geometry.Combine(borderOutlinePath, backgroundOutlinePath, GeometryCombineMode.Exclude, transform: null);

                drawingContext.DrawGeometry(borderBrush, pen: null, borderOutlineGeometry);
                drawingContext.DrawGeometry(bgBrush, pen: null, backgroundGeometry);
            }

            return;
        }

        if (borderBrush != null &&
            !borders.IsZero())
        {
            if (borderGeometry is not null &&
                backgroundGeometry is not null)
            {
                var borderOutlinePath = borderGeometry.GetOutlinedPathGeometry();
                var backgroundOutlinePath = backgroundGeometry.GetOutlinedPathGeometry();
                var borderOutlineGeometry = Geometry.Combine(borderOutlinePath, backgroundOutlinePath, GeometryCombineMode.Exclude, transform: null);

                drawingContext.DrawGeometry(borderBrush, pen: null, borderOutlineGeometry);
            }
            else
            {
                drawingContext.DrawGeometry(borderBrush, pen: null, borderGeometry);
            }
        }

        if (bgBrush != null)
        {
            drawingContext.DrawGeometry(bgBrush, pen: null, backgroundGeometry);
        }
    }

    private static void GenerateGeometry(
        StreamGeometryContext ctx,
        Rect rect,
        BorderInfo borderInfo)
    {
        var leftTop = new Point(borderInfo.LeftTop, 0);
        var rightTop = new Point(rect.Width - borderInfo.RightTop, 0);
        var topRight = new Point(rect.Width, borderInfo.TopRight);
        var bottomRight = new Point(rect.Width, rect.Height - borderInfo.BottomRight);
        var rightBottom = new Point(rect.Width - borderInfo.RightBottom, rect.Height);
        var leftBottom = new Point(borderInfo.LeftBottom, rect.Height);
        var bottomLeft = new Point(0, rect.Height - borderInfo.BottomLeft);
        var topLeft = new Point(0, borderInfo.TopLeft);

        if (leftTop.X > rightTop.X)
        {
            var v = borderInfo.LeftTop / (borderInfo.LeftTop + borderInfo.RightTop) * rect.Width;
            leftTop.X = v;
            rightTop.X = v;
        }

        if (topRight.Y > bottomRight.Y)
        {
            var v = borderInfo.TopRight / (borderInfo.TopRight + borderInfo.BottomRight) * rect.Height;
            topRight.Y = v;
            bottomRight.Y = v;
        }

        if (leftBottom.X > rightBottom.X)
        {
            var v = borderInfo.LeftBottom / (borderInfo.LeftBottom + borderInfo.RightBottom) * rect.Width;
            rightBottom.X = v;
            leftBottom.X = v;
        }

        if (topLeft.Y > bottomLeft.Y)
        {
            var v = borderInfo.TopLeft / (borderInfo.TopLeft + borderInfo.BottomLeft) * rect.Height;
            bottomLeft.Y = v;
            topLeft.Y = v;
        }

        var offsetX = rect.TopLeft.X;
        var offsetY = rect.TopLeft.Y;
        var offset = new Vector(offsetX, offsetY);
        leftTop += offset;
        rightTop += offset;
        topRight += offset;
        bottomRight += offset;
        rightBottom += offset;
        leftBottom += offset;
        bottomLeft += offset;
        topLeft += offset;

        ctx.BeginFigure(
            leftTop,
            isFilled: true,
            isClosed: true);

        ctx.LineTo(
            rightTop,
            isStroked: true,
            isSmoothJoin: false);

        var radiusX = rect.TopRight.X - rightTop.X;
        var radiusY = topRight.Y - rect.TopRight.Y;
        if (!radiusX.IsZero() || !radiusY.IsZero())
        {
            ctx.ArcTo(
                topRight,
                new Size(radiusX, radiusY),
                rotationAngle: 0,
                isLargeArc: false,
                SweepDirection.Clockwise,
                isStroked: true,
                isSmoothJoin: false);
        }

        ctx.LineTo(
            bottomRight,
            isStroked: true,
            isSmoothJoin: false);

        radiusX = rect.BottomRight.X - rightBottom.X;
        radiusY = rect.BottomRight.Y - bottomRight.Y;
        if (!radiusX.IsZero() || !radiusY.IsZero())
        {
            ctx.ArcTo(
                rightBottom,
                new Size(radiusX, radiusY),
                rotationAngle: 0,
                isLargeArc: false,
                SweepDirection.Clockwise,
                isStroked: true,
                isSmoothJoin: false);
        }

        ctx.LineTo(
            leftBottom,
            isStroked: true,
            isSmoothJoin: false);

        radiusX = leftBottom.X - rect.BottomLeft.X;
        radiusY = rect.BottomLeft.Y - bottomLeft.Y;
        if (!radiusX.IsZero() || !radiusY.IsZero())
        {
            ctx.ArcTo(
                bottomLeft,
                new Size(radiusX, radiusY),
                rotationAngle: 0,
                isLargeArc: false,
                SweepDirection.Clockwise,
                isStroked: true,
                isSmoothJoin: false);
        }

        ctx.LineTo(
            topLeft,
            isStroked: true,
            isSmoothJoin: false);

        radiusX = leftTop.X - rect.TopLeft.X;
        radiusY = topLeft.Y - rect.TopLeft.Y;
        if (!radiusX.IsZero() || !radiusY.IsZero())
        {
            ctx.ArcTo(
                leftTop,
                new Size(radiusX, radiusY),
                rotationAngle: 0,
                isLargeArc: false,
                SweepDirection.Clockwise,
                isStroked: true,
                isSmoothJoin: false);
        }
    }
}