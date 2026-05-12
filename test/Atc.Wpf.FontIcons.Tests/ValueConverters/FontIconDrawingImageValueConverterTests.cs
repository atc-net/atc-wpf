namespace Atc.Wpf.FontIcons.Tests.ValueConverters;

[Collection("Sequential")]
public sealed class FontIconDrawingImageValueConverterTests
{
    private readonly IValueConverter converter = new FontIconDrawingImageValueConverter();

    [Fact]
    public void Instance_Is_Not_Null()
        => Assert.NotNull(FontIconDrawingImageValueConverter.Instance);

    [Fact]
    public void Convert_Null_ReturnsNull()
        => Assert.Null(converter.Convert(
            value: null,
            targetType: typeof(System.Windows.Media.DrawingImage),
            parameter: null,
            culture: CultureInfo.InvariantCulture));

    [Fact]
    public void Convert_NonFontIconType_ReturnsNull()
        => Assert.Null(converter.Convert(
            value: "NotAnIcon",
            targetType: typeof(System.Windows.Media.DrawingImage),
            parameter: null,
            culture: CultureInfo.InvariantCulture));

    [Fact]
    public void ConvertBack_Throws_NotSupportedException()
    {
        var exception = Record.Exception(() => converter.ConvertBack(
            value: null,
            targetType: typeof(FontAwesomeRegularType),
            parameter: null,
            culture: CultureInfo.InvariantCulture));

        Assert.IsType<NotSupportedException>(exception);
        Assert.Equal("This is a OneWay converter.", exception.Message);
    }

    [Fact]
    public void Defaults_Match_BuiltInValues()
    {
        try
        {
            FontIconDrawingImageValueConverter.ResetToDefaults();
            Assert.Same(System.Windows.Media.Brushes.Black, FontIconDrawingImageValueConverter.DefaultBrush);
            Assert.Equal(100.0, FontIconDrawingImageValueConverter.DefaultEmSize);
        }
        finally
        {
            FontIconDrawingImageValueConverter.ResetToDefaults();
        }
    }

    [Fact]
    public void DefaultBrush_IsSettable()
    {
        try
        {
            FontIconDrawingImageValueConverter.DefaultBrush = System.Windows.Media.Brushes.Red;
            Assert.Same(System.Windows.Media.Brushes.Red, FontIconDrawingImageValueConverter.DefaultBrush);
        }
        finally
        {
            FontIconDrawingImageValueConverter.ResetToDefaults();
        }
    }

    [Fact]
    public void DefaultEmSize_IsSettable()
    {
        try
        {
            FontIconDrawingImageValueConverter.DefaultEmSize = 24;
            Assert.Equal(24.0, FontIconDrawingImageValueConverter.DefaultEmSize);
        }
        finally
        {
            FontIconDrawingImageValueConverter.ResetToDefaults();
        }
    }

    [Fact]
    public void ResetToDefaults_RestoresBuiltInValues()
    {
        FontIconDrawingImageValueConverter.DefaultBrush = System.Windows.Media.Brushes.HotPink;
        FontIconDrawingImageValueConverter.DefaultEmSize = 42;

        FontIconDrawingImageValueConverter.ResetToDefaults();

        Assert.Same(FontIconDrawingImageValueConverter.BuiltInDefaultBrush, FontIconDrawingImageValueConverter.DefaultBrush);
        Assert.Equal(FontIconDrawingImageValueConverter.BuiltInDefaultEmSize, FontIconDrawingImageValueConverter.DefaultEmSize);
    }
}