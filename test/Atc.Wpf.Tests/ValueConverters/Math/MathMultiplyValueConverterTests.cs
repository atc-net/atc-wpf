// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters.Math;

public sealed class MathMultiplyValueConverterTests
{
    private readonly IValueConverter converter = new MathMultiplyValueConverter();

    [Theory]
    [InlineData(15.0, 5, "3")]
    [InlineData(0.0, 10, "0")]
    public void Convert(
        double expected,
        object input,
        string parameter)
    {
        var result = converter.Convert(input, targetType: null, parameter: parameter, culture: null);

        Assert.Equal(expected, (double)result!);
    }
}