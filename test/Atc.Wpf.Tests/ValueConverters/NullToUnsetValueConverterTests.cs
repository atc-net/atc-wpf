namespace Atc.Wpf.Tests.ValueConverters;

public sealed class NullToUnsetValueConverterTests
{
    private readonly IValueConverter converter = new NullToUnsetValueConverter();

    [Fact]
    public void Convert_Null_Returns_UnsetValue()
    {
        // Act
        var actual = converter.Convert(
            value: null,
            targetType: null,
            parameter: null,
            culture: null);

        // Assert
        Assert.Equal(DependencyProperty.UnsetValue, actual);
    }

    [Fact]
    public void Convert_String_Returns_String()
    {
        // Act
        var actual = converter.Convert(
            value: "hello",
            targetType: null,
            parameter: null,
            culture: null);

        // Assert
        Assert.Equal("hello", actual);
    }

    [Fact]
    public void Convert_Int_Returns_Int()
    {
        // Act
        var actual = converter.Convert(
            value: 42,
            targetType: null,
            parameter: null,
            culture: null);

        // Assert
        Assert.Equal(42, actual);
    }

    [Fact]
    public void ConvertBack_Returns_UnsetValue()
    {
        // Act
        var actual = converter.ConvertBack(
            value: "anything",
            targetType: null,
            parameter: null,
            culture: null);

        // Assert
        Assert.Equal(DependencyProperty.UnsetValue, actual);
    }
}