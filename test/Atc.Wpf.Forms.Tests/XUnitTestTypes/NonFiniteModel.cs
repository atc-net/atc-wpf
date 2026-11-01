namespace Atc.Wpf.Forms.Tests.XUnitTestTypes;

public sealed class NonFiniteModel
{
    public double NotNumber { get; set; } = double.NaN;

    public double Infinity { get; set; } = double.PositiveInfinity;

    public double Huge { get; set; } = 1e30;

    public float HugeFloat { get; set; } = float.MinValue;

    public Size EmptySize { get; set; } = Size.Empty;

    public Point2D InfinitePoint { get; set; } = new(double.NegativeInfinity, double.NaN);

    [Range(0, double.MaxValue)]
    public double UnboundedDouble { get; set; } = 3.5;

    [Range(0, double.MaxValue)]
    public int UnboundedInt { get; set; } = 4;

    [Range(0, double.MaxValue)]
    public long UnboundedLong { get; set; } = 5;

    [Range(0, double.MaxValue)]
    public Size UnboundedSize { get; set; } = new(6, 7);

    [Range(0, double.MaxValue)]
    public Point2D UnboundedPoint { get; set; } = new(8, 9);
}