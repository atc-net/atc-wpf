namespace Atc.Wpf.Controls.Tests.Layouts;

public sealed class RowTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Measure_NoChildrenWithGutter_DoesNotThrow()
    {
        var row = new Row { Gutter = 16 };

        var exception = Record.Exception(() => row.Measure(new Size(500, 500)));

        Assert.Null(exception);
        Assert.Equal(0d, row.DesiredSize.Height);
    }

    [StaFact]
    public void Measure_OnlyCollapsedChildWithGutter_DoesNotThrow()
    {
        var row = new Row { Gutter = 16 };
        row.Children.Add(new Col { Content = new Border { Height = 40 }, Visibility = Visibility.Collapsed });

        var exception = Record.Exception(() => row.Measure(new Size(500, 500)));

        Assert.Null(exception);
    }

    [StaFact]
    public void Measure_ChildShrinks_DesiredHeightShrinksToo()
    {
        var content = new Border { Height = 100 };
        var row = new Row();
        row.Children.Add(new Col { Content = content });
        row.Measure(new Size(500, double.PositiveInfinity));
        Assert.Equal(100d, row.DesiredSize.Height);

        content.Height = 20;
        row.UpdateLayout();

        Assert.Equal(20d, row.DesiredSize.Height);
    }

    [StaFact]
    public void Measure_FirstPass_UsesTheBreakpointOfTheAvailableWidth()
    {
        // Two half-width columns share one line at the Xxl breakpoint; their Xs span (24) would need two lines.
        var row = new Row();
        row.Children.Add(new Col { Layout = new ColLayout { Xs = 24, Xxl = 12 }, Content = new Border { Height = 40 } });
        row.Children.Add(new Col { Layout = new ColLayout { Xs = 24, Xxl = 12 }, Content = new Border { Height = 40 } });

        row.Measure(new Size(3000, double.PositiveInfinity));

        Assert.Equal(40d, row.DesiredSize.Height);
    }
}