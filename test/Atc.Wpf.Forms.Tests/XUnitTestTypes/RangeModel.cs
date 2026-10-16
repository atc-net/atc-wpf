namespace Atc.Wpf.Forms.Tests.XUnitTestTypes;

public sealed class RangeModel
{
    [Range(0, 100)]
    public double PercentWithIntegerRange { get; set; }

    [Range(0.5, 9.5)]
    public decimal RatioWithDoubleRange { get; set; }

    [Range(typeof(decimal), "1.25", "7.75")]
    public decimal PriceWithStringRange { get; set; }

    [Range(0.0, 10.0)]
    public int LevelWithDoubleRange { get; set; }

    [Range(typeof(int), "2", "8")]
    public int CountWithStringRange { get; set; }

    [Range(1, 4096)]
    public Size ResolutionWithIntegerRange { get; set; }

    public Size WindowSize { get; set; } = new(1920, 1080);
}