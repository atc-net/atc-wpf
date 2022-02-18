namespace Atc.Wpf.Controls.W3cSvg;

internal class SvgRender
{
    public SvgRender()
        : this(FileSystemLoader.Instance)
    {
    }

    public SvgRender(IExternalFileLoader? fileLoader)
    {
        ExternalFileLoader = fileLoader ?? FileSystemLoader.Instance;
    }

    public Svg? Svg { get; private set; }

    public bool UseAnimations { get; init; }

    public Color? OverrideColor { get; init; }

    public double? OverrideStrokeWidth { get; set; }

    private Dictionary<string, Brush>? customBrushes;

    public Dictionary<string, Brush>? CustomBrushes
    {
        get => this.customBrushes;
        set
        {
            this.customBrushes = value;
            if (this.Svg is not null)
            {
                this.Svg.CustomBrushes = value;
            }
        }
    }

    public IExternalFileLoader ExternalFileLoader { get; init; }

    public DrawingGroup LoadXmlDrawing(string xmlFile)
    {
        this.Svg = new Svg(this.ExternalFileLoader);
        this.Svg.LoadXml(xmlFile);
        return this.CreateDrawing(this.Svg);
    }

    public DrawingGroup LoadDrawing(string fileName)
    {
        this.Svg = new Svg(fileName, ExternalFileLoader);
        return this.CreateDrawing(this.Svg);
    }

    public DrawingGroup LoadDrawing(Uri fileUri)
    {
        this.Svg = new Svg(this.ExternalFileLoader);
        this.Svg.Load(fileUri);
        return this.CreateDrawing(this.Svg);
    }

    public DrawingGroup LoadDrawing(TextReader textReader)
    {
        this.Svg = new Svg(this.ExternalFileLoader);
        this.Svg.Load(textReader);
        return this.CreateDrawing(this.Svg);
    }

    public DrawingGroup LoadDrawing(XmlReader xmlReader)
    {
        this.Svg = new Svg(this.ExternalFileLoader);
        this.Svg.Load(xmlReader);
        return this.CreateDrawing(this.Svg);
    }

    public DrawingGroup LoadDrawing(Stream stream)
    {
        this.Svg = new Svg(stream, ExternalFileLoader);
        return this.CreateDrawing(this.Svg);
    }

    public DrawingGroup CreateDrawing(Svg svg)
    {
        if (svg is null)
        {
            throw new ArgumentNullException(nameof(svg));
        }

        return this.LoadGroup(svg.Elements, svg.ViewBox, isSwitch: false);
    }

    public DrawingGroup CreateDrawing(Shape shape)
    {
        return this.LoadGroup(new[] { shape }, viewBox: null, isSwitch: false);
    }

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK - for now.")]
    private GeometryDrawing NewDrawingItem(Shape shape, Geometry geometry)
    {
        shape.GeometryElement = geometry;
        var geometryDrawing = new GeometryDrawing();
        var stroke = shape.Stroke;
        if (stroke is not null)
        {
            if (OverrideStrokeWidth.HasValue)
            {
                stroke.Width = OverrideStrokeWidth.Value;
            }

            var brush = stroke.StrokeBrush(this.Svg, this, shape, shape.Opacity, geometry.Bounds);
            if (OverrideColor is not null)
            {
                brush = new SolidColorBrush(Color.FromArgb((byte)(255 * shape.Opacity), OverrideColor.Value.R, OverrideColor.Value.G, OverrideColor.Value.B));
            }

            geometryDrawing.Pen = new Pen(brush, stroke.Width);
            if (stroke.StrokeArray is not null)
            {
                geometryDrawing.Pen.DashCap = PenLineCap.Flat;
                DashStyle ds = new DashStyle();
                double scale = 1 / stroke.Width;
                foreach (var d in stroke.StrokeArray)
                {
                    var dash = (int)d;
                    ds.Dashes.Add(dash * scale);
                }

                geometryDrawing.Pen.DashStyle = ds;
            }

            switch (stroke.LineCap)
            {
                case StrokeLineCapType.Butt:
                    geometryDrawing.Pen.StartLineCap = PenLineCap.Flat;
                    geometryDrawing.Pen.EndLineCap = PenLineCap.Flat;
                    break;
                case StrokeLineCapType.Round:
                    geometryDrawing.Pen.StartLineCap = PenLineCap.Round;
                    geometryDrawing.Pen.EndLineCap = PenLineCap.Round;
                    break;
                case StrokeLineCapType.Square:
                    geometryDrawing.Pen.StartLineCap = PenLineCap.Square;
                    geometryDrawing.Pen.EndLineCap = PenLineCap.Square;
                    break;
            }

            switch (stroke.LineJoin)
            {
                case StrokeLineJoinType.Round:
                    geometryDrawing.Pen.LineJoin = PenLineJoin.Round;
                    break;
                case StrokeLineJoinType.Miter:
                    geometryDrawing.Pen.LineJoin = PenLineJoin.Miter;
                    break;
                case StrokeLineJoinType.Bevel:
                    geometryDrawing.Pen.LineJoin = PenLineJoin.Bevel;
                    break;
            }
        }

        if (shape.Fill is null)
        {
            geometryDrawing.Brush = Brushes.Black;
            if (OverrideColor is not null)
            {
                geometryDrawing.Brush = new SolidColorBrush(Color.FromArgb((byte)(255 * shape.Opacity), OverrideColor.Value.R, OverrideColor.Value.G, OverrideColor.Value.B));
            }

            var g = new GeometryGroup
            {
                FillRule = FillRule.Nonzero,
            };

            g.Children.Add(geometry);
            geometry = g;
        }
        else
        {
            geometryDrawing.Brush = shape.Fill.FillBrush(this.Svg, this, shape, shape.Opacity, geometry.Bounds);
            if (OverrideColor is not null)
            {
                geometryDrawing.Brush = new SolidColorBrush(Color.FromArgb((byte)(255 * shape.Opacity), OverrideColor.Value.R, OverrideColor.Value.G, OverrideColor.Value.B));
            }

            var g = new GeometryGroup
            {
                FillRule = FillRule.Nonzero,
            };

            if (shape.Fill.FillRule == FillRuleType.EvenOdd)
            {
                g.FillRule = FillRule.EvenOdd;
            }

            g.Children.Add(geometry);
            geometry = g;
        }

        geometryDrawing.Geometry = geometry;
        return geometryDrawing;
    }

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK - for now.")]
    internal DrawingGroup LoadGroup(IEnumerable<Shape> elements, Rect? viewBox, bool isSwitch)
    {
        var drawingGroup = new DrawingGroup();
        if (viewBox.HasValue)
        {
            drawingGroup.ClipGeometry = new RectangleGeometry(viewBox.Value);
        }

        foreach (Shape shape in elements)
        {
            shape.RealParent = null;
            if (!shape.Display)
            {
                continue;
            }

            if (isSwitch)
            {
                if (drawingGroup.Children.Count > 0)
                {
                    break;
                }

                if (!string.IsNullOrEmpty(shape.RequiredFeatures))
                {
                    if (!SvgFeatures.Features.Contains(shape.RequiredFeatures, StringComparer.Ordinal))
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(shape.RequiredExtensions))
                    {
                        continue;
                    }
                }
            }

            switch (shape)
            {
                case AnimationBase:
                {
                    if (UseAnimations)
                    {
                        switch (shape)
                        {
                            case AnimateTransform animateTransform:
                            {
                                if (animateTransform.Type == AnimateTransformType.Rotate)
                                {
                                    var animation = new DoubleAnimation
                                    {
                                        From = double.Parse(animateTransform.From, GlobalizationConstants.EnglishCultureInfo),
                                        To = double.Parse(animateTransform.To, GlobalizationConstants.EnglishCultureInfo),
                                        Duration = animateTransform.Duration,
                                        RepeatBehavior = RepeatBehavior.Forever,
                                    };

                                    var r = new RotateTransform();
                                    drawingGroup.Transform = r;
                                    r.BeginAnimation(RotateTransform.AngleProperty, animation);
                                }

                                break;
                            }

                            case Animate animate:
                            {
                                var target = this.Svg!.GetShape(animate.Href);
                                var g = target?.GeometryElement;
                                if (g is null)
                                {
                                    continue;
                                }

                                switch (animate.AttributeName)
                                {
                                    case "r":
                                    {
                                        var animation = new DoubleAnimationUsingKeyFrames
                                        {
                                            Duration = animate.Duration,
                                        };

                                        if (animate.Values is not null)
                                        {
                                            foreach (var d in animate.Values
                                                         .Split(';')
                                                         .Select(x => new LinearDoubleKeyFrame(double.Parse(x, GlobalizationConstants.EnglishCultureInfo))))
                                            {
                                                animation.KeyFrames.Add(d);
                                            }
                                        }

                                        animation.RepeatBehavior = RepeatBehavior.Forever;
                                        g.BeginAnimation(EllipseGeometry.RadiusXProperty, animation);
                                        g.BeginAnimation(EllipseGeometry.RadiusYProperty, animation);
                                        break;
                                    }

                                    case "cx":
                                    {
                                        var animation = new PointAnimationUsingKeyFrames
                                        {
                                            Duration = animate.Duration,
                                        };

                                        if (animate.Values is not null)
                                        {
                                            foreach (var d in animate.Values
                                                         .Split(';')
                                                         .Select(_ => new LinearPointKeyFrame(new Point(double.Parse(_, GlobalizationConstants.EnglishCultureInfo), ((EllipseGeometry)g)!.Center.Y))))
                                            {
                                                animation.KeyFrames.Add(d);
                                            }
                                        }

                                        animation.RepeatBehavior = RepeatBehavior.Forever;
                                        g.BeginAnimation(EllipseGeometry.CenterProperty, animation);
                                        break;
                                    }

                                    case "cy":
                                    {
                                        var animation = new PointAnimationUsingKeyFrames
                                        {
                                            Duration = animate.Duration,
                                        };

                                        if (animate.Values is not null)
                                        {
                                            foreach (var d in animate.Values
                                                         .Split(';')
                                                         .Select(_ => new LinearPointKeyFrame(new Point(((EllipseGeometry)g)!.Center.X, double.Parse(_, GlobalizationConstants.EnglishCultureInfo)))))
                                            {
                                                animation.KeyFrames.Add(d);
                                            }
                                        }

                                        animation.RepeatBehavior = RepeatBehavior.Forever;
                                        g.BeginAnimation(EllipseGeometry.CenterProperty, animation);
                                        break;
                                    }
                                }

                                break;
                            }
                        }
                    }

                    continue;
                }

                case UseShape useShape:
                {
                    var currentUsedShape = this.Svg!.GetShape(useShape.Href);
                    if (currentUsedShape is not null)
                    {
                        currentUsedShape.RealParent = useShape;
                        var oldParent = currentUsedShape.Parent;
                        DrawingGroup subgroup;
                        if (currentUsedShape is Shapes.Group group)
                        {
                            subgroup = this.LoadGroup(group.Elements, viewBox: null, isSwitch: false);
                        }
                        else
                        {
                            subgroup = this.LoadGroup(new[] { currentUsedShape }, viewBox: null, isSwitch: false);
                        }

                        if (currentUsedShape.Clip is not null)
                        {
                            subgroup.ClipGeometry = currentUsedShape.Clip.ClipGeometry;
                        }

                        subgroup.Transform = new TranslateTransform(useShape.X, useShape.Y);
                        if (useShape.Transform is not null)
                        {
                            subgroup.Transform = new TransformGroup
                            {
                                Children = new TransformCollection
                                {
                                    subgroup.Transform, useShape.Transform,
                                },
                            };
                        }

                        drawingGroup.Children.Add(subgroup);
                        currentUsedShape.Parent = oldParent;
                    }

                    continue;
                }

                case Clip clip:
                {
                    DrawingGroup subgroup = this.LoadGroup(clip.Elements, viewBox: null, isSwitch: false);
                    if (clip.Transform is not null)
                    {
                        subgroup.Transform = clip.Transform;
                    }

                    drawingGroup.Children.Add(subgroup);
                    continue;
                }

                case Shapes.Group groupShape:
                {
                    DrawingGroup subgroup = this.LoadGroup(groupShape.Elements, viewBox: null, groupShape.IsSwitch);
                    AddDrawingToGroup(drawingGroup, groupShape, subgroup);
                    continue;
                }

                case RectangleShape rectangleShape:
                {
                    double dx = rectangleShape.X;
                    double dy = rectangleShape.Y;
                    double width = rectangleShape.Width;
                    double height = rectangleShape.Height;
                    double rx = rectangleShape.Rx;
                    double ry = rectangleShape.Ry;
                    if (width <= 0 || height <= 0)
                    {
                        continue;
                    }

                    switch (rx)
                    {
                        case <= 0 when ry > 0:
                            rx = ry;
                            break;
                        case > 0 when ry <= 0:
                            ry = rx;
                            break;
                    }

                    RectangleGeometry rect = new RectangleGeometry(new Rect(dx, dy, width, height), rx, ry);
                    var di = this.NewDrawingItem(rectangleShape, rect);
                    AddDrawingToGroup(drawingGroup, rectangleShape, di);
                    continue;
                }

                case LineShape lineShape:
                {
                    var lineGeometry = new LineGeometry(lineShape.P1, lineShape.P2);
                    var di = this.NewDrawingItem(lineShape, lineGeometry);
                    AddDrawingToGroup(drawingGroup, lineShape, di);
                    continue;
                }

                case PolylineShape polylineShape:
                {
                    var pathGeometry = new PathGeometry();
                    var pathFigure = new PathFigure();
                    pathGeometry.Figures.Add(pathFigure);
                    pathFigure.IsClosed = false;
                    pathFigure.StartPoint = polylineShape.Points[0];
                    for (int index = 1; index < polylineShape.Points.Length; index++)
                    {
                        pathFigure.Segments.Add(new LineSegment(polylineShape.Points[index], isStroked: true));
                    }

                    var di = this.NewDrawingItem(polylineShape, pathGeometry);
                    AddDrawingToGroup(drawingGroup, polylineShape, di);
                    continue;
                }

                case PolygonShape polygonShape:
                {
                    var pathGeometry = new PathGeometry();
                    var pathFigure = new PathFigure();
                    pathGeometry.Figures.Add(pathFigure);
                    pathFigure.IsClosed = true;
                    pathFigure.StartPoint = polygonShape.Points[0];
                    for (int index = 1; index < polygonShape.Points.Length; index++)
                    {
                        pathFigure.Segments.Add(new LineSegment(polygonShape.Points[index], isStroked: true));
                    }

                    var di = this.NewDrawingItem(polygonShape, pathGeometry);
                    AddDrawingToGroup(drawingGroup, polygonShape, di);
                    continue;
                }

                case CircleShape circleShape:
                {
                    var ellipseGeometry = new EllipseGeometry(new Point(circleShape.Cx, circleShape.Cy), circleShape.R, circleShape.R);
                    var di = this.NewDrawingItem(circleShape, ellipseGeometry);
                    AddDrawingToGroup(drawingGroup, circleShape, di);
                    continue;
                }

                case EllipseShape ellipseShape:
                {
                    var ellipseGeometry = new EllipseGeometry(new Point(ellipseShape.Cx, ellipseShape.Cy), ellipseShape.Rx, ellipseShape.Ry);
                    var di = this.NewDrawingItem(ellipseShape, ellipseGeometry);
                    AddDrawingToGroup(drawingGroup, ellipseShape, di);
                    continue;
                }

                case ImageShape image:
                {
                    var imageDrawing = new ImageDrawing(image.ImageSource, new Rect(image.X, image.Y, image.Width, image.Height));
                    AddDrawingToGroup(drawingGroup, image, imageDrawing);
                    continue;
                }

                case TextShape textShape:
                {
                    var gp = TextRender.BuildTextGeometry(textShape);
                    foreach (Geometry gm in gp.Children)
                    {
                        var tSpan = TextRender.GetElement(gm);

                        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                        if (tSpan is null)
                        {
                            var di = this.NewDrawingItem(textShape, gm);
                            AddDrawingToGroup(drawingGroup, textShape, di);
                        }
                        else
                        {
                            var di = this.NewDrawingItem(tSpan, gm);
                            AddDrawingToGroup(drawingGroup, textShape, di);
                        }
                    }

                    continue;
                }

                case PathShape pathShape:
                {
                    var path = PathGeometry.CreateFromGeometry(Geometry.Parse(pathShape.Data));
                    var di = this.NewDrawingItem(pathShape, path);
                    AddDrawingToGroup(drawingGroup, pathShape, di);
                    break;
                }
            }
        }

        return drawingGroup;
    }

    private static void AddDrawingToGroup(DrawingGroup grp, Shape shape, Drawing drawing)
    {
        if (shape.Clip is not null || shape.Transform is not null || shape.Filter is not null)
        {
            var drawingGroup = new DrawingGroup();
            if (shape.Clip is not null)
            {
                drawingGroup.ClipGeometry = shape.Clip.ClipGeometry;
            }

            if (shape.Transform is not null)
            {
                drawingGroup.Transform = shape.Transform;
            }

            if (shape.Filter is not null)
            {
                drawingGroup.BitmapEffect = shape.Filter.GetBitmapEffect();
            }

            drawingGroup.Children.Add(drawing);
            grp.Children.Add(drawingGroup);
        }
        else
        {
            grp.Children.Add(drawing);
        }
    }
}