namespace Atc.Wpf.Forms.Tests.Extractors;

public sealed class LabelControlDataToLabelControlExtractorTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Extract_DecimalWithDecimalValue_CreatesDecimalBoxWithTheValue()
    {
        var data = new List<LabelControlData>
        {
            new(typeof(decimal), "Price") { Value = 9.95m },
        };

        var actual = LabelControlDataToLabelControlExtractor.Extract(data);

        var control = Assert.IsType<LabelDecimalBox>(Assert.Single(actual));
        Assert.Equal(9.95m, control.Value);
    }

    [StaFact]
    public void Extract_DoubleWithDoubleValue_CreatesDecimalBoxWithTheValue()
    {
        var data = new List<LabelControlData>
        {
            new(typeof(double), "Ratio") { Value = 2.5 },
        };

        var actual = LabelControlDataToLabelControlExtractor.Extract(data);

        var control = Assert.IsType<LabelDecimalBox>(Assert.Single(actual));
        Assert.Equal(2.5m, control.Value);
    }

    [StaFact]
    public void Extract_DecimalWithIntegerValue_CreatesDecimalBoxWithTheValue()
    {
        var data = new List<LabelControlData>
        {
            new(typeof(decimal), "Amount") { Value = 3 },
        };

        var actual = LabelControlDataToLabelControlExtractor.Extract(data);

        var control = Assert.IsType<LabelDecimalBox>(Assert.Single(actual));
        Assert.Equal(3m, control.Value);
    }

    [StaFact]
    public void Extract_DecimalWithoutValue_CreatesDecimalBoxWithZero()
    {
        var data = new List<LabelControlData>
        {
            new(typeof(decimal?), "Amount"),
        };

        var actual = LabelControlDataToLabelControlExtractor.Extract(data);

        var control = Assert.IsType<LabelDecimalBox>(Assert.Single(actual));
        Assert.Equal(0m, control.Value);
    }
}