namespace Atc.Wpf.Tests.ValueConverters;

public sealed class BoolToInverseBoolValueConverterTests
{
    private readonly IValueConverter converter = new BoolToInverseBoolValueConverter();

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void Convert(bool expected, bool input)
        => Assert.Equal(
            expected,
            converter.Convert(input, targetType: null, parameter: null, culture: null));

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void ConvertBack(bool expected, bool input)
        => Assert.Equal(
            expected,
            converter.ConvertBack(input, targetType: null, parameter: null, culture: null));
}