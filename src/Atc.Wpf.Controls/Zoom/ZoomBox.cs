// ReSharper disable ConvertIfStatementToSwitchStatement
namespace Atc.Wpf.Controls.Zoom;

/// <summary>
/// A control that wraps content and provides zoom and pan functionality.
/// </summary>
[SuppressMessage("Design", "MA0048:File name must match type name", Justification = "OK - partial class")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1601:Partial elements should be documented", Justification = "OK - partial class")]
[TemplatePart(Name = "PART_Content", Type = typeof(FrameworkElement))]
[TemplatePart(Name = "PART_DragZoomCanvas", Type = typeof(Canvas))]
[TemplatePart(Name = "PART_DragZoomBorder", Type = typeof(Border))]
[DefaultStyleKey]
public partial class ZoomBox : ContentControl, IScrollInfo, INotifyPropertyChanged
{
    private FrameworkElement? content;
    private ScaleTransform? contentZoomTransform;
    private TranslateTransform? contentOffsetTransform;
    private double constrainedContentViewportHeight;
    private double constrainedContentViewportWidth;
    private bool disableContentFocusSync;
    private bool enableContentOffsetUpdateFromScale;
    private bool disableScrollOffsetSync;
    private CurrentZoomType currentZoomType;

    /// <summary>
    /// Gets or sets a value indicating whether zoom animations are enabled.
    /// </summary>
    [DependencyProperty(DefaultValue = true, Flags = FrameworkPropertyMetadataOptions.None)]
    private bool useAnimations;

    [DependencyProperty(DefaultValue = 0.4)]
    private double animationDuration;

    /// <summary>
    /// Gets or sets the delay in milliseconds before a scroll-zoom gesture
    /// commits to the undo history. Default is 750.
    /// </summary>
    [DependencyProperty(DefaultValue = 750)]
    private int zoomSaveDelayShortMs;

    /// <summary>
    /// Gets or sets the delay in milliseconds before a zoom-in/out command
    /// commits to the undo history. Default is 1500.
    /// </summary>
    [DependencyProperty(DefaultValue = 1500)]
    private int zoomSaveDelayLongMs;

    [DependencyProperty(DefaultValue = ZoomInitialPositionType.Default)]
    private ZoomInitialPositionType zoomInitialPosition;

    /// <summary>
    /// Gets or sets the X offset (in content coordinates) of the view on the content.
    /// </summary>
    [DependencyProperty(
        DefaultValue = 0.0,
        Flags = FrameworkPropertyMetadataOptions.None,
        PropertyChangedCallback = nameof(OnContentOffsetXChanged),
        CoerceValueCallback = nameof(CoerceContentOffsetX))]
    private double contentOffsetX;

    /// <summary>
    /// Gets or sets the Y offset (in content coordinates) of the view on the content.
    /// </summary>
    [DependencyProperty(
        DefaultValue = 0.0,
        Flags = FrameworkPropertyMetadataOptions.None,
        PropertyChangedCallback = nameof(OnContentOffsetYChanged),
        CoerceValueCallback = nameof(CoerceContentOffsetY))]
    private double contentOffsetY;

    [DependencyProperty]
    private double contentViewportHeight;

    [DependencyProperty]
    private double contentViewportWidth;

    [DependencyProperty]
    private double contentZoomFocusX;

    [DependencyProperty]
    private double contentZoomFocusY;

    /// <summary>
    /// Gets or sets a value indicating whether mouse wheel scrolling is enabled.
    /// </summary>
    [DependencyProperty(DefaultValue = false, Flags = FrameworkPropertyMetadataOptions.None)]
    private bool isMouseWheelScrollingEnabled;

    /// <summary>
    /// Gets or sets the maximum value for 'ViewportZoom'.
    /// </summary>
    [DependencyProperty(
        DefaultValue = 10.0,
        Flags = FrameworkPropertyMetadataOptions.None,
        PropertyChangedCallback = nameof(OnMinimumOrMaximumZoomChanged),
        CoerceValueCallback = nameof(CoerceMaximumZoom))]
    private double maximumZoom;

    [DependencyProperty(DefaultValue = ZoomMinimumType.MinimumZoom)]
    private ZoomMinimumType minimumZoomType;

    /// <summary>
    /// Gets or sets the minimum value for 'ViewportZoom'.
    /// </summary>
    [DependencyProperty(
        DefaultValue = 0.1,
        Flags = FrameworkPropertyMetadataOptions.None,
        PropertyChangedCallback = nameof(OnMinimumOrMaximumZoomChanged))]
    private double minimumZoom;

    [DependencyProperty]
    private Point? mousePosition;

    /// <summary>
    /// Gets or sets the current zoom level.
    /// </summary>
    [DependencyProperty(
        DefaultValue = 1.0,
        Flags = FrameworkPropertyMetadataOptions.None,
        PropertyChangedCallback = nameof(OnViewportZoomChanged))]
    private double viewportZoom;

    [DependencyProperty]
    private double viewportZoomFocusX;

    [DependencyProperty]
    private double viewportZoomFocusY;

    [DependencyProperty(DefaultValue = 10.0)]
    private double dragZoomThreshold;

    /// <summary>
    /// Gets or sets a value indicating whether touch gestures (pinch-to-zoom, two-finger pan) are enabled.
    /// </summary>
    [DependencyProperty(DefaultValue = true, Flags = FrameworkPropertyMetadataOptions.None)]
    private bool isTouchEnabled;

    /// <summary>
    /// Gets or sets a value indicating whether holding the spacebar enables temporary pan mode.
    /// When enabled, holding Space and dragging pans the content with a hand cursor.
    /// </summary>
    [DependencyProperty(DefaultValue = true, Flags = FrameworkPropertyMetadataOptions.None)]
    private bool isSpacebarPanEnabled;

    /// <summary>
    /// Gets or sets a value indicating whether Shift+MouseWheel pans horizontally.
    /// </summary>
    [DependencyProperty(DefaultValue = true, Flags = FrameworkPropertyMetadataOptions.None)]
    private bool isShiftScrollHorizontalPanEnabled;

    /// <summary>
    /// Gets or sets the list of preset zoom levels used by
    /// <see cref="ZoomToNextPresetCommand"/> and <see cref="ZoomToPreviousPresetCommand"/>.
    /// When <see langword="null"/>, a default set is used (10% to 1000%).
    /// Values represent zoom ratios where 1.0 = 100%.
    /// </summary>
    [DependencyProperty(Flags = FrameworkPropertyMetadataOptions.None)]
    private IList<double>? zoomPresets;

    /// <summary>
    /// Gets or sets how the viewport is constrained relative to the content bounds.
    /// </summary>
    [DependencyProperty(DefaultValue = ZoomConstraintMode.Inside, Flags = FrameworkPropertyMetadataOptions.None)]
    private ZoomConstraintMode constraintMode;

    /// <summary>
    /// Gets or sets custom keyboard bindings for zoom actions.
    /// When <see langword="null"/>, the default bindings are used.
    /// </summary>
    [DependencyProperty(Flags = FrameworkPropertyMetadataOptions.None)]
    private IList<ZoomKeyBinding>? zoomKeyBindings;

    [DependencyProperty(
        DefaultValue = 1.0,
        Flags = FrameworkPropertyMetadataOptions.None,
        PropertyChangedCallback = nameof(OnInternalViewportZoomChanged),
        CoerceValueCallback = nameof(CoerceInternalViewportZoom),
        AccessModifier = PropertyAccessModifier.Private)]
    private double internalViewportZoom;

    private static readonly double[] DefaultZoomPresets = [0.1, 0.25, 0.5, 0.75, 1.0, 1.5, 2.0, 3.0, 4.0, 5.0, 10.0];

    private static readonly IList<ZoomKeyBinding> DefaultZoomKeyBindings
        = new ReadOnlyCollection<ZoomKeyBinding>(ZoomKeyBinding.CreateDefaults());

    internal IList<double> EffectiveZoomPresets
        => ZoomPresets ?? DefaultZoomPresets;

    internal IList<ZoomKeyBinding> EffectiveZoomKeyBindings
        => ZoomKeyBindings ?? DefaultZoomKeyBindings;

    static ZoomBox()
    {
        OverrideDefaultStyleKey();
        StyleCheck.Register(typeof(ZoomBox));
    }

    /// <summary>Initializes a new instance of the <see cref="ZoomBox"/> class.</summary>
    public ZoomBox()
    {
        Focusable = true;
        IsManipulationEnabled = true;
        Loaded += OnZoomBoxLoaded;
        Unloaded += OnZoomBoxUnloaded;
    }

    private void OnZoomBoxLoaded(
        object sender,
        RoutedEventArgs e)
        => Messenger.Default.Register<ZoomCommandMessage>(this, OnZoomCommandMessageHandler);

    private void OnZoomBoxUnloaded(
        object sender,
        RoutedEventArgs e)
        => Messenger.Default.UnRegister<ZoomCommandMessage>(this, OnZoomCommandMessageHandler);

    /// <summary>Gets the zoom level at which the content fits inside the viewport.</summary>
    public double FitZoomValue => ViewportHelpers.FitZoom(
        ActualWidth,
        ActualHeight,
        content?.ActualWidth,
        content?.ActualHeight);

    /// <summary>Gets the zoom level at which the content fills the viewport.</summary>
    public double FillZoomValue => ViewportHelpers.FillZoom(
        ActualWidth,
        ActualHeight,
        content?.ActualWidth,
        content?.ActualHeight);

    /// <summary>Gets the effective minimum zoom level, determined by <see cref="MinimumZoomType"/>.</summary>
    public double MinimumZoomClamped
        => (MinimumZoomType switch
        {
            ZoomMinimumType.FillScreen => FillZoomValue,
            ZoomMinimumType.FitScreen => FitZoomValue,
            _ => MinimumZoom,
        }).ToRealNumber();

    /// <summary>
    /// Event raised when the ContentOffsetX property has changed.
    /// </summary>
    public event EventHandler? ContentOffsetXChanged;

    /// <summary>
    /// Event raised when the ContentOffsetY property has changed.
    /// </summary>
    public event EventHandler? ContentOffsetYChanged;

    /// <summary>
    /// Event raised when the ViewportZoom property has changed.
    /// </summary>
    public event EventHandler? ContentZoomChanged;

    /// <summary>
    /// Raised when the zoom level changes, providing old/new zoom values
    /// and a normalized <see cref="ZoomLevelChangedEventArgs.LevelOfDetail"/>.
    /// </summary>
    public event EventHandler<ZoomLevelChangedEventArgs>? ZoomLevelChanged;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// This allows the same property name be used for direct and indirect access to this control.
    /// </summary>
    public ZoomBox ZoomContent => this;

    /// <inheritdoc />
    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        ArgumentNullException.ThrowIfNull(sizeInfo);

        base.OnRenderSizeChanged(sizeInfo);

        if (sizeInfo.NewSize.Width <= 1 ||
            sizeInfo.NewSize.Height <= 1)
        {
            return;
        }

        InternalViewportZoom = currentZoomType switch
        {
            CurrentZoomType.Fit => ViewportHelpers.FitZoom(
                sizeInfo.NewSize.Width,
                sizeInfo.NewSize.Height,
                content?.ActualWidth,
                content?.ActualHeight),
            CurrentZoomType.Fill => ViewportHelpers.FillZoom(
                sizeInfo.NewSize.Width,
                sizeInfo.NewSize.Height,
                content?.ActualWidth,
                content?.ActualHeight),
            _ => InternalViewportZoom,
        };

        if (InternalViewportZoom < MinimumZoomClamped)
        {
            InternalViewportZoom = MinimumZoomClamped;
        }

        OnPropertyChanged(nameof(MinimumZoomClamped));
        OnPropertyChanged(nameof(FillZoomValue));
        OnPropertyChanged(nameof(FitZoomValue));
    }

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        content = Template.FindName("PART_Content", this) as FrameworkElement;
        if (content is null)
        {
            return;
        }

        contentZoomTransform = new ScaleTransform(InternalViewportZoom, InternalViewportZoom);
        contentOffsetTransform = new TranslateTransform();

        UpdateTranslationX();
        UpdateTranslationY();

        var transformGroup = new TransformGroup();
        transformGroup.Children.Add(contentOffsetTransform);
        transformGroup.Children.Add(contentZoomTransform);
        content.RenderTransform = transformGroup;

        partDragZoomBorder = Template.FindName("PART_DragZoomBorder", this) as Border;
        partDragZoomCanvas = Template.FindName("PART_DragZoomCanvas", this) as Canvas;
    }

    /// <inheritdoc />
    protected override Size MeasureOverride(Size constraint)
    {
        var infiniteSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
        var childSize = base.MeasureOverride(infiniteSize);

        if (childSize != unScaledExtent)
        {
            unScaledExtent = childSize;
            ScrollOwner?.InvalidateScrollInfo();
        }

        UpdateViewportSize(constraint);

        var width = constraint.Width;
        var height = constraint.Height;
        if (double.IsInfinity(width))
        {
            width = childSize.Width;
        }

        if (double.IsInfinity(height))
        {
            height = childSize.Height;
        }

        UpdateTranslationX();
        UpdateTranslationY();

        return new Size(width, height);
    }

    /// <inheritdoc />
    protected override Size ArrangeOverride(Size arrangeBounds)
    {
        var size = base.ArrangeOverride(DesiredSize);

        if (content is not null &&
            content.DesiredSize != unScaledExtent)
        {
            unScaledExtent = content.DesiredSize;
            ScrollOwner?.InvalidateScrollInfo();
        }

        UpdateViewportSize(arrangeBounds);

        return size;
    }

    /// <summary>Raises the <see cref="PropertyChanged"/> event for the specified property.</summary>
    protected virtual void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));

    private Size unScaledExtent = new(0, 0);

    private Size viewport = new(0, 0);

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK.")]
    private static void OnInternalViewportZoomChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs e)
    {
        var c = (ZoomBox)dependencyObject;

        if (c.contentZoomTransform != null)
        {
            c.contentZoomTransform.ScaleX = c.InternalViewportZoom;
            c.contentZoomTransform.ScaleY = c.InternalViewportZoom;
        }

        c.UpdateContentViewportSize();

        if (c.enableContentOffsetUpdateFromScale)
        {
            try
            {
                c.disableContentFocusSync = true;

                var viewportOffsetX = c.ViewportZoomFocusX - (c.ViewportWidth / 2);
                var viewportOffsetY = c.ViewportZoomFocusY - (c.ViewportHeight / 2);
                var focusOffsetX = viewportOffsetX / c.InternalViewportZoom;
                var focusOffsetY = viewportOffsetY / c.InternalViewportZoom;
                c.ContentOffsetX = c.ContentZoomFocusX - (c.ContentViewportWidth / 2) - focusOffsetX;
                c.ContentOffsetY = c.ContentZoomFocusY - (c.ContentViewportHeight / 2) - focusOffsetY;
            }
            finally
            {
                c.disableContentFocusSync = false;
            }
        }

        c.ContentZoomChanged?.Invoke(c, EventArgs.Empty);

        if (c.ZoomLevelChanged is not null)
        {
            var oldZoom = (double)e.OldValue;
            var newZoom = (double)e.NewValue;
            c.ZoomLevelChanged.Invoke(
                c,
                new ZoomLevelChangedEventArgs(oldZoom, newZoom, c.MinimumZoomClamped, c.MaximumZoom));
        }

        c.ViewportZoom = c.InternalViewportZoom;
        c.OnPropertyChanged(
            new DependencyPropertyChangedEventArgs(
                ViewportZoomProperty,
                c.ViewportZoom,
                c.InternalViewportZoom));
        c.ScrollOwner?.InvalidateScrollInfo();
        c.SetCurrentZoomType();
        c.RaiseCanExecuteChanged();
    }

    private static object CoerceInternalViewportZoom(
        DependencyObject dependencyObject,
        object baseValue)
    {
        var c = (ZoomBox)dependencyObject;
        var value = System.Math.Max((double)baseValue, c.MinimumZoomClamped);
        value = c.MinimumZoomType switch
        {
            ZoomMinimumType.FitScreen => System.Math.Min(System.Math.Max(value, c.FitZoomValue), c.MaximumZoom),
            ZoomMinimumType.FillScreen => System.Math.Min(System.Math.Max(value, c.FillZoomValue), c.MaximumZoom),
            ZoomMinimumType.MinimumZoom => System.Math.Min(System.Math.Max(value, c.MinimumZoom), c.MaximumZoom),
            _ => throw new SwitchCaseDefaultException(c.MinimumZoomType),
        };

        return value;
    }

    private static object CoerceMaximumZoom(
        DependencyObject d,
        object baseValue)
    {
        var c = (ZoomBox)d;
        var value = (double)baseValue;
        return System.Math.Max(value, c.MinimumZoom);
    }

    private static void OnMinimumOrMaximumZoomChanged(
        DependencyObject o,
        DependencyPropertyChangedEventArgs e)
    {
        var c = (ZoomBox)o;

        // Re-coerce MaximumZoom when MinimumZoom changes to keep them consistent
        c.CoerceValue(MaximumZoomProperty);

        c.InternalViewportZoom = System.Math.Min(System.Math.Max(c.InternalViewportZoom, c.MinimumZoomClamped), c.MaximumZoom);
    }

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK.")]
    private static void OnContentOffsetXChanged(
        DependencyObject o,
        DependencyPropertyChangedEventArgs e)
    {
        var c = (ZoomBox)o;

        c.UpdateTranslationX();

        if (!c.disableContentFocusSync)
        {
            c.UpdateContentZoomFocusX();
        }

        if (!c.disableScrollOffsetSync)
        {
            c.ScrollOwner?.InvalidateScrollInfo();
        }

        c.ContentOffsetXChanged?.Invoke(c, EventArgs.Empty);
    }

    private static object CoerceContentOffsetX(
        DependencyObject d,
        object baseValue)
    {
        var c = (ZoomBox)d;
        var value = (double)baseValue;

        if (c.ConstraintMode != ZoomConstraintMode.Free)
        {
            var minOffsetX = 0.0;
            var maxOffsetX = System.Math.Max(0.0, c.unScaledExtent.Width - c.constrainedContentViewportWidth);
            value = System.Math.Min(System.Math.Max(value, minOffsetX), maxOffsetX);
        }

        return value;
    }

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK.")]
    private static void OnContentOffsetYChanged(
        DependencyObject o,
        DependencyPropertyChangedEventArgs e)
    {
        var c = (ZoomBox)o;

        c.UpdateTranslationY();

        if (!c.disableContentFocusSync)
        {
            c.UpdateContentZoomFocusY();
        }

        if (!c.disableScrollOffsetSync)
        {
            c.ScrollOwner?.InvalidateScrollInfo();
        }

        c.ContentOffsetYChanged?.Invoke(c, EventArgs.Empty);
    }

    private static object CoerceContentOffsetY(
        DependencyObject d,
        object baseValue)
    {
        var c = (ZoomBox)d;
        var value = (double)baseValue;

        if (c.ConstraintMode != ZoomConstraintMode.Free)
        {
            var minOffsetY = 0.0;
            var maxOffsetY = System.Math.Max(0.0, c.unScaledExtent.Height - c.constrainedContentViewportHeight);
            value = System.Math.Min(System.Math.Max(value, minOffsetY), maxOffsetY);
        }

        return value;
    }

    private static void OnViewportZoomChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs e)
    {
        var c = (ZoomBox)dependencyObject;
        var newZoom = (double)e.NewValue;

        Messenger.Default.Send(new ZoomInformationMessage(newZoom * 100));

        if (c.InternalViewportZoom.IsEqual(newZoom))
        {
            return;
        }

        var centerPoint = new Point(
            c.ContentOffsetX + (c.constrainedContentViewportWidth / 2),
            c.ContentOffsetY + (c.constrainedContentViewportHeight / 2));

        c.ZoomAboutPoint(newZoom, centerPoint);
    }

    private void ResetViewportZoomFocus()
    {
        ViewportZoomFocusX = ViewportWidth / 2;
        ViewportZoomFocusY = ViewportHeight / 2;
    }

    private void UpdateViewportSize(Size newSize)
    {
        if (viewport == newSize)
        {
            return;
        }

        viewport = newSize;

        UpdateContentViewportSize();

        UpdateContentZoomFocusX();
        UpdateContentZoomFocusY();

        ResetViewportZoomFocus();

        var x = ContentOffsetX;
        ContentOffsetX = x;
        var y = ContentOffsetY;
        ContentOffsetY = y;

        ScrollOwner?.InvalidateScrollInfo();
    }

    private void UpdateContentViewportSize()
    {
        ContentViewportWidth = ViewportWidth / InternalViewportZoom;
        ContentViewportHeight = ViewportHeight / InternalViewportZoom;

        constrainedContentViewportWidth = System.Math.Min(ContentViewportWidth, unScaledExtent.Width);
        constrainedContentViewportHeight = System.Math.Min(ContentViewportHeight, unScaledExtent.Height);

        UpdateTranslationX();
        UpdateTranslationY();
    }

    private void UpdateTranslationX()
    {
        if (contentOffsetTransform is null)
        {
            return;
        }

        var scaledContentWidth = unScaledExtent.Width * InternalViewportZoom;
        if (scaledContentWidth < ViewportWidth)
        {
            contentOffsetTransform.X = (ContentViewportWidth - unScaledExtent.Width) / 2;
        }
        else
        {
            contentOffsetTransform.X = -ContentOffsetX;
        }
    }

    private void UpdateTranslationY()
    {
        if (contentOffsetTransform is null)
        {
            return;
        }

        var scaledContentHeight = unScaledExtent.Height * InternalViewportZoom;
        if (scaledContentHeight < ViewportHeight)
        {
            contentOffsetTransform.Y = (ContentViewportHeight - unScaledExtent.Height) / 2;
        }
        else
        {
            contentOffsetTransform.Y = -ContentOffsetY;
        }
    }

    private void UpdateContentZoomFocusX()
        => ContentZoomFocusX = ContentOffsetX + (constrainedContentViewportWidth / 2);

    private void UpdateContentZoomFocusY()
        => ContentZoomFocusY = ContentOffsetY + (constrainedContentViewportHeight / 2);

    private void SetCurrentZoomType()
    {
        if (ViewportZoom.IsWithinOnePercent(FitZoomValue))
        {
            currentZoomType = CurrentZoomType.Fit;
        }
        else if (ViewportZoom.IsWithinOnePercent(FillZoomValue))
        {
            currentZoomType = CurrentZoomType.Fill;
        }
        else
        {
            currentZoomType = CurrentZoomType.Other;
        }
    }
}