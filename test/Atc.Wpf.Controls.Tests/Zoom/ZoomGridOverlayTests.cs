namespace Atc.Wpf.Controls.Tests.Zoom;

public sealed class ZoomGridOverlayTests
{
    [StaTheory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void BaseGridSpacing_InvalidValue_Throws(double value)
    {
        // A non-positive or non-finite spacing makes the adaptive-spacing loop in OnRender never terminate.
        var overlay = new ZoomGridOverlay(new ZoomBox());

        Assert.Throws<ArgumentOutOfRangeException>(() => overlay.BaseGridSpacing = value);
        Assert.Equal(50.0, overlay.BaseGridSpacing);
    }

    [StaFact]
    public void BaseGridSpacing_PositiveValue_IsAccepted()
    {
        var overlay = new ZoomGridOverlay(new ZoomBox());

        overlay.BaseGridSpacing = 0.25;

        Assert.Equal(0.25, overlay.BaseGridSpacing);
    }

    [StaFact]
    public void BaseGridSpacing_InvalidValueThroughBinding_IsRejected()
    {
        var overlay = new ZoomGridOverlay(new ZoomBox());

        BindingOperations.SetBinding(overlay, ZoomGridOverlay.BaseGridSpacingProperty, new Binding { Source = 0.0 });

        Assert.Equal(50.0, overlay.BaseGridSpacing);
    }

    [StaTheory]
    [InlineData(nameof(ZoomGridOverlay.MinorLineBrush))]
    [InlineData(nameof(ZoomGridOverlay.MajorLineBrush))]
    [InlineData(nameof(ZoomGridOverlay.MinorLineThickness))]
    [InlineData(nameof(ZoomGridOverlay.MajorLineThickness))]
    [InlineData(nameof(ZoomGridOverlay.BaseGridSpacing))]
    public void StylingProperty_IsADependencyPropertyThatAffectsRender(
        string propertyName)
    {
        // Bindable and themable (e.g. via SetResourceReference), and a change repaints the grid.
        var descriptor = DependencyPropertyDescriptor.FromName(propertyName, typeof(ZoomGridOverlay), typeof(ZoomGridOverlay));

        Assert.NotNull(descriptor);
        var metadata = Assert.IsType<FrameworkPropertyMetadata>(descriptor.DependencyProperty.GetMetadata(typeof(ZoomGridOverlay)));
        Assert.True(metadata.AffectsRender);
    }

    [StaFact]
    public void MajorLineBrush_CanBeBound()
    {
        var overlay = new ZoomGridOverlay(new ZoomBox());
        var brush = new SolidColorBrush(Colors.OrangeRed);

        BindingOperations.SetBinding(overlay, ZoomGridOverlay.MajorLineBrushProperty, new Binding { Source = brush });

        Assert.Same(brush, overlay.MajorLineBrush);
    }

    [SuppressMessage("Major Code Smell", "S1215:\"GC.Collect\" should not be called", Justification = "Forcing a collection is how the leak is detected.")]
    [StaFact]
    public void Detach_ReleasesTheOverlayWhileTheZoomBoxLives()
    {
        var zoomBox = new ZoomBox();

        var reference = CreateAndDetach(zoomBox);
        Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle, new Action(() => { }));
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        Assert.False(reference.IsAlive);
        GC.KeepAlive(zoomBox);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CreateAndDetach(ZoomBox zoomBox)
    {
        var overlay = new ZoomGridOverlay(zoomBox);
        overlay.Detach();
        return new WeakReference(overlay);
    }
}