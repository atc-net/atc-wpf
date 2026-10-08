namespace Atc.Wpf.Controls.Tests.Zoom;

public sealed class ZoomMiniMapTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    // The canvas is 10x wider than the laid-out minimap, so an on-screen thickness of 1 is 10 content units.
    private const string TemplateXaml = """
        <ControlTemplate
            xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
            xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
            xmlns:zoom="clr-namespace:Atc.Wpf.Controls.Zoom;assembly=Atc.Wpf.Controls"
            TargetType="zoom:ZoomMiniMap">
            <Canvas x:Name="PART_Content" Width="1000" Height="500">
                <Border x:Name="PART_DraggingBorder" BorderBrush="Blue" BorderThickness="10" />
                <Border x:Name="PART_SizingBorder" BorderBrush="Blue" BorderThickness="10" />
            </Canvas>
        </ControlTemplate>
        """;

    [StaFact]
    public void ViewportBorderBrush_SetBeforeTemplate_IsAppliedToTheViewportBorders()
    {
        var sut = new ZoomMiniMap { ViewportBorderBrush = Brushes.Red };

        var (dragging, sizing) = ApplyTemplate(sut);

        Assert.Same(Brushes.Red, dragging.BorderBrush);
        Assert.Same(Brushes.Red, sizing.BorderBrush);
    }

    [StaFact]
    public void ViewportBorderBrush_ChangedAfterTemplate_UpdatesTheViewportBorders()
    {
        var sut = new ZoomMiniMap();
        var (dragging, sizing) = ApplyTemplate(sut);

        sut.ViewportBorderBrush = Brushes.Green;

        Assert.Same(Brushes.Green, dragging.BorderBrush);
        Assert.Same(Brushes.Green, sizing.BorderBrush);
    }

    [StaFact]
    public void ViewportBorderBrush_ResetToNull_RestoresTheTemplateBrush()
    {
        var sut = new ZoomMiniMap { ViewportBorderBrush = Brushes.Red };
        var (dragging, _) = ApplyTemplate(sut);

        sut.ViewportBorderBrush = null;

        Assert.Equal(Colors.Blue, ((SolidColorBrush)dragging.BorderBrush).Color);
    }

    [StaFact]
    public void ViewportBorderThickness_Set_IsScaledToContentUnits()
    {
        var sut = new ZoomMiniMap { BorderThickness = new Thickness(1), ViewportBorderThickness = 2 };

        var (dragging, sizing) = ApplyTemplateAndLayout(sut);

        Assert.Equal(new Thickness(20), dragging.BorderThickness);
        Assert.Equal(new Thickness(20), sizing.BorderThickness);
    }

    [StaFact]
    public void ViewportBorderThickness_ChangedAfterLayout_UpdatesTheViewportBorders()
    {
        var sut = new ZoomMiniMap { BorderThickness = new Thickness(1) };
        var (dragging, sizing) = ApplyTemplateAndLayout(sut);

        sut.ViewportBorderThickness = 3;

        Assert.Equal(new Thickness(30), dragging.BorderThickness);
        Assert.Equal(new Thickness(30), sizing.BorderThickness);
    }

    [StaFact]
    public void ViewportBorderThickness_ResetToNull_FallsBackToBorderThickness()
    {
        var sut = new ZoomMiniMap { BorderThickness = new Thickness(1), ViewportBorderThickness = 2 };
        var (dragging, _) = ApplyTemplateAndLayout(sut);

        sut.ViewportBorderThickness = null;

        Assert.Equal(new Thickness(10), dragging.BorderThickness);
    }

    private static (Border Dragging, Border Sizing) ApplyTemplate(
        ZoomMiniMap miniMap)
    {
        miniMap.Template = (ControlTemplate)XamlReader.Parse(TemplateXaml);
        miniMap.ApplyTemplate();

        return (
            (Border)miniMap.Template.FindName("PART_DraggingBorder", miniMap),
            (Border)miniMap.Template.FindName("PART_SizingBorder", miniMap));
    }

    private static (Border Dragging, Border Sizing) ApplyTemplateAndLayout(
        ZoomMiniMap miniMap)
    {
        var borders = ApplyTemplate(miniMap);
        miniMap.Measure(new Size(100, 50));
        miniMap.Arrange(new Rect(0, 0, 100, 50));
        miniMap.UpdateLayout();
        return borders;
    }
}