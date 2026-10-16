namespace Atc.Wpf.Controls.Tests.Zoom;

public sealed class ZoomBoxTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Content_SetToNull_DoesNotThrow()
    {
        var zoomBox = new ZoomBox { Content = new Border() };

        var exception = Record.Exception(() => zoomBox.Content = null);

        Assert.Null(exception);
        Assert.Null(zoomBox.Content);
    }

    [StaFact]
    public void Content_SetToNonFrameworkElement_DoesNotThrow()
    {
        var zoomBox = new ZoomBox { Content = new Border() };

        var exception = Record.Exception(() => zoomBox.Content = "text");

        Assert.Null(exception);
        Assert.Equal("text", zoomBox.Content);
    }

    [StaFact]
    public void AnimatedScaleToFit_WideContent_ZoomsToTheFitValue()
    {
        // 800x600 content in a 400x400 viewport: fit is 0.5 (the whole content is visible), fill would be 0.667.
        var zoomBox = ZoomBoxFactory.CreateTemplated(new Canvas { Width = 800, Height = 600 }, 400, 400);
        zoomBox.UseAnimations = false;

        zoomBox.AnimatedScaleToFit();

        Assert.Equal(0.5, zoomBox.FitZoomValue, 6);
        Assert.Equal(zoomBox.FitZoomValue, zoomBox.ViewportZoom, 6);
    }

    [StaFact]
    public void MakeVisible_ElementAlreadyVisible_DoesNotCancelARunningOffsetAnimation()
    {
        var child = new Border { Width = 50, Height = 50 };
        Canvas.SetLeft(child, 10);
        Canvas.SetTop(child, 10);
        var canvas = new Canvas { Width = 800, Height = 600 };
        canvas.Children.Add(child);
        var zoomBox = ZoomBoxFactory.CreateTemplated(canvas, 400, 400);
        zoomBox.UseAnimations = true;
        zoomBox.AnimationDuration = 10;
        zoomBox.AnimatedSnapTo(new Point(400, 300));
        Assert.True(zoomBox.HasAnimatedProperties);

        zoomBox.MakeVisible(child, new Rect(0, 0, 50, 50));

        Assert.True(zoomBox.HasAnimatedProperties);
    }
}