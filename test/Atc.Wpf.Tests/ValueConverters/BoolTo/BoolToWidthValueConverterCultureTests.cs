// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class BoolToWidthValueConverterCultureTests
{
    [Theory]
    [InlineData("en-US", "12.5", 12.5)]
    [InlineData("da-DK", "12.5", 12.5)]
    [InlineData("de-DE", "12.5", 12.5)]
    [InlineData("da-DK", "100", 100)]
    [InlineData("da-DK", "1in", 96)]
    [InlineData("da-DK", "0.5in", 48)]
    [InlineData("da-DK", "Auto", double.NaN)]
    public void Convert_parses_the_parameter_independently_of_the_current_culture(
        string cultureName,
        string parameter,
        double expected)
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo(cultureName);

            var actual = BoolToWidthValueConverter.Instance.Convert(
                value: true,
                typeof(double),
                parameter,
                CultureInfo.CurrentCulture);

            Assert.Equal(expected, actual);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [Theory]
    [InlineData("en-US", "12.5")]
    [InlineData("da-DK", "12.5")]
    public void Convert_returns_zero_for_false_whatever_the_culture(
        string cultureName,
        string parameter)
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo(cultureName);

            var actual = BoolToWidthValueConverter.Instance.Convert(
                value: false,
                typeof(double),
                parameter,
                CultureInfo.CurrentCulture);

            Assert.Equal(0d, actual);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}