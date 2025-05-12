// ReSharper disable ConvertSwitchStatementToSwitchExpression
// ReSharper disable InvertIf
// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
namespace Atc.Wpf.Controls.Media;

/// <summary>
/// This is the SVG image view control.
/// The image control can either load the image from a file <see cref="SetImage(string)"/> or by
/// setting the <see cref="Drawing"/> object through <see cref="SetImage(Drawing)"/>, which allows
/// multiple controls to share the same drawing instance.
/// </summary>
[SuppressMessage("Style", "IDE0066:Convert switch statement to expression", Justification = "OK.")]
public sealed partial class SvgImage : Control
{
    private readonly TranslateTransform translateTransform = new();
    private readonly ScaleTransform scaleTransform = new();
    private Drawing? drawing;
    private SvgRender? svgRender;
    private Action<SvgRender>? loadImage;

    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyChangedCallback = nameof(OnBackgroundChanged))]
    private Brush? background;

    [DependencyProperty(
        DefaultValue = ControlSizeType.ContentToSizeNoStretch,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyChangedCallback = nameof(OnControlSizeTypeChanged))]
    private ControlSizeType controlSizeType;

    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyChangedCallback = nameof(OnSourceChanged))]
    private string source;

    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyChangedCallback = nameof(OnFileSourceChanged))]
    private string fileSource;

    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyChangedCallback = nameof(OnImageSourceChanged))]
    private Drawing imageSource;

    [DependencyProperty(
        DefaultValue = true)]
    private bool useAnimations;

    [DependencyProperty]
    private Color? overrideColor;

    [DependencyProperty]
    private Color? overrideStrokeColor;

    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyChangedCallback = nameof(OverrideStrokeWidthPropertyChanged))]
    private double? overrideStrokeWidth;

    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender,
        PropertyChangedCallback = nameof(CustomBrushesPropertyChanged))]
    private Dictionary<string, Brush> customBrushes;

    [DependencyProperty(
        DefaultValue = "FileSystemLoader.Instance")]
    private IExternalFileLoader externalFileLoader;

    static SvgImage()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(SvgImage),
            new FrameworkPropertyMetadata(typeof(SvgImage)));
        ClipToBoundsProperty.OverrideMetadata(
            typeof(SvgImage),
            new FrameworkPropertyMetadata(defaultValue: true));
        SnapsToDevicePixelsProperty.OverrideMetadata(
            typeof(SvgImage),
            new FrameworkPropertyMetadata(defaultValue: true));
    }

    public SvgImage()
    {
        ClipToBounds = true;
        SnapsToDevicePixels = true;
    }

    internal Svg? Svg => svgRender?.Svg;

    public void ReRenderSvg()
    {
        if (svgRender?.Svg is not null)
        {
            svgRender.OverrideColor = OverrideColor;
            svgRender.OverrideStrokeColor = OverrideStrokeColor;
            var svgDrawing = svgRender.CreateDrawing(svgRender.Svg);
            SetImage(svgDrawing);
        }
    }

    public void SetImage(
        string svgFileName)
    {
        loadImage = render =>
        {
            SetImage(render.LoadDrawing(svgFileName));
        };

        if (!IsInitialized &&
            !DesignerProperties.GetIsInDesignMode(this))
        {
            return;
        }

        InitializeSvgRender();

        if (svgRender is not null)
        {
            loadImage(svgRender);
        }

        loadImage = null;
    }

    public void SetImage(
        Stream svgStream)
    {
        loadImage = render =>
        {
            SetImage(render.LoadDrawing(svgStream));
        };

        if (!IsInitialized &&
            !DesignerProperties.GetIsInDesignMode(this))
        {
            return;
        }

        InitializeSvgRender();

        if (svgRender is not null)
        {
            loadImage(svgRender);
        }

        loadImage = null;
    }

    public void SetImage(
        Drawing svgDrawing)
    {
        drawing = svgDrawing;
        InvalidateVisual();
        if (drawing is not null)
        {
            InvalidateMeasure();
        }

        ReCalculateImageSize();
    }

    protected override void OnInitialized(
        EventArgs e)
    {
        base.OnInitialized(e);
        if (loadImage is null)
        {
            return;
        }

        InitializeSvgRender();

        if (svgRender is null)
        {
            return;
        }

        loadImage(svgRender);
        loadImage = null;
        var brushesFromSvg = new Dictionary<string, Brush>(StringComparer.Ordinal);
        if (svgRender.Svg is not null)
        {
            foreach (var (key, value) in svgRender.Svg.PaintServers.GetServers())
            {
                var brush = value.GetBrush();
                if (brush is not null)
                {
                    brushesFromSvg[key] = brush;
                }
            }
        }

        CustomBrushes = brushesFromSvg;
    }

    protected override void OnRenderSizeChanged(
        SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        ReCalculateImageSize();
        InvalidateVisual();
    }

    protected override void OnRender(
        DrawingContext drawingContext)
    {
        ArgumentNullException.ThrowIfNull(drawingContext);

        if (drawing is null)
        {
            return;
        }

        if (Background is not null)
        {
            // Notice TemplateBinding background must be removed from the Border in the default template (or remove Border from the template)
            // Border renders the background AFTER the child render has been called
            // http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/1575d2af-8e86-4085-81b8-a8bf24268e51/?prof=required
            drawingContext.DrawRectangle(
                Background,
                pen: null,
                new Rect(0, 0, ActualWidth, ActualHeight));
        }

        drawingContext.PushTransform(translateTransform);
        drawingContext.PushTransform(scaleTransform);
        drawingContext.DrawDrawing(drawing);
        drawingContext.Pop();
        drawingContext.Pop();
    }

    protected override Size MeasureOverride(
        Size constraint)
    {
        var size = base.MeasureOverride(constraint);
        if (ControlSizeType == ControlSizeType.SizeToContent &&
            drawing is not null &&
            !drawing.Bounds.Size.IsEmpty)
        {
            size = drawing.Bounds.Size;
        }

        if (constraint.Width > 0 && constraint.Width < size.Width)
        {
            size.Width = constraint.Width;
        }

        if (constraint.Height > 0 && constraint.Height < size.Height)
        {
            size.Height = constraint.Height;
        }

        return size;
    }

    protected override Size ArrangeOverride(
        Size arrangeBounds)
    {
        var size = base.ArrangeOverride(arrangeBounds);
        if (ControlSizeType == ControlSizeType.SizeToContent &&
            drawing is not null &&
            !drawing.Bounds.Size.IsEmpty)
        {
            size = drawing.Bounds.Size;
        }

        if (arrangeBounds.Width > 0 && arrangeBounds.Width < size.Width)
        {
            size.Width = arrangeBounds.Width;
        }

        if (arrangeBounds.Height > 0 && arrangeBounds.Height < size.Height)
        {
            size.Height = arrangeBounds.Height;
        }

        return size;
    }

    private static void OnBackgroundChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not SvgImage svgImage)
        {
            return;
        }

        svgImage.ReRenderSvg();
    }

    private static void OnControlSizeTypeChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not SvgImage svgImage)
        {
            return;
        }

        svgImage.ReCalculateImageSize();
    }

    private static void OnSourceChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not SvgImage svgImage)
        {
            return;
        }

        var uri = e.NewValue?.ToString();
        if (string.IsNullOrEmpty(uri) || uri.StartsWith("System.", StringComparison.Ordinal))
        {
            return;
        }

        var resource = Application.GetResourceStream(new Uri(uri, UriKind.Relative));
        if (resource is not null)
        {
            svgImage.SetImage(resource.Stream);
        }
    }

    private static void OnFileSourceChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not SvgImage svgImage)
        {
            return;
        }

        var uri = e.NewValue?.ToString();
        if (string.IsNullOrEmpty(uri))
        {
            return;
        }

        using var fileStream = new FileStream(uri, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        var memoryStream = (MemoryStream)fileStream.CopyToStream();
        svgImage.SetImage(memoryStream);
    }

    private static void OnImageSourceChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not SvgImage svgImage)
        {
            return;
        }

        if (e.NewValue is Drawing newDrawing)
        {
            svgImage.SetImage(newDrawing);
        }
    }

    private static void OverrideStrokeWidthPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not SvgImage svgImage ||
            e.NewValue is not double newStrokeWidth ||
            svgImage.svgRender is null)
        {
            return;
        }

        svgImage.svgRender.OverrideStrokeWidth = newStrokeWidth;
        svgImage.InvalidateVisual();
        svgImage.ReRenderSvg();
    }

    private static void CustomBrushesPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not SvgImage svgImage ||
            e.NewValue is not Dictionary<string, Brush> newBrushes)
        {
            return;
        }

        if (svgImage.svgRender is not null)
        {
            if (svgImage.svgRender.CustomBrushes is not null)
            {
                var newCustomBrushes = new Dictionary<string, Brush>(svgImage.svgRender.CustomBrushes, StringComparer.Ordinal);
                foreach (var (key, value) in newBrushes)
                {
                    newCustomBrushes[key] = value;
                }

                svgImage.svgRender.CustomBrushes = newCustomBrushes;
            }
            else
            {
                svgImage.svgRender.CustomBrushes = newBrushes;
            }
        }

        svgImage.InvalidateVisual();
        svgImage.ReRenderSvg();
    }

    private void InitializeSvgRender() =>
        svgRender = new SvgRender
        {
            ExternalFileLoader = ExternalFileLoader,
            CustomBrushes = CustomBrushes,
            OverrideColor = OverrideColor,
            OverrideStrokeColor = OverrideStrokeColor,
            OverrideStrokeWidth = OverrideStrokeWidth,
            UseAnimations = UseAnimations,
        };

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK - for now.")]
    private void ReCalculateImageSize()
    {
        if (drawing is null)
        {
            return;
        }

        var rect = drawing.Bounds;
        switch (ControlSizeType)
        {
            case ControlSizeType.None:
                scaleTransform.ScaleX = 1;
                scaleTransform.ScaleY = 1;
                switch (HorizontalContentAlignment)
                {
                    case HorizontalAlignment.Center:
                        translateTransform.X = (ActualWidth / 2) - (rect.Width / 2) - rect.Left;
                        break;
                    case HorizontalAlignment.Right:
                        translateTransform.X = ActualWidth - rect.Right;
                        break;
                    default:
                        // Move to left by default
                        translateTransform.X = -rect.Left;
                        break;
                }

                switch (VerticalContentAlignment)
                {
                    case VerticalAlignment.Center:
                        translateTransform.Y = (ActualHeight / 2) - (rect.Height / 2);
                        break;
                    case VerticalAlignment.Bottom:
                        translateTransform.Y = ActualHeight - rect.Height - rect.Top;
                        break;
                    default:
                        // Move to top by default
                        translateTransform.Y = -rect.Top;
                        break;
                }

                break;
            case ControlSizeType.ContentToSizeNoStretch:
                SizeToContentNoStretch();
                break;
            case ControlSizeType.ContentToSizeStretch:
                var xScale = ActualWidth / rect.Width;
                var yScale = ActualHeight / rect.Height;
                scaleTransform.CenterX = rect.Left;
                scaleTransform.CenterY = rect.Top;
                scaleTransform.ScaleX = xScale;
                scaleTransform.ScaleY = yScale;

                // Move to top-left by default
                translateTransform.X = -rect.Left;
                translateTransform.Y = -rect.Top;
                break;
            case ControlSizeType.SizeToContent when rect.Width > ActualWidth || rect.Height > ActualHeight:
                SizeToContentNoStretch();
                break;
            case ControlSizeType.SizeToContent:
                scaleTransform.CenterX = rect.Left;
                scaleTransform.CenterY = rect.Top;
                scaleTransform.ScaleX = 1;
                scaleTransform.ScaleY = 1;

                // Move to top-left by default
                translateTransform.X = -rect.Left;
                translateTransform.Y = -rect.Top;
                break;
            default:
                throw new SwitchExpressionException(ControlSizeType);
        }
    }

    private void SizeToContentNoStretch()
    {
        var rect = drawing!.Bounds;
        var xScale = ActualWidth / rect.Width;
        var yScale = ActualHeight / rect.Height;
        var scale = xScale;
        if (scale > yScale)
        {
            scale = yScale;
        }

        scaleTransform.CenterX = rect.Left;
        scaleTransform.CenterY = rect.Top;
        scaleTransform.ScaleX = scale;
        scaleTransform.ScaleY = scale;

        translateTransform.X = -rect.Left;
        if (scale < xScale)
        {
            switch (HorizontalContentAlignment)
            {
                case HorizontalAlignment.Center:
                    var width = rect.Width * scale;
                    translateTransform.X = (ActualWidth / 2) - (width / 2) - rect.Left;
                    break;
                case HorizontalAlignment.Right:
                    translateTransform.X = ActualWidth - (rect.Right * scale);
                    break;
            }
        }

        translateTransform.Y = -rect.Top;
        if (scale < yScale)
        {
            switch (VerticalContentAlignment)
            {
                case VerticalAlignment.Center:
                    var height = rect.Height * scale;
                    translateTransform.Y = (ActualHeight / 2) - (height / 2) - rect.Top;
                    break;
                case VerticalAlignment.Bottom:
                    translateTransform.Y = ActualHeight - (rect.Height * scale) - rect.Top;
                    break;
            }
        }
    }
}