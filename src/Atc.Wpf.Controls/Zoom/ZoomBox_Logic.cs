namespace Atc.Wpf.Controls.Zoom;

[SuppressMessage("Design", "MA0048:File name must match type name", Justification = "OK - partial class")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1601:Partial elements should be documented", Justification = "OK - partial class")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:Element parameters should be documented", Justification = "OK.")]
public partial class ZoomBox
{
    /// <summary>
    /// Do an animated zoom to view a specific scale and rectangle (in content coordinates).
    /// </summary>
    public void AnimatedZoomTo(
        double newScale,
        Rect contentRect)
    {
        AnimatedZoomPointToViewportCenter(
            newScale,
            new Point(
                contentRect.X + (contentRect.Width / 2),
                contentRect.Y + (contentRect.Height / 2)),
            (_, _) =>
            {
                ContentOffsetX = contentRect.X;
                ContentOffsetY = contentRect.Y;
            });
    }

    /// <summary>
    /// Do an animated zoom to the specified rectangle (in content coordinates).
    /// </summary>
    public void AnimatedZoomTo(Rect contentRect)
    {
        var scaleX = ContentViewportWidth / contentRect.Width;
        var scaleY = ContentViewportHeight / contentRect.Height;
        var contentFitZoom = InternalViewportZoom * System.Math.Min(scaleX, scaleY);

        AnimatedZoomPointToViewportCenter(
            contentFitZoom,
            new Point(
                contentRect.X + (contentRect.Width / 2),
                contentRect.Y + (contentRect.Height / 2)),
            callback: null);
    }

    /// <summary>
    /// Zoom in/out centered on the viewport center.
    /// </summary>
    public void AnimatedZoomTo(double viewportZoom)
    {
        if (content is null)
        {
            return;
        }

        var xAdjust = (ContentViewportWidth - content.ActualWidth) * InternalViewportZoom / 2;
        var yAdjust = (ContentViewportHeight - content.ActualHeight) * InternalViewportZoom / 2;
        var zoomCenter = InternalViewportZoom >= FillZoomValue
            ? new Point(
                ContentOffsetX + (ContentViewportWidth / 2),
                ContentOffsetY + (ContentViewportHeight / 2))
            : new Point(
                (content.ActualWidth / 2) - xAdjust,
                (content.ActualHeight / 2) + yAdjust);

        AnimatedZoomAboutPoint(viewportZoom, zoomCenter);
    }

    /// <summary>
    /// Zoom in/out centered on the viewport center.
    /// </summary>
    public void AnimatedZoomToCentered(double viewportZoom)
    {
        if (content is null)
        {
            return;
        }

        var zoomCenter = new Point(
            content.ActualWidth / 2,
            content.ActualHeight / 2);

        AnimatedZoomAboutPoint(viewportZoom, zoomCenter);
    }

    /// <summary>
    /// Gets the current viewport state (zoom and scroll position) for serialization.
    /// </summary>
    public ViewportState CurrentViewportState
        => new(InternalViewportZoom, ContentOffsetX, ContentOffsetY);

    /// <summary>
    /// Restores a previously captured viewport state.
    /// </summary>
    /// <param name="state">The state to restore.</param>
    /// <param name="animate">Whether to animate the transition.</param>
    public void RestoreViewportState(
        ViewportState state,
        bool animate = true)
    {
        ArgumentNullException.ThrowIfNull(state);

        if (animate && UseAnimations)
        {
            AnimatedZoomAboutPoint(
                state.Zoom,
                new Point(
                    state.OffsetX + (ContentViewportWidth / 2),
                    state.OffsetY + (ContentViewportHeight / 2)));
        }
        else
        {
            InternalViewportZoom = state.Zoom;
            ContentOffsetX = state.OffsetX;
            ContentOffsetY = state.OffsetY;
        }
    }

    /// <summary>
    /// Scrolls the minimum distance to make the specified rectangle visible in the viewport
    /// without changing the zoom level. If the rectangle is already fully visible, no scrolling occurs.
    /// </summary>
    /// <param name="contentRect">The rectangle to make visible, in content coordinates.</param>
    /// <param name="marginX">Horizontal margin in content coordinates to keep around the rectangle.</param>
    /// <param name="marginY">Vertical margin in content coordinates to keep around the rectangle.</param>
    public void EnsureVisible(
        Rect contentRect,
        double marginX = 50,
        double marginY = 50)
    {
        var expandedRect = new Rect(
            contentRect.X - marginX,
            contentRect.Y - marginY,
            contentRect.Width + (2 * marginX),
            contentRect.Height + (2 * marginY));

        var newOffsetX = ContentOffsetX;
        var newOffsetY = ContentOffsetY;

        if (expandedRect.Left < ContentOffsetX)
        {
            newOffsetX = expandedRect.Left;
        }
        else if (expandedRect.Right > ContentOffsetX + ContentViewportWidth)
        {
            newOffsetX = expandedRect.Right - ContentViewportWidth;
        }

        if (expandedRect.Top < ContentOffsetY)
        {
            newOffsetY = expandedRect.Top;
        }
        else if (expandedRect.Bottom > ContentOffsetY + ContentViewportHeight)
        {
            newOffsetY = expandedRect.Bottom - ContentViewportHeight;
        }

        SnapContentOffsetTo(new Point(newOffsetX, newOffsetY));
    }

    /// <summary>
    /// Animated version of <see cref="EnsureVisible"/>. Scrolls the minimum distance
    /// to make the specified rectangle visible without changing the zoom level.
    /// </summary>
    public void AnimatedEnsureVisible(
        Rect contentRect,
        double marginX = 50,
        double marginY = 50)
    {
        var expandedRect = new Rect(
            contentRect.X - marginX,
            contentRect.Y - marginY,
            contentRect.Width + (2 * marginX),
            contentRect.Height + (2 * marginY));

        var newOffsetX = ContentOffsetX;
        var newOffsetY = ContentOffsetY;

        if (expandedRect.Left < ContentOffsetX)
        {
            newOffsetX = expandedRect.Left;
        }
        else if (expandedRect.Right > ContentOffsetX + ContentViewportWidth)
        {
            newOffsetX = expandedRect.Right - ContentViewportWidth;
        }

        if (expandedRect.Top < ContentOffsetY)
        {
            newOffsetY = expandedRect.Top;
        }
        else if (expandedRect.Bottom > ContentOffsetY + ContentViewportHeight)
        {
            newOffsetY = expandedRect.Bottom - ContentViewportHeight;
        }

        AnimationHelper.StartAnimation(this, ContentOffsetXProperty, newOffsetX, AnimationDuration, UseAnimations);
        AnimationHelper.StartAnimation(this, ContentOffsetYProperty, newOffsetY, AnimationDuration, UseAnimations);
    }

    /// <summary>
    /// Use animation to center the view on the specified point (in content coordinates).
    /// </summary>
    public void AnimatedSnapTo(Point contentPoint)
    {
        var newX = contentPoint.X - (ContentViewportWidth / 2);
        var newY = contentPoint.Y - (ContentViewportHeight / 2);

        AnimationHelper.StartAnimation(this, ContentOffsetXProperty, newX, AnimationDuration, UseAnimations);
        AnimationHelper.StartAnimation(this, ContentOffsetYProperty, newY, AnimationDuration, UseAnimations);
    }

    /// <summary>
    /// Zoom in/out centered on the specified point (in content coordinates).
    /// </summary>
    public void AnimatedZoomAboutPoint(
        double newContentZoom,
        Point contentZoomFocus)
    {
        newContentZoom = System.Math.Min(System.Math.Max(newContentZoom, MinimumZoomClamped), MaximumZoom);

        AnimationHelper.CancelAnimation(this, ContentZoomFocusXProperty);
        AnimationHelper.CancelAnimation(this, ContentZoomFocusYProperty);
        AnimationHelper.CancelAnimation(this, ViewportZoomFocusXProperty);
        AnimationHelper.CancelAnimation(this, ViewportZoomFocusYProperty);

        ContentZoomFocusX = contentZoomFocus.X;
        ContentZoomFocusY = contentZoomFocus.Y;
        ViewportZoomFocusX = (ContentZoomFocusX - ContentOffsetX) * InternalViewportZoom;
        ViewportZoomFocusY = (ContentZoomFocusY - ContentOffsetY) * InternalViewportZoom;

        enableContentOffsetUpdateFromScale = true;

        AnimationHelper.StartAnimation(
            this,
            InternalViewportZoomProperty,
            newContentZoom,
            AnimationDuration,
            (_, _) =>
            {
                enableContentOffsetUpdateFromScale = false;
                ResetViewportZoomFocus();
            },
            UseAnimations);
    }

    /// <summary>
    /// Instantly center the view on the specified point (in content coordinates).
    /// </summary>
    public void SnapContentOffsetTo(Point contentOffset)
    {
        AnimationHelper.CancelAnimation(this, ContentOffsetXProperty);
        AnimationHelper.CancelAnimation(this, ContentOffsetYProperty);

        ContentOffsetX = contentOffset.X;
        ContentOffsetY = contentOffset.Y;
    }

    /// <summary>
    /// Instantly center the view on the specified point (in content coordinates).
    /// </summary>
    public void SnapTo(Point contentPoint)
    {
        AnimationHelper.CancelAnimation(this, ContentOffsetXProperty);
        AnimationHelper.CancelAnimation(this, ContentOffsetYProperty);

        ContentOffsetX = contentPoint.X - (ContentViewportWidth / 2);
        ContentOffsetY = contentPoint.Y - (ContentViewportHeight / 2);
    }

    /// <summary>
    /// Instantly zoom to the specified rectangle (in content coordinates).
    /// </summary>
    public void ZoomTo(Rect contentRect)
    {
        var scaleX = ContentViewportWidth / contentRect.Width;
        var scaleY = ContentViewportHeight / contentRect.Height;
        var newScale = InternalViewportZoom * System.Math.Min(scaleX, scaleY);

        ZoomPointToViewportCenter(
            newScale,
            new Point(
                contentRect.X + (contentRect.Width / 2),
                contentRect.Y + (contentRect.Height / 2)));
    }

    /// <summary>
    /// Zoom in/out centered on the viewport center.
    /// </summary>
    public void ZoomTo(double viewportZoom)
    {
        var zoomCenter = new Point(
            ContentOffsetX + (ContentViewportWidth / 2),
            ContentOffsetY + (ContentViewportHeight / 2));

        ZoomAboutPoint(viewportZoom, zoomCenter);
    }

    /// <summary>
    /// Zoom in/out centered on the specified point (in content coordinates).
    /// </summary>
    public void ZoomAboutPoint(
        double newContentZoom,
        Point contentZoomFocus)
    {
        newContentZoom = System.Math.Min(System.Math.Max(newContentZoom, MinimumZoomClamped), MaximumZoom);

        var screenSpaceZoomOffsetX = (contentZoomFocus.X - ContentOffsetX) * InternalViewportZoom;
        var screenSpaceZoomOffsetY = (contentZoomFocus.Y - ContentOffsetY) * InternalViewportZoom;
        var contentSpaceZoomOffsetX = screenSpaceZoomOffsetX / newContentZoom;
        var contentSpaceZoomOffsetY = screenSpaceZoomOffsetY / newContentZoom;
        var newContentOffsetX = contentZoomFocus.X - contentSpaceZoomOffsetX;
        var newContentOffsetY = contentZoomFocus.Y - contentSpaceZoomOffsetY;

        AnimationHelper.CancelAnimation(this, InternalViewportZoomProperty);
        AnimationHelper.CancelAnimation(this, ContentOffsetXProperty);
        AnimationHelper.CancelAnimation(this, ContentOffsetYProperty);

        InternalViewportZoom = newContentZoom;
        ContentOffsetX = newContentOffsetX;
        ContentOffsetY = newContentOffsetY;
        RaiseCanExecuteChanged();

        ContentZoomChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Do animation that scales the content so that it fits completely in the control.
    /// </summary>
    public void AnimatedScaleToFit()
    {
        if (content is null)
        {
            throw new InvalidOperationException("PART_Content was not found in the ZoomBox visual template!");
        }

        ZoomTo(FillZoomValue);
    }

    /// <summary>
    /// Instantly scale the content so that it fits completely in the control.
    /// </summary>
    public void ScaleToFit()
    {
        if (content is null)
        {
            throw new InvalidOperationException("PART_Content was not found in the ZoomBox visual template!");
        }

        ZoomTo(FitZoomValue);
    }

    private void AnimatedZoomPointToViewportCenter(
        double newContentZoom,
        Point contentZoomFocus,
        EventHandler? callback)
    {
        newContentZoom = System.Math.Min(System.Math.Max(newContentZoom, MinimumZoomClamped), MaximumZoom);

        AnimationHelper.CancelAnimation(this, ContentZoomFocusXProperty);
        AnimationHelper.CancelAnimation(this, ContentZoomFocusYProperty);
        AnimationHelper.CancelAnimation(this, ViewportZoomFocusXProperty);
        AnimationHelper.CancelAnimation(this, ViewportZoomFocusYProperty);

        ContentZoomFocusX = contentZoomFocus.X;
        ContentZoomFocusY = contentZoomFocus.Y;
        ViewportZoomFocusX = (ContentZoomFocusX - ContentOffsetX) * InternalViewportZoom;
        ViewportZoomFocusY = (ContentZoomFocusY - ContentOffsetY) * InternalViewportZoom;

        enableContentOffsetUpdateFromScale = true;

        AnimationHelper.StartAnimation(
            this,
            InternalViewportZoomProperty,
            newContentZoom,
            AnimationDuration,
            (_, _) =>
            {
                enableContentOffsetUpdateFromScale = false;
                callback?.Invoke(this, EventArgs.Empty);
            },
            UseAnimations);

        AnimationHelper.StartAnimation(
            this,
            ViewportZoomFocusXProperty,
            ViewportWidth / 2,
            AnimationDuration,
            UseAnimations);
        AnimationHelper.StartAnimation(
            this,
            ViewportZoomFocusYProperty,
            ViewportHeight / 2,
            AnimationDuration,
            UseAnimations);
    }

    private void ZoomPointToViewportCenter(
        double newContentZoom,
        Point contentZoomFocus)
    {
        newContentZoom = System.Math.Min(System.Math.Max(newContentZoom, MinimumZoomClamped), MaximumZoom);

        AnimationHelper.CancelAnimation(this, InternalViewportZoomProperty);
        AnimationHelper.CancelAnimation(this, ContentOffsetXProperty);
        AnimationHelper.CancelAnimation(this, ContentOffsetYProperty);

        InternalViewportZoom = newContentZoom;
        ContentOffsetX = contentZoomFocus.X - (ContentViewportWidth / 2);
        ContentOffsetY = contentZoomFocus.Y - (ContentViewportHeight / 2);
    }
}