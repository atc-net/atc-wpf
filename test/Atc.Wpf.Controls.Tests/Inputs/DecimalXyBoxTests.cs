namespace Atc.Wpf.Controls.Tests.Inputs;

public sealed class DecimalXyBoxTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Values_AreShownWithTheirFractionInTheInnerBoxes()
    {
        var sut = CreateMeasured(new DecimalXyBox { DecimalPlaces = 2, ValueX = 1.25m, ValueY = 2.75m });

        var (boxX, boxY) = GetInnerBoxes(sut);

        Assert.Equal(1.25, boxX.Value);
        Assert.Equal(2.75, boxY.Value);
    }

    [StaFact]
    public void MinimumAndMaximum_WithFractions_ArePassedToTheInnerBoxes()
    {
        var sut = CreateMeasured(new DecimalXyBox { Minimum = -1.5m, Maximum = 9.5m });

        var (boxX, _) = GetInnerBoxes(sut);

        Assert.Equal(-1.5, boxX.Minimum);
        Assert.Equal(9.5, boxX.Maximum);
    }

    [StaFact]
    public void InnerBoxValueChanged_KeepsTheFractionInTheDecimalValue()
    {
        var sut = CreateMeasured(new DecimalXyBox { DecimalPlaces = 2, ValueX = 1m });
        var (boxX, _) = GetInnerBoxes(sut);

        boxX.Value = 3.25;
        BindingOperations.GetBindingExpression(boxX, NumericBox.ValueProperty)?.UpdateSource();

        Assert.Equal(3.25m, sut.ValueX);
    }

    private static DecimalXyBox CreateMeasured(DecimalXyBox box)
    {
        box.Measure(new Size(400, 100));
        box.Arrange(new Rect(0, 0, 400, 100));
        Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle, new Action(() => { }));
        return box;
    }

    private static (DecimalBox X, DecimalBox Y) GetInnerBoxes(DecimalXyBox box)
    {
        var boxes = LogicalTreeHelper
            .GetChildren((DependencyObject)box.Content)
            .OfType<DecimalBox>()
            .ToArray();
        return (boxes[0], boxes[1]);
    }
}