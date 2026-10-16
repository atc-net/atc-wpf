namespace Atc.Wpf.Controls.Tests.Layouts;

public sealed class GridLinesTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Step_ChangedWithoutAnApplication_DoesNotThrow()
    {
        var gridLines = new GridLines();

        var exception = Record.Exception(() => gridLines.HorizontalStep = 10);

        Assert.Null(exception);
    }

    [StaFact]
    public void Steps_Positive_DrawOneLinePerStep()
    {
        var gridLines = new GridLines { HorizontalStep = 25, VerticalStep = 50 };

        Layout(gridLines);

        // Vertical lines at x = 0, 25, 50, 75 and horizontal lines at y = 0, 50.
        Assert.Equal(6, ((Canvas)gridLines.Content).Children.Count);
    }

    [StaTheory]
    [InlineData(0d)]
    [InlineData(-5d)]
    [InlineData(double.NaN)]
    public void HorizontalStep_NotPositive_DrawsNoVerticalLines(double step)
    {
        var gridLines = new GridLines { VerticalStep = 20 };
        Layout(gridLines);

        gridLines.HorizontalStep = step;

        // Only the horizontal lines at y = 0, 20, 40, 60, 80 remain.
        Assert.Equal(5, ((Canvas)gridLines.Content).Children.Count);
    }

    private static void Layout(UIElement element)
    {
        element.Measure(new Size(100, 100));
        element.Arrange(new Rect(0, 0, 100, 100));
        element.UpdateLayout();
    }
}