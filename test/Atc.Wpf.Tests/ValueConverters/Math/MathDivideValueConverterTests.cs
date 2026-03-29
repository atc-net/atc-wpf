// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters.Math;

public sealed class MathDivideValueConverterTests
{
    private readonly IValueConverter converter = new MathDivideValueConverter();

    [Fact]
    public void Convert_Divides_Values()
    {
        var result = converter.Convert(10, targetType: null, parameter: "2", culture: null);

        Assert.Equal(5.0, (double)result!);
    }

    [Fact]
    public void Convert_DivisorZero_Returns_DoNothing()
        => Assert.Equal(
            Binding.DoNothing,
            converter.Convert(
                10,
                targetType: null,
                parameter: "0",
                culture: null));

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