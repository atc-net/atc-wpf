namespace Atc.Wpf.Controls.Zoom;

/// <summary>
/// IScrollInfo implementation for ZoomBox, allowing integration with ScrollViewer.
/// </summary>
[SuppressMessage("Design", "MA0048:File name must match type name", Justification = "OK - partial class")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1601:Partial elements should be documented", Justification = "OK - partial class")]
public partial class ZoomBox
{
    /// <inheritdoc />
    public bool CanVerticallyScroll { get; set; }

    /// <inheritdoc />
    public bool CanHorizontallyScroll { get; set; }

    /// <inheritdoc />
    public double ExtentWidth => unScaledExtent.Width * InternalViewportZoom;

    /// <inheritdoc />
    public double ExtentHeight => unScaledExtent.Height * InternalViewportZoom;

    /// <inheritdoc />
    public double ViewportWidth => viewport.Width;

    /// <inheritdoc />
    public double ViewportHeight => viewport.Height;

    /// <inheritdoc />
    public ScrollViewer? ScrollOwner { get; set; }

    /// <inheritdoc />
    public double HorizontalOffset => ContentOffsetX * InternalViewportZoom;

    /// <inheritdoc />
    public double VerticalOffset => ContentOffsetY * InternalViewportZoom;

    /// <inheritdoc />
    public void SetHorizontalOffset(double offset)
    {
        if (disableScrollOffsetSync)
        {
            return;
        }

        try
        {
            disableScrollOffsetSync = true;
            ContentOffsetX = offset / InternalViewportZoom;
            DelayedSaveZoom750MilliSeconds();
        }
        finally
        {
            disableScrollOffsetSync = false;
        }
    }

    /// <inheritdoc />
    public void SetVerticalOffset(double offset)
    {
        if (disableScrollOffsetSync)
        {
            return;
        }

        try
        {
            disableScrollOffsetSync = true;
            ContentOffsetY = offset / InternalViewportZoom;
            DelayedSaveZoom750MilliSeconds();
        }
        finally
        {
            disableScrollOffsetSync = false;
        }
    }

    /// <inheritdoc />
    public void LineUp()
    {
        DelayedSaveZoom750MilliSeconds();
        ContentOffsetY -= ContentViewportHeight / 10;
    }

    /// <inheritdoc />
    public void LineDown()
    {
        DelayedSaveZoom750MilliSeconds();
        ContentOffsetY += ContentViewportHeight / 10;
    }

    /// <inheritdoc />
    public void LineLeft()
    {
        DelayedSaveZoom750MilliSeconds();
        ContentOffsetX -= ContentViewportWidth / 10;
    }

    /// <inheritdoc />
    public void LineRight()
    {
        DelayedSaveZoom750MilliSeconds();
        ContentOffsetX += ContentViewportWidth / 10;
    }

    /// <inheritdoc />
    public void PageUp()
    {
        DelayedSaveZoom1500MilliSeconds();
        ContentOffsetY -= ContentViewportHeight;
    }

    /// <inheritdoc />
    public void PageDown()
    {
        DelayedSaveZoom1500MilliSeconds();
        ContentOffsetY += ContentViewportHeight;
    }

    /// <inheritdoc />
    public void PageLeft()
    {
        DelayedSaveZoom1500MilliSeconds();
        ContentOffsetX -= ContentViewportWidth;
    }

    /// <inheritdoc />
    public void PageRight()
    {
        DelayedSaveZoom1500MilliSeconds();
        ContentOffsetX += ContentViewportWidth;
    }

    /// <inheritdoc />
    public void MouseWheelDown()
    {
        if (IsMouseWheelScrollingEnabled)
        {
            LineDown();
        }
    }

    /// <inheritdoc />
    public void MouseWheelLeft()
    {
        if (IsMouseWheelScrollingEnabled)
        {
            LineLeft();
        }
    }

    /// <inheritdoc />
    public void MouseWheelRight()
    {
        if (IsMouseWheelScrollingEnabled)
        {
            LineRight();
        }
    }

    /// <inheritdoc />
    public void MouseWheelUp()
    {
        if (IsMouseWheelScrollingEnabled)
        {
            LineUp();
        }
    }

    /// <inheritdoc />
    public Rect MakeVisible(
        Visual visual,
        Rect rectangle)
    {
        ArgumentNullException.ThrowIfNull(visual);

        if (content is null ||
            !content.IsAncestorOf(visual))
        {
            return rectangle;
        }

        var transformedRect = visual
            .TransformToAncestor(content)
            .TransformBounds(rectangle);

        var viewportRect = new Rect(
            ContentOffsetX,
            ContentOffsetY,
            ContentViewportWidth,
            ContentViewportHeight);

        if (transformedRect.Contains(viewportRect))
        {
            return rectangle;
        }

        var horizontalOffset = 0d;
        var verticalOffset = 0d;

        if (transformedRect.Left < viewportRect.Left)
        {
            horizontalOffset = transformedRect.Left - viewportRect.Left;
        }
        else if (transformedRect.Right > viewportRect.Right)
        {
            horizontalOffset = transformedRect.Right - viewportRect.Right;
        }

        if (transformedRect.Top < viewportRect.Top)
        {
            verticalOffset = transformedRect.Top - viewportRect.Top;
        }
        else if (transformedRect.Bottom > viewportRect.Bottom)
        {
            verticalOffset = transformedRect.Bottom - viewportRect.Bottom;
        }

        SnapContentOffsetTo(
            new Point(
                ContentOffsetX + horizontalOffset,
                ContentOffsetY + verticalOffset));

        return rectangle;
    }
}