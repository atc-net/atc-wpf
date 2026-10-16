namespace Atc.Wpf.Forms.Tests.Factories;

public sealed class LabelControlFactoryTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaTheory]
    [InlineData(nameof(RangeModel.PercentWithIntegerRange), 0, 100)]
    [InlineData(nameof(RangeModel.RatioWithDoubleRange), 0.5, 9.5)]
    [InlineData(nameof(RangeModel.PriceWithStringRange), 1.25, 7.75)]
    public void CreateLabelDecimalBox_WithRangeAttribute_UsesMinimumAndMaximum(
        string propertyName,
        double expectedMinimum,
        double expectedMaximum)
    {
        var propertyInfo = typeof(RangeModel).GetProperty(propertyName)!;

        var actual = LabelControlFactory.CreateLabelDecimalBox(
            propertyInfo,
            groupIdentifier: string.Empty,
            isReadOnly: false,
            propertyInfo.PropertyType,
            value: 2);

        Assert.Equal((decimal)expectedMinimum, actual.Minimum);
        Assert.Equal((decimal)expectedMaximum, actual.Maximum);
    }

    [StaTheory]
    [InlineData(nameof(RangeModel.LevelWithDoubleRange), 0, 10)]
    [InlineData(nameof(RangeModel.CountWithStringRange), 2, 8)]
    public void CreateLabelIntegerBox_WithRangeAttribute_UsesMinimumAndMaximum(
        string propertyName,
        int expectedMinimum,
        int expectedMaximum)
    {
        var propertyInfo = typeof(RangeModel).GetProperty(propertyName)!;

        var actual = LabelControlFactory.CreateLabelIntegerBox(
            propertyInfo,
            groupIdentifier: string.Empty,
            isReadOnly: false,
            propertyInfo.PropertyType,
            value: 5);

        Assert.Equal(expectedMinimum, actual.Minimum);
        Assert.Equal(expectedMaximum, actual.Maximum);
    }

    [StaFact]
    public void CreateLabelPixelSizeBox_WithRangeAttribute_UsesMinimumAndMaximum()
    {
        var propertyInfo = typeof(RangeModel).GetProperty(nameof(RangeModel.ResolutionWithIntegerRange))!;

        var actual = LabelControlFactory.CreateLabelPixelSizeBox(
            propertyInfo,
            groupIdentifier: string.Empty,
            isReadOnly: false,
            propertyInfo.PropertyType,
            valueWidth: 1920,
            valueHeight: 1080);

        Assert.Equal(1, actual.Minimum);
        Assert.Equal(4096, actual.Maximum);
    }

    [StaFact]
    public void CreateLabelPixelSizeBox_WithoutRange_KeepsTheDefaultLimitsAndTheValues()
    {
        var propertyInfo = typeof(RangeModel).GetProperty(nameof(RangeModel.WindowSize))!;
        var defaults = new LabelPixelSizeBox();

        var actual = LabelControlFactory.CreateLabelPixelSizeBox(
            propertyInfo,
            groupIdentifier: string.Empty,
            isReadOnly: false,
            propertyInfo.PropertyType,
            valueWidth: 1920,
            valueHeight: 1080);

        Assert.Equal(defaults.Minimum, actual.Minimum);
        Assert.Equal(defaults.Maximum, actual.Maximum);
        Assert.Equal(1920, actual.ValueWidth);
        Assert.Equal(1080, actual.ValueHeight);
    }
}