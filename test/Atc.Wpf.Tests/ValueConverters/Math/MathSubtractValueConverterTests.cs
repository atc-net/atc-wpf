// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters.Math;

public sealed class MathSubtractValueConverterTests
{
    private readonly IValueConverter converter = new MathSubtractValueConverter();

    [Theory]
    [InlineData(7.0, 10, "3")]
    [InlineData(0.0, 5, "5")]
    public void Convert(
        double expected,
        object input,
        string parameter)
    {
        var result = converter.Convert(input, targetType: null, parameter: parameter, culture: null);

        Assert.Equal(expected, (double)result!);
    }
}