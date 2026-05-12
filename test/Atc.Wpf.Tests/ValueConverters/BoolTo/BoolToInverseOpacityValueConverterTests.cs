// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class BoolToInverseOpacityValueConverterTests
{
    private readonly IValueConverter converter = new BoolToInverseOpacityValueConverter();

    [Fact]
    public void Instance_Is_Not_Null()
        => Assert.NotNull(BoolToInverseOpacityValueConverter.Instance);

    [Theory]
    [InlineData(0.0, true)]
    [InlineData(1.0, false)]
    public void Convert(
        double expected,
        bool input)
        => Assert.Equal(
            expected,
            converter.Convert(
                input,
                targetType: null,
                parameter: null,
                culture: null));

    [Fact]
    public void Convert_Null_ReturnsOne()
        => Assert.Equal(
            1.0,
            converter.Convert(
                value: null,
                targetType: null,
                parameter: null,
                culture: null));

    [Fact]
    public void Convert_NonBool_ReturnsOne()
        => Assert.Equal(
            1.0,
            converter.Convert(
                value: "NotABool",
                targetType: null,
                parameter: null,
                culture: null));

    [Fact]
    public void ConvertBack_Throws_NotSupportedException()
    {
        var exception = Record.Exception(() => converter.ConvertBack(
            value: 0.0,
            targetType: null,
            parameter: null,
            culture: null));

        Assert.IsType<NotSupportedException>(exception);
        Assert.Equal("This is a OneWay converter.", exception.Message);
    }
}