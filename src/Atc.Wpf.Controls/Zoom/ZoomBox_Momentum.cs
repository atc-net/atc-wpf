namespace Atc.Wpf.Controls.Zoom;

[SuppressMessage("Design", "MA0048:File name must match type name", Justification = "OK - partial class")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1601:Partial elements should be documented", Justification = "OK - partial class")]
public partial class ZoomBox
{
    private DispatcherTimer? momentumTimer;
    private double momentumVelocity;
    private Point momentumFocus;

    public static readonly DependencyProperty IsZoomMomentumEnabledProperty = DependencyProperty.Register(
        nameof(IsZoomMomentumEnabled),
        typeof(bool),
        typeof(ZoomBox),
        new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Gets or sets a value indicating whether zoom momentum is enabled.
    /// When <see langword="true"/>, a fast scroll-zoom gesture continues
    /// briefly with decaying momentum after release.
    /// Default is <see langword="false"/> (opt-in).
    /// </summary>
    public bool IsZoomMomentumEnabled
    {
        get => (bool)GetValue(IsZoomMomentumEnabledProperty);
        set => SetValue(IsZoomMomentumEnabledProperty, BooleanBoxes.Box(value));
    }

    [DependencyProperty(DefaultValue = 0.85)]
    private double momentumFriction;

    internal void ApplyZoomMomentum(
        double velocity,
        Point focus)
    {
        if (!IsZoomMomentumEnabled || System.Math.Abs(velocity) < 0.001)
        {
            return;
        }

        momentumVelocity = velocity;
        momentumFocus = focus;

        if (momentumTimer is not null)
        {
            momentumTimer.Stop();
        }
        else
        {
            momentumTimer = new DispatcherTimer(DispatcherPriority.Render)
            {
                Interval = TimeSpan.FromMilliseconds(16),
            };
            momentumTimer.Tick += OnMomentumTick;
        }

        momentumTimer.Start();
    }

    internal void StopMomentum()
    {
        momentumTimer?.Stop();
        momentumVelocity = 0;
    }

    private void OnMomentumTick(
        object? sender,
        EventArgs e)
    {
        momentumVelocity *= MomentumFriction;

        if (System.Math.Abs(momentumVelocity) < 0.001)
        {
            StopMomentum();
            return;
        }

        var zoomFactor = 1.0 + (momentumVelocity * 0.1);
        var newZoom = InternalViewportZoom * zoomFactor;
        newZoom = System.Math.Min(System.Math.Max(newZoom, MinimumZoomClamped), MaximumZoom);

        if (System.Math.Abs(newZoom - InternalViewportZoom) < 0.0001)
        {
            StopMomentum();
            return;
        }

        ZoomAboutPoint(newZoom, momentumFocus);
    }
}