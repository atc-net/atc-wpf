// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters.Math;

public sealed class MathAddValueConverterTests
{
    private readonly IValueConverter converter = new MathAddValueConverter();

    [Theory]
    [InlineData(8.0, 5, "3")]
    [InlineData(10.0, 10, "0")]
    public void Convert(
        double expected,
        object input,
        string parameter)
    {
        var result = converter.Convert(input, targetType: null, parameter: parameter, culture: null);

        Assert.Equal(expected, (double)result!);
    }

    [Fact]
    public void Convert_Null_Returns_DoNothing()
        => Assert.Equal(
            Binding.DoNothing,
            converter.Convert(
                value: null,
                targetType: null,
                parameter: null,
                culture: null));
}