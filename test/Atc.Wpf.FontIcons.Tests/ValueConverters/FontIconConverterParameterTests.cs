namespace Atc.Wpf.FontIcons.Tests.ValueConverters;

public sealed class FontIconConverterParameterTests
{
    private const double DefaultEmSize = 100;
    private static readonly Brush DefaultBrush = Brushes.Black;

    [Fact]
    public void Resolve_Null_ReturnsTheDefaults()
    {
        var (brush, emSize) = FontIconConverterParameter.Resolve(null, DefaultBrush, DefaultEmSize);

        Assert.Same(DefaultBrush, brush);
        Assert.Equal(DefaultEmSize, emSize);
    }

    [Fact]
    public void Resolve_Brush_ReturnsTheBrushAndTheDefaultEmSize()
    {
        var (brush, emSize) = FontIconConverterParameter.Resolve(Brushes.Red, DefaultBrush, DefaultEmSize);

        Assert.Same(Brushes.Red, brush);
        Assert.Equal(DefaultEmSize, emSize);
    }

    [Fact]
    public void Resolve_Double_ReturnsTheEmSizeAndTheDefaultBrush()
    {
        var (brush, emSize) = FontIconConverterParameter.Resolve(24.0, DefaultBrush, DefaultEmSize);

        Assert.Same(DefaultBrush, brush);
        Assert.Equal(24.0, emSize);
    }

    // XAML passes ConverterParameter=24 as the string "24".
    [Theory]
    [InlineData("24", 24.0)]
    [InlineData("24.5", 24.5)]
    [InlineData(" 32 ", 32.0)]
    public void Resolve_NumericString_ReturnsTheEmSize(
        string parameter,
        double expected)
    {
        var (brush, emSize) = FontIconConverterParameter.Resolve(parameter, DefaultBrush, DefaultEmSize);

        Assert.Same(DefaultBrush, brush);
        Assert.Equal(expected, emSize);
    }

    [Fact]
    public void Resolve_NumericString_IsParsedWithTheInvariantCulture()
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("da-DK");

            var (_, emSize) = FontIconConverterParameter.Resolve("24.5", DefaultBrush, DefaultEmSize);

            Assert.Equal(24.5, emSize);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    // XAML passes ConverterParameter=Red as the string "Red".
    [Fact]
    public void Resolve_ColorNameString_ReturnsAFrozenBrushOfThatColor()
    {
        var (brush, emSize) = FontIconConverterParameter.Resolve("Red", DefaultBrush, DefaultEmSize);

        var solidColorBrush = Assert.IsType<SolidColorBrush>(brush);
        Assert.Equal(Colors.Red, solidColorBrush.Color);
        Assert.True(solidColorBrush.IsFrozen);
        Assert.Equal(DefaultEmSize, emSize);
    }

    [Fact]
    public void Resolve_HexColorString_ReturnsAFrozenBrushOfThatColor()
    {
        var (brush, _) = FontIconConverterParameter.Resolve("#FF00FF00", DefaultBrush, DefaultEmSize);

        var solidColorBrush = Assert.IsType<SolidColorBrush>(brush);
        Assert.Equal(Color.FromArgb(0xFF, 0x00, 0xFF, 0x00), solidColorBrush.Color);
        Assert.True(solidColorBrush.IsFrozen);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("NotABrushOrANumber")]
    public void Resolve_UnusableString_ReturnsTheDefaults(string parameter)
    {
        var (brush, emSize) = FontIconConverterParameter.Resolve(parameter, DefaultBrush, DefaultEmSize);

        Assert.Same(DefaultBrush, brush);
        Assert.Equal(DefaultEmSize, emSize);
    }

    [Fact]
    public void Resolve_OtherObject_ReturnsTheDefaults()
    {
        var (brush, emSize) = FontIconConverterParameter.Resolve(42, DefaultBrush, DefaultEmSize);

        Assert.Same(DefaultBrush, brush);
        Assert.Equal(DefaultEmSize, emSize);
    }
}