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
public sealed class SvgImage : Control
{
    /// <summary>Identifies the <see cref="Background"/> dependency property.</summary>
    public static new readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
        nameof(Background),
        typeof(Brush),
        typeof(SvgImage),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, OnBackgroundChanged));

    /// <summary>Identifies the <see cref="ControlSizeType"/> dependency property.</summary>
    public static readonly DependencyProperty ControlSizeTypeProperty = DependencyProperty.Register(
        nameof(ControlSizeType),
        typeof(ControlSizeType),
        typeof(SvgImage),
        new FrameworkPropertyMetadata(ControlSizeType.ContentToSizeNoStretch, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, OnControlSizeTypeChanged));

    /// <summary>Identifies the <see cref="Source"/> dependency property.</summary>
    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
        nameof(Source),
        typeof(string),
        typeof(SvgImage),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, OnSourceChanged));

    /// <summary>Identifies the <see cref="FileSource"/> dependency property.</summary>
    public static readonly DependencyProperty FileSourceProperty = DependencyProperty.Register(
        nameof(FileSource),
        typeof(string),
        typeof(SvgImage),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, OnFileSourceChanged));

    /// <summary>Identifies the <see cref="ImageSource"/> dependency property.</summary>
    public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
        nameof(ImageSource),
        typeof(Drawing),
        typeof(SvgImage),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, OnImageSourceChanged));

    /// <summary>Identifies the <see cref="UseAnimations"/> dependency property.</summary>
    public static readonly DependencyProperty UseAnimationsProperty = DependencyProperty.Register(
        nameof(UseAnimations),
        typeof(bool),
        typeof(SvgImage),
        new PropertyMetadata(true));

    /// <summary>Identifies the <see cref="OverrideColor"/> dependency property.</summary>
    public static readonly DependencyProperty OverrideColorProperty = DependencyProperty.Register(
        nameof(OverrideColor),
        typeof(Color?),
        typeof(SvgImage),
        new FrameworkPropertyMetadata(default(Color?), FrameworkPropertyMetadataOptions.AffectsRender, OverrideColorPropertyChanged));

    /// <summary>Identifies the <see cref="OverrideStrokeColor"/> dependency property.</summary>
    public static readonly DependencyProperty OverrideStrokeColorProperty = DependencyProperty.Register(
        nameof(OverrideStrokeColor),
        typeof(Color?),
        typeof(SvgImage),
        new FrameworkPropertyMetadata(default(Color?), FrameworkPropertyMetadataOptions.AffectsRender, OverrideStrokeColorPropertyChanged));

    /// <summary>Identifies the <see cref="OverrideStrokeWidth"/> dependency property.</summary>
    public static readonly DependencyProperty OverrideStrokeWidthProperty = DependencyProperty.Register(
        nameof(OverrideStrokeWidth),
        typeof(double?),
        typeof(SvgImage),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OverrideStrokeWidthPropertyChanged));

    /// <summary>Identifies the <see cref="CustomBrushes"/> dependency property.</summary>
    public static readonly DependencyProperty CustomBrushesProperty = DependencyProperty.Register(
        nameof(CustomBrushes),
        typeof(Dictionary<string, Brush>),
        typeof(SvgImage),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, CustomBrushesPropertyChanged));

    /// <summary>Identifies the <see cref="ExternalFileLoader"/> dependency property.</summary>
    public static readonly DependencyProperty ExternalFileLoaderProperty = DependencyProperty.Register(
        nameof(ExternalFileLoader),
        typeof(IExternalFileLoader),
        typeof(SvgImage),
        new PropertyMetadata(FileSystemLoader.Instance));

    private readonly TranslateTransform translateTransform = new();
    private readonly ScaleTransform scaleTransform = new();
    private Drawing? drawing;
    private SvgRender? svgRender;
    private Action<SvgRender>? loadImage;
    private bool isExposingSvgBrushes;
    private SvgSource? source;
    private bool loadSourceOnInitialized;
    private bool isDrawingFromCache;
    private bool hasApplicationCustomBrushes;

    /// <summary>
    /// Gets or sets the brush drawn behind the SVG image.
    /// </summary>
    public new Brush? Background
    {
        get => (Brush?)GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets how the SVG drawing is sized relative to the control.
    /// </summary>
    public ControlSizeType ControlSizeType
    {
        get => (ControlSizeType)GetValue(ControlSizeTypeProperty);
        set => SetValue(ControlSizeTypeProperty, value);
    }

    /// <summary>
    /// Gets or sets the relative URI of an application resource containing the SVG image.
    /// </summary>
    public string Source
    {
        get => (string)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the path of an SVG file on disk to display.
    /// </summary>
    public string FileSource
    {
        get => (string)GetValue(FileSourceProperty);
        set => SetValue(FileSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets a pre-rendered <see cref="Drawing"/> to display.
    /// </summary>
    public Drawing ImageSource
    {
        get => (Drawing)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether SVG animations are rendered.
    /// </summary>
    public bool UseAnimations
    {
        get => (bool)GetValue(UseAnimationsProperty);
        set => SetValue(UseAnimationsProperty, value);
    }

    /// <summary>
    /// Gets or sets an optional color that replaces the fill (and, unless <see cref="OverrideStrokeColor"/> is set, stroke) colors of the SVG.
    /// </summary>
    public Color? OverrideColor
    {
        get => (Color?)GetValue(OverrideColorProperty);
        set => SetValue(OverrideColorProperty, value);
    }

    /// <summary>
    /// Gets or sets an optional color that replaces the stroke colors of the SVG.
    /// </summary>
    public Color? OverrideStrokeColor
    {
        get => (Color?)GetValue(OverrideStrokeColorProperty);
        set => SetValue(OverrideStrokeColorProperty, value);
    }

    /// <summary>
    /// Gets or sets an optional width that replaces the stroke widths of the SVG.
    /// </summary>
    public double? OverrideStrokeWidth
    {
        get => (double?)GetValue(OverrideStrokeWidthProperty);
        set => SetValue(OverrideStrokeWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets named brushes that replace the matching brushes used by the SVG.
    /// </summary>
    public Dictionary<string, Brush> CustomBrushes
    {
        get => (Dictionary<string, Brush>)GetValue(CustomBrushesProperty);
        set => SetValue(CustomBrushesProperty, value);
    }

    /// <summary>
    /// Gets or sets the loader used to resolve external files referenced by the SVG.
    /// </summary>
    public IExternalFileLoader ExternalFileLoader
    {
        get => (IExternalFileLoader)GetValue(ExternalFileLoaderProperty);
        set => SetValue(ExternalFileLoaderProperty, value);
    }

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

    /// <summary>
    /// Initializes a new instance of the <see cref="SvgImage"/> class.
    /// </summary>
    public SvgImage()
    {
        ClipToBounds = true;
        SnapsToDevicePixels = true;
    }

    internal Svg? Svg => svgRender?.Svg;

    internal Drawing? CurrentDrawing => drawing;

    internal bool IsDrawingFromCache => isDrawingFromCache;

    /// <summary>
    /// Renders the current SVG again using the current override and brush settings.
    /// </summary>
    public void ReRenderSvg()
    {
        if (svgRender?.Svg is null)
        {
            // A drawing shared from the cache has no parsed SVG of its own, so load one for this image.
            if (isDrawingFromCache)
            {
                LoadAndRenderSource();
            }

            return;
        }

        RenderWithCurrentSettings();
    }

    /// <summary>
    /// Loads and displays the SVG image from the specified file.
    /// </summary>
    /// <param name="svgFileName">The path of the SVG file to load.</param>
    public void SetImage(string svgFileName)
    {
        ClearSource();
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

    /// <summary>
    /// Loads and displays the SVG image from the specified stream.
    /// </summary>
    /// <param name="svgStream">The stream containing the SVG content.</param>
    public void SetImage(Stream svgStream)
    {
        ClearSource();
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

    /// <summary>
    /// Displays the specified drawing, which may be shared between multiple controls.
    /// </summary>
    /// <param name="svgDrawing">The drawing to display.</param>
    public void SetImage(Drawing svgDrawing)
    {
        if (!ReferenceEquals(svgDrawing, drawing))
        {
            ClearSource();
        }

        ShowDrawing(svgDrawing);
    }

    private void ShowDrawing(Drawing svgDrawing)
    {
        drawing = svgDrawing;
        InvalidateVisual();
        if (drawing is not null)
        {
            InvalidateMeasure();
        }

        ReCalculateImageSize();
    }

    /// <inheritdoc />
    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);
        if (loadSourceOnInitialized)
        {
            loadSourceOnInitialized = false;
            LoadSource();
            return;
        }

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
        ExposeSvgBrushes();
    }

    /// <inheritdoc />
    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        ReCalculateImageSize();
        InvalidateVisual();
    }

    /// <inheritdoc />
    protected override void OnRender(DrawingContext drawingContext)
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

    /// <inheritdoc />
    protected override Size MeasureOverride(Size constraint)
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

    /// <inheritdoc />
    protected override Size ArrangeOverride(Size arrangeBounds)
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

        svgImage.SetSource(new SvgSource(
            $"resource:{uri}",
            () => Application.GetResourceStream(new Uri(uri, UriKind.Relative))?.Stream));
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

        var fileInfo = new FileInfo(uri);
        if (!fileInfo.Exists)
        {
            throw new FileNotFoundException($"Could not find file '{fileInfo.FullName}'.", fileInfo.FullName);
        }

        // The file can change on disk, so its identity includes the write time and size. It is only opened when
        // the image is not in the drawing cache.
        var fullName = fileInfo.FullName;
        svgImage.SetSource(new SvgSource(
            $"file:{fullName.ToUpperInvariant()}|{fileInfo.LastWriteTimeUtc.Ticks}|{fileInfo.Length}",
            () => new FileStream(fullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)));
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

    private static void OverrideColorPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not SvgImage svgImage)
        {
            return;
        }

        svgImage.RefreshForOverrides();
    }

    private static void OverrideStrokeColorPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not SvgImage svgImage)
        {
            return;
        }

        svgImage.RefreshForOverrides();
    }

    private static void OverrideStrokeWidthPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not SvgImage svgImage ||
            e.NewValue is not double)
        {
            return;
        }

        svgImage.InvalidateVisual();
        svgImage.RefreshForOverrides();
    }

    private static void CustomBrushesPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not SvgImage svgImage ||
            svgImage.isExposingSvgBrushes ||
            e.NewValue is not Dictionary<string, Brush> newBrushes)
        {
            return;
        }

        svgImage.hasApplicationCustomBrushes = newBrushes.Count > 0;
        if (svgImage.svgRender is null && svgImage.isDrawingFromCache)
        {
            // A drawing shared from the cache: parse the source for this image, with the new brushes.
            svgImage.LoadAndRenderSource();
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

    private bool CanUseDrawingCache
        => source is not null &&
           !hasApplicationCustomBrushes &&
           ReferenceEquals(ExternalFileLoader, FileSystemLoader.Instance);

    private SvgDrawingCache.Key CreateCacheKey()
        => new(
            source!.Identity,
            OverrideColor,
            OverrideStrokeColor,
            OverrideStrokeWidth,
            UseAnimations);

    private void ClearSource()
    {
        source = null;
        loadSourceOnInitialized = false;
        isDrawingFromCache = false;
    }

    private void SetSource(SvgSource newSource)
    {
        source = newSource;
        loadImage = null;

        if (!IsInitialized &&
            !DesignerProperties.GetIsInDesignMode(this))
        {
            loadSourceOnInitialized = true;
            return;
        }

        LoadSource();
    }

    private void LoadSource()
    {
        if (CanUseDrawingCache &&
            SvgDrawingCache.TryGet(CreateCacheKey(), out var entry))
        {
            ShowCachedDrawing(entry);
            return;
        }

        LoadAndRenderSource();
    }

    private void LoadAndRenderSource()
    {
        if (source is null)
        {
            return;
        }

        using var stream = source.Open();
        if (stream is null)
        {
            return;
        }

        var addToCache = CanUseDrawingCache;
        InitializeSvgRender();
        var svgDrawing = svgRender!.LoadDrawing(stream);
        isDrawingFromCache = false;
        ShowDrawing(svgDrawing);
        var paintServerBrushes = ExposeSvgBrushes();

        if (addToCache)
        {
            SvgDrawingCache.TryAdd(CreateCacheKey(), svgDrawing, paintServerBrushes);
        }
    }

    private void ShowCachedDrawing(SvgDrawingCache.Entry entry)
    {
        svgRender = null;
        isDrawingFromCache = true;
        ShowDrawing(entry.Drawing);

        // The cached brushes are frozen; the image gets its own copies so they can be changed.
        var brushes = new Dictionary<string, Brush>(StringComparer.Ordinal);
        foreach (var (key, brush) in entry.PaintServerBrushes)
        {
            brushes[key] = brush.Clone();
        }

        SetExposedCustomBrushes(brushes);
    }

    /// <summary>
    /// An override changed after the image was loaded. A cached drawing for the new settings is reused (a theme
    /// switch changes the same override on every icon), otherwise the image is rendered again.
    /// </summary>
    private void RefreshForOverrides()
    {
        var isLoadedFromSource = source is not null && (isDrawingFromCache || svgRender?.Svg is not null);
        if (!isLoadedFromSource)
        {
            ReRenderSvg();
            return;
        }

        if (CanUseDrawingCache &&
            SvgDrawingCache.TryGet(CreateCacheKey(), out var entry))
        {
            ShowCachedDrawing(entry);
            return;
        }

        if (svgRender?.Svg is null)
        {
            LoadAndRenderSource();
            return;
        }

        RenderWithCurrentSettings();
    }

    private void RenderWithCurrentSettings()
    {
        svgRender!.OverrideColor = OverrideColor;
        svgRender.OverrideStrokeColor = OverrideStrokeColor;
        svgRender.OverrideStrokeWidth = OverrideStrokeWidth;
        var svgDrawing = svgRender.CreateDrawing(svgRender.Svg!);
        isDrawingFromCache = false;
        ShowDrawing(svgDrawing);
    }

    /// <summary>
    /// Exposes the SVG's paint servers through <see cref="CustomBrushes"/>, keeping any brush the application set.
    /// The image was just rendered with exactly these brushes, so this does not render it again.
    /// </summary>
    private Dictionary<string, Brush> ExposeSvgBrushes()
    {
        var brushesFromSvg = new Dictionary<string, Brush>(StringComparer.Ordinal);
        if (svgRender?.Svg is not null)
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

        if (CustomBrushes is not null)
        {
            foreach (var (key, value) in CustomBrushes)
            {
                brushesFromSvg[key] = value;
            }
        }

        SetExposedCustomBrushes(brushesFromSvg);
        return brushesFromSvg;
    }

    private void SetExposedCustomBrushes(Dictionary<string, Brush> brushes)
    {
        isExposingSvgBrushes = true;
        try
        {
            // The renderer shares the dictionary, so editing CustomBrushes in place and calling ReRenderSvg works.
            CustomBrushes = brushes;
            if (svgRender is not null)
            {
                svgRender.CustomBrushes = brushes;
            }
        }
        finally
        {
            isExposingSvgBrushes = false;
        }
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

    /// <summary>
    /// Where the image was loaded from: an identity for the drawing cache, and a way to read the SVG again.
    /// </summary>
    private sealed record SvgSource(
        string Identity,
        Func<Stream?> Open);
}