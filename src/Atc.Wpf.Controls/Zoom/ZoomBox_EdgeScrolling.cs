namespace Atc.Wpf.Controls.Zoom;

[SuppressMessage("Design", "MA0048:File name must match type name", Justification = "OK - partial class")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1601:Partial elements should be documented", Justification = "OK - partial class")]
public partial class ZoomBox
{
    private DispatcherTimer? edgeScrollTimer;

    public static readonly DependencyProperty IsEdgeScrollingEnabledProperty = DependencyProperty.Register(
        nameof(IsEdgeScrollingEnabled),
        typeof(bool),
        typeof(ZoomBox),
        new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Gets or sets a value indicating whether the viewport auto-pans when the
    /// mouse is near the edge during a drag operation.
    /// </summary>
    public bool IsEdgeScrollingEnabled
    {
        get => (bool)GetValue(IsEdgeScrollingEnabledProperty);
        set => SetValue(IsEdgeScrollingEnabledProperty, BooleanBoxes.Box(value));
    }

    [DependencyProperty(DefaultValue = 40.0)]
    private double edgeScrollZoneWidth;

    [DependencyProperty(DefaultValue = 5.0)]
    private double edgeScrollSpeed;

    internal void StartEdgeScrolling()
    {
        if (!IsEdgeScrollingEnabled || edgeScrollTimer is not null)
        {
            return;
        }

        edgeScrollTimer = new DispatcherTimer(DispatcherPriority.Normal)
        {
            Interval = TimeSpan.FromMilliseconds(16),
        };
        edgeScrollTimer.Tick += OnEdgeScrollTick;
        edgeScrollTimer.Start();
    }

    internal void StopEdgeScrolling()
    {
        if (edgeScrollTimer is null)
        {
            return;
        }

        edgeScrollTimer.Stop();
        edgeScrollTimer.Tick -= OnEdgeScrollTick;
        edgeScrollTimer = null;
    }

    private void OnEdgeScrollTick(
        object? sender,
        EventArgs e)
    {
        if (!IsMouseCaptured || content is null)
        {
            StopEdgeScrolling();
            return;
        }

        var mousePos = Mouse.GetPosition(this);
        var zone = EdgeScrollZoneWidth;
        var speed = EdgeScrollSpeed / InternalViewportZoom;
        var dx = 0.0;
        var dy = 0.0;

        if (mousePos.X < zone)
        {
            dx = -speed * (1.0 - (mousePos.X / zone));
        }
        else if (mousePos.X > ActualWidth - zone)
        {
            dx = speed * (1.0 - ((ActualWidth - mousePos.X) / zone));
        }

        if (mousePos.Y < zone)
        {
            dy = -speed * (1.0 - (mousePos.Y / zone));
        }
        else if (mousePos.Y > ActualHeight - zone)
        {
            dy = speed * (1.0 - ((ActualHeight - mousePos.Y) / zone));
        }

        if (System.Math.Abs(dx) > double.Epsilon || System.Math.Abs(dy) > double.Epsilon)
        {
            ContentOffsetX += dx;
            ContentOffsetY += dy;
        }
    }
}