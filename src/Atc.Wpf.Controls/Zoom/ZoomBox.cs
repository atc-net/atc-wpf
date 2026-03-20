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

    public static readonly DependencyProperty UseAnimationsProperty = DependencyProperty.Register(
        nameof(UseAnimations),
        typeof(bool),
        typeof(ZoomBox),
        new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Gets or sets a value indicating whether zoom animations are enabled.
    /// </summary>
    public bool UseAnimations
    {
        get => (bool)GetValue(UseAnimationsProperty);
        set => SetValue(UseAnimationsProperty, BooleanBoxes.Box(value));
    }

    [DependencyProperty(DefaultValue = 0.4)]
    private double animationDuration;

    [DependencyProperty(DefaultValue = ZoomInitialPositionType.Default)]
    private ZoomInitialPositionType zoomInitialPosition;

    public static readonly DependencyProperty ContentOffsetXProperty = DependencyProperty.Register(
        nameof(ContentOffsetX),
        typeof(double),
        typeof(ZoomBox),
        new FrameworkPropertyMetadata(
            0.0,
            OnContentOffsetXChanged,
            CoerceContentOffsetX));

    /// <summary>
    /// Gets or sets the X offset (in content coordinates) of the view on the content.
    /// </summary>
    public double ContentOffsetX
    {
        get => (double)GetValue(ContentOffsetXProperty);
        set => SetValue(ContentOffsetXProperty, value);
    }

    public static readonly DependencyProperty ContentOffsetYProperty = DependencyProperty.Register(
        nameof(ContentOffsetY),
        typeof(double),
        typeof(ZoomBox),
        new FrameworkPropertyMetadata(
            0.0,
            OnContentOffsetYChanged,
            CoerceContentOffsetY));

    /// <summary>
    /// Gets or sets the Y offset (in content coordinates) of the view on the content.
    /// </summary>
    public double ContentOffsetY
    {
        get => (double)GetValue(ContentOffsetYProperty);
        set => SetValue(ContentOffsetYProperty, value);
    }

    [DependencyProperty]
    private double contentViewportHeight;

    [DependencyProperty]
    private double contentViewportWidth;

    [DependencyProperty]
    private double contentZoomFocusX;

    [DependencyProperty]
    private double contentZoomFocusY;

    public static readonly DependencyProperty IsMouseWheelScrollingEnabledProperty = DependencyProperty.Register(
        nameof(IsMouseWheelScrollingEnabled),
        typeof(bool),
        typeof(ZoomBox),
        new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Gets or sets a value indicating whether mouse wheel scrolling is enabled.
    /// </summary>
    public bool IsMouseWheelScrollingEnabled
    {
        get => (bool)GetValue(IsMouseWheelScrollingEnabledProperty);
        set => SetValue(IsMouseWheelScrollingEnabledProperty, BooleanBoxes.Box(value));
    }

    public static readonly DependencyProperty MaximumZoomProperty = DependencyProperty.Register(
        nameof(MaximumZoom),
        typeof(double),
        typeof(ZoomBox),
        new FrameworkPropertyMetadata(
            10.0,
            OnMinimumOrMaximumZoomChanged));

    /// <summary>
    /// Gets or sets the maximum value for 'ViewportZoom'.
    /// </summary>
    public double MaximumZoom
    {
        get => (double)GetValue(MaximumZoomProperty);
        set => SetValue(MaximumZoomProperty, value);
    }

    [DependencyProperty(DefaultValue = ZoomMinimumType.MinimumZoom)]
    private ZoomMinimumType minimumZoomType;

    public static readonly DependencyProperty MinimumZoomProperty = DependencyProperty.Register(
        nameof(MinimumZoom),
        typeof(double),
        typeof(ZoomBox),
        new FrameworkPropertyMetadata(
            0.1,
            OnMinimumOrMaximumZoomChanged));

    /// <summary>
    /// Gets or sets the minimum value for 'ViewportZoom'.
    /// </summary>
    public double MinimumZoom
    {
        get => (double)GetValue(MinimumZoomProperty);
        set => SetValue(MinimumZoomProperty, value);
    }

    [DependencyProperty]
    private Point? mousePosition;

    public static readonly DependencyProperty ViewportZoomProperty = DependencyProperty.Register(
        nameof(ViewportZoom),
        typeof(double),
        typeof(ZoomBox),
        new FrameworkPropertyMetadata(
            1.0,
            OnViewportZoomChanged));

    /// <summary>
    /// Gets or sets the current zoom level.
    /// </summary>
    public double ViewportZoom
    {
        get => (double)GetValue(ViewportZoomProperty);
        set => SetValue(ViewportZoomProperty, value);
    }

    [DependencyProperty]
    private double viewportZoomFocusX;

    [DependencyProperty]
    private double viewportZoomFocusY;

    [DependencyProperty(DefaultValue = 10.0)]
    private double dragZoomThreshold;

    public static readonly DependencyProperty IsTouchEnabledProperty = DependencyProperty.Register(
        nameof(IsTouchEnabled),
        typeof(bool),
        typeof(ZoomBox),
        new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Gets or sets a value indicating whether touch gestures (pinch-to-zoom, two-finger pan) are enabled.
    /// </summary>
    public bool IsTouchEnabled
    {
        get => (bool)GetValue(IsTouchEnabledProperty);
        set => SetValue(IsTouchEnabledProperty, BooleanBoxes.Box(value));
    }

    private static readonly DependencyProperty InternalViewportZoomProperty = DependencyProperty.Register(
        nameof(InternalViewportZoom),
        typeof(double),
        typeof(ZoomBox),
        new FrameworkPropertyMetadata(
            1.0,
            OnInternalViewportZoomChanged,
            CoerceInternalViewportZoom));

    private double InternalViewportZoom
    {
        get => (double)GetValue(InternalViewportZoomProperty);
        set => SetValue(InternalViewportZoomProperty, value);
    }

    static ZoomBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(ZoomBox),
            new FrameworkPropertyMetadata(typeof(ZoomBox)));
    }

    public ZoomBox()
    {
        Focusable = true;
        IsManipulationEnabled = true;
        Messenger.Default.Register<ZoomCommandMessage>(this, OnZoomCommandMessageHandler);
    }

    public double FitZoomValue => ViewportHelpers.FitZoom(
        ActualWidth,
        ActualHeight,
        content?.ActualWidth,
        content?.ActualHeight);

    public double FillZoomValue => ViewportHelpers.FillZoom(
        ActualWidth,
        ActualHeight,
        content?.ActualWidth,
        content?.ActualHeight);

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

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// This allows the same property name be used for direct and indirect access to this control.
    /// </summary>
    public ZoomBox ZoomContent => this;

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
                var contentOffsetX = viewportOffsetX / c.InternalViewportZoom;
                var contentOffsetY = viewportOffsetY / c.InternalViewportZoom;
                c.ContentOffsetX = c.ContentZoomFocusX - (c.ContentViewportWidth / 2) - contentOffsetX;
                c.ContentOffsetY = c.ContentZoomFocusY - (c.ContentViewportHeight / 2) - contentOffsetY;
            }
            finally
            {
                c.disableContentFocusSync = false;
            }
        }

        c.ContentZoomChanged?.Invoke(c, EventArgs.Empty);
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

    private static void OnMinimumOrMaximumZoomChanged(
        DependencyObject o,
        DependencyPropertyChangedEventArgs e)
    {
        var c = (ZoomBox)o;
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
        var minOffsetX = 0.0;
        var maxOffsetX = System.Math.Max(0.0, c.unScaledExtent.Width - c.constrainedContentViewportWidth);
        value = System.Math.Min(System.Math.Max(value, minOffsetX), maxOffsetX);
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
        var minOffsetY = 0.0;
        var maxOffsetY = System.Math.Max(0.0, c.unScaledExtent.Height - c.constrainedContentViewportHeight);
        value = System.Math.Min(System.Math.Max(value, minOffsetY), maxOffsetY);
        return value;
    }

    private static void OnViewportZoomChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs e)
    {
        var c = (ZoomBox)dependencyObject;
        var newZoom = (double)e.NewValue;

        Messenger.Default.Send(new ZoomInformationMessage(newZoom * 100));

        if (c.InternalViewportZoom.Equals(newZoom))
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