// ReSharper disable InvertIf

namespace Atc.Wpf.Controls.Zoom;

/// <summary>
/// A minimap control that shows an overview of the ZoomBox content with a draggable viewport indicator.
/// </summary>
public partial class ZoomMiniMap : ContentControl
{
    private Border? dragBorder;
    private Border? sizingBorder;
    private Canvas? viewportCanvas;
    private FrameworkElement? deferredVisualElement;
    private MouseHandlingModeType mouseHandlingMode;
    private Point contentMouseDownPoint;

    [DependencyProperty(PropertyChangedCallback = nameof(OnVisualElementChanged))]
    private FrameworkElement? visualElement;

    /// <summary>
    /// Gets or sets the brush used for the viewport indicator border.
    /// When <see langword="null"/>, the default theme brush is used.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnViewportBorderBrushChanged))]
    private Brush? viewportBorderBrush;

    /// <summary>
    /// Gets or sets the on-screen thickness of the viewport indicator border.
    /// When <see langword="null"/>, <see cref="Control.BorderThickness"/> is used.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnViewportBorderThicknessChanged))]
    private double? viewportBorderThickness;

    static ZoomMiniMap()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(ZoomMiniMap),
            new FrameworkPropertyMetadata(typeof(ZoomMiniMap)));

        DataContextProperty.OverrideMetadata(
            typeof(ZoomMiniMap),
            new FrameworkPropertyMetadata(OnDataContextChangedCallback));
    }

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        dragBorder = Template.FindName("PART_DraggingBorder", this) as Border;
        sizingBorder = Template.FindName("PART_SizingBorder", this) as Border;
        viewportCanvas = Template.FindName("PART_Content", this) as Canvas;
        ApplyViewportBorderBrush(dragBorder);
        ApplyViewportBorderBrush(sizingBorder);
        UpdateViewportBorderThickness();
        SetBackground(VisualElement);
    }

    /// <inheritdoc />
    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        UpdateViewportBorderThickness();
    }

    /// <inheritdoc />
    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        OnMouseLeftButtonDown(e);

        if (dragBorder is null ||
            sizingBorder is null ||
            viewportCanvas is null)
        {
            return;
        }

        GetZoomBox().SaveZoom();
        mouseHandlingMode = MouseHandlingModeType.Panning;
        contentMouseDownPoint = e.GetPosition(viewportCanvas);

        if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None)
        {
            mouseHandlingMode = MouseHandlingModeType.DragZooming;
            dragBorder.Visibility = Visibility.Hidden;
            sizingBorder.Visibility = Visibility.Visible;
            Canvas.SetLeft(sizingBorder, contentMouseDownPoint.X);
            Canvas.SetTop(sizingBorder, contentMouseDownPoint.Y);
            sizingBorder.Width = 0;
            sizingBorder.Height = 0;
        }
        else
        {
            mouseHandlingMode = MouseHandlingModeType.Panning;
        }

        viewportCanvas.CaptureMouse();
        e.Handled = true;
    }

    /// <inheritdoc />
    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        OnMouseLeftButtonUp(e);

        if (viewportCanvas is null ||
            sizingBorder is null ||
            dragBorder is null)
        {
            return;
        }

        if (mouseHandlingMode == MouseHandlingModeType.DragZooming)
        {
            var zoomBox = GetZoomBox();
            var curContentPoint = e.GetPosition(viewportCanvas);
            var rect = ViewportHelpers.Clip(
                curContentPoint,
                contentMouseDownPoint,
                new Point(0, 0),
                new Point(viewportCanvas.Width, viewportCanvas.Height));
            zoomBox.AnimatedZoomTo(rect);
            dragBorder.Visibility = Visibility.Visible;
            sizingBorder.Visibility = Visibility.Hidden;
        }

        mouseHandlingMode = MouseHandlingModeType.None;
        viewportCanvas.ReleaseMouseCapture();
        e.Handled = true;
    }

    /// <inheritdoc />
    protected override void OnMouseMove(MouseEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnMouseMove(e);

        if (viewportCanvas is null ||
            sizingBorder is null ||
            dragBorder is null)
        {
            return;
        }

        if (mouseHandlingMode == MouseHandlingModeType.Panning)
        {
            var curContentPoint = e.GetPosition(viewportCanvas);
            var rectangleDragVector = curContentPoint - contentMouseDownPoint;

            contentMouseDownPoint = e.GetPosition(viewportCanvas).Clamp();
            Canvas.SetLeft(dragBorder, Canvas.GetLeft(dragBorder) + rectangleDragVector.X);
            Canvas.SetTop(dragBorder, Canvas.GetTop(dragBorder) + rectangleDragVector.Y);
        }
        else if (mouseHandlingMode == MouseHandlingModeType.DragZooming)
        {
            var curContentPoint = e.GetPosition(viewportCanvas);
            var rect = ViewportHelpers.Clip(curContentPoint, contentMouseDownPoint, new Point(0, 0), new Point(viewportCanvas.Width, viewportCanvas.Height));
            ViewportHelpers.PositionBorderOnCanvas(sizingBorder, rect);
        }

        e.Handled = true;
    }

    /// <inheritdoc />
    protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnMouseDoubleClick(e);

        if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None)
        {
            return;
        }

        var zoomBox = GetZoomBox();
        zoomBox.SaveZoom();
        zoomBox.AnimatedSnapTo(e.GetPosition(viewportCanvas));
    }

    private static void OnVisualElementChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var c = (ZoomMiniMap)d;
        c.SetBackground(e.NewValue as FrameworkElement);
    }

    private static void OnViewportBorderBrushChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var c = (ZoomMiniMap)d;
        c.ApplyViewportBorderBrush(c.dragBorder);
        c.ApplyViewportBorderBrush(c.sizingBorder);
    }

    private static void OnViewportBorderThicknessChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((ZoomMiniMap)d).UpdateViewportBorderThickness();

    /// <summary>
    /// Overrides the template's border brush; clearing the local value restores the template brush.
    /// </summary>
    private void ApplyViewportBorderBrush(Border? border)
    {
        if (border is null)
        {
            return;
        }

        if (ViewportBorderBrush is null)
        {
            border.ClearValue(Border.BorderBrushProperty);
        }
        else
        {
            border.BorderBrush = ViewportBorderBrush;
        }
    }

    /// <summary>
    /// The viewport borders live in content coordinates inside a Viewbox, so the on-screen
    /// thickness (ViewportBorderThickness, or BorderThickness when not set) is scaled to content units.
    /// </summary>
    private void UpdateViewportBorderThickness()
    {
        if (viewportCanvas is null ||
            sizingBorder is null ||
            dragBorder is null ||
            ActualWidth <= 0)
        {
            return;
        }

        var thickness = ViewportBorderThickness is { } uniform
            ? new Thickness(uniform)
            : BorderThickness;
        var scale = viewportCanvas.ActualWidth / ActualWidth;

        sizingBorder.BorderThickness = dragBorder.BorderThickness = new Thickness(
            scale * thickness.Left,
            scale * thickness.Top,
            scale * thickness.Right,
            scale * thickness.Bottom);
    }

    private void SetBackground(FrameworkElement? frameworkElement)
    {
        frameworkElement ??= (DataContext as ContentControl)?.Content as FrameworkElement;
        if (frameworkElement is null)
        {
            return;
        }

        // A VisualBrush lays out a visual that has no parent yet on its own. For right-to-left content
        // that layout mirrors it against the missing (left-to-right) parent, and the mirror stays once
        // the content is attached, so wait until the content is in the visual tree.
        if (VisualTreeHelper.GetParent(frameworkElement) is null)
        {
            DeferSetBackgroundUntilLoaded(frameworkElement);
            return;
        }

        var visualBrush = new VisualBrush
        {
            Visual = frameworkElement,
            ViewboxUnits = BrushMappingMode.RelativeToBoundingBox,
            ViewportUnits = BrushMappingMode.RelativeToBoundingBox,
            TileMode = TileMode.None,
            Stretch = Stretch.Fill,
        };

        if (viewportCanvas is not null)
        {
            viewportCanvas.Height = frameworkElement.ActualHeight;
            viewportCanvas.Width = frameworkElement.ActualWidth;
            viewportCanvas.Background = visualBrush;
        }

        frameworkElement.SizeChanged += (_, _) =>
        {
            if (viewportCanvas is not null)
            {
                viewportCanvas.Height = frameworkElement.ActualHeight;
                viewportCanvas.Width = frameworkElement.ActualWidth;
                viewportCanvas.Background = visualBrush;
            }
        };
    }

    private void DeferSetBackgroundUntilLoaded(
        FrameworkElement frameworkElement)
    {
        if (ReferenceEquals(deferredVisualElement, frameworkElement))
        {
            return;
        }

        if (deferredVisualElement is not null)
        {
            deferredVisualElement.Loaded -= OnDeferredVisualElementLoaded;
        }

        deferredVisualElement = frameworkElement;
        frameworkElement.Loaded += OnDeferredVisualElementLoaded;
    }

    private void OnDeferredVisualElementLoaded(
        object sender,
        RoutedEventArgs e)
    {
        var frameworkElement = (FrameworkElement)sender;
        frameworkElement.Loaded -= OnDeferredVisualElementLoaded;
        deferredVisualElement = null;
        SetBackground(frameworkElement);
    }

    private static void OnDataContextChangedCallback(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var miniMap = (ZoomMiniMap)d;
        miniMap.SetBackground(miniMap.VisualElement);
    }

    private ZoomBox GetZoomBox()
    {
        var zoomBox = DataContext as ZoomBox ??
                      (DataContext as ZoomScrollViewer)?.ZoomContent;

        return zoomBox ?? throw new InvalidOperationException("DataContext is not of type ZoomBox");
    }
}