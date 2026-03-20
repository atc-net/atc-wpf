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
    private MouseHandlingModeType mouseHandlingMode;
    private Point contentMouseDownPoint;

    [DependencyProperty(PropertyChangedCallback = nameof(OnVisualElementChanged))]
    private FrameworkElement? visualElement;

    static ZoomMiniMap()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(ZoomMiniMap),
            new FrameworkPropertyMetadata(typeof(ZoomMiniMap)));
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        dragBorder = Template.FindName("PART_DraggingBorder", this) as Border;
        sizingBorder = Template.FindName("PART_SizingBorder", this) as Border;
        viewportCanvas = Template.FindName("PART_Content", this) as Canvas;
        SetBackground(VisualElement);
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);

        if (viewportCanvas is null ||
            sizingBorder is null ||
            dragBorder is null)
        {
            return;
        }

        if (ActualWidth > 0)
        {
            sizingBorder.BorderThickness = dragBorder.BorderThickness = new Thickness(
                viewportCanvas.ActualWidth / ActualWidth * BorderThickness.Left,
                viewportCanvas.ActualWidth / ActualWidth * BorderThickness.Top,
                viewportCanvas.ActualWidth / ActualWidth * BorderThickness.Right,
                viewportCanvas.ActualWidth / ActualWidth * BorderThickness.Bottom);
        }
    }

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

    private void SetBackground(FrameworkElement? frameworkElement)
    {
        frameworkElement ??= (DataContext as ContentControl)?.Content as FrameworkElement;
        if (frameworkElement is null)
        {
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

    private ZoomBox GetZoomBox()
    {
        var zoomBox = DataContext as ZoomBox ??
                      (DataContext as ZoomScrollViewer)?.ZoomContent;

        return zoomBox ?? throw new InvalidOperationException("DataContext is not of type ZoomBox");
    }
}