namespace Atc.Wpf.FontIcons.Tests.ValueConverters;

[Collection("Sequential")]
public sealed class FontIconImageSourceValueConverterTests
{
    private readonly IValueConverter converter = new FontIconImageSourceValueConverter();

    [Fact]
    public void Instance_Is_Not_Null()
        => Assert.NotNull(FontIconImageSourceValueConverter.Instance);

    [Fact]
    public void Convert_Null_ReturnsNull()
        => Assert.Null(converter.Convert(
            value: null,
            targetType: typeof(System.Windows.Media.ImageSource),
            parameter: null,
            culture: CultureInfo.InvariantCulture));

    [Fact]
    public void Convert_NonFontIconType_ReturnsNull()
        => Assert.Null(converter.Convert(
            value: 42,
            targetType: typeof(System.Windows.Media.ImageSource),
            parameter: null,
            culture: CultureInfo.InvariantCulture));

    [Fact]
    public void ConvertBack_Throws_NotSupportedException()
    {
        var exception = Record.Exception(() => converter.ConvertBack(
            value: null,
            targetType: typeof(FontAwesomeSolidType),
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
            FontIconImageSourceValueConverter.ResetToDefaults();
            Assert.Same(System.Windows.Media.Brushes.Black, FontIconImageSourceValueConverter.DefaultBrush);
            Assert.Equal(100.0, FontIconImageSourceValueConverter.DefaultEmSize);
        }
        finally
        {
            FontIconImageSourceValueConverter.ResetToDefaults();
        }
    }

    [Fact]
    public void DefaultBrush_IsSettable()
    {
        try
        {
            FontIconImageSourceValueConverter.DefaultBrush = System.Windows.Media.Brushes.Red;
            Assert.Same(System.Windows.Media.Brushes.Red, FontIconImageSourceValueConverter.DefaultBrush);
        }
        finally
        {
            FontIconImageSourceValueConverter.ResetToDefaults();
        }
    }

    [Fact]
    public void DefaultEmSize_IsSettable()
    {
        try
        {
            FontIconImageSourceValueConverter.DefaultEmSize = 24;
            Assert.Equal(24.0, FontIconImageSourceValueConverter.DefaultEmSize);
        }
        finally
        {
            FontIconImageSourceValueConverter.ResetToDefaults();
        }
    }

    [Fact]
    public void ResetToDefaults_RestoresBuiltInValues()
    {
        FontIconImageSourceValueConverter.DefaultBrush = System.Windows.Media.Brushes.HotPink;
        FontIconImageSourceValueConverter.DefaultEmSize = 42;

        FontIconImageSourceValueConverter.ResetToDefaults();

        Assert.Same(FontIconImageSourceValueConverter.BuiltInDefaultBrush, FontIconImageSourceValueConverter.DefaultBrush);
        Assert.Equal(FontIconImageSourceValueConverter.BuiltInDefaultEmSize, FontIconImageSourceValueConverter.DefaultEmSize);
    }
}