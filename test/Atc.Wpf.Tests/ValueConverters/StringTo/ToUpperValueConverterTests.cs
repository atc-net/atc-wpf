// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters.StringTo;

public sealed class ToUpperValueConverterTests
{
    private readonly IValueConverter converter = new ToUpperValueConverter();

    [Theory]
    [InlineData("HELLO", "hello")]
    [InlineData("HELLO WORLD", "Hello World")]
    [InlineData("ALREADY", "ALREADY")]
    public void Convert_String(
        string expected,
        string input)
        => Assert.Equal(
            expected,
            converter.Convert(
                input,
                targetType: null,
                parameter: null,
                culture: null));

    [Fact]
    public void Convert_NonString_Returns_Value_Unchanged()
        => Assert.Equal(
            42,
            converter.Convert(
                42,
                targetType: null,
                parameter: null,
                culture: null));

    [Fact]
    public void ConvertBack_Returns_DoNothing()
        => Assert.Equal(
            Binding.DoNothing,
            converter.ConvertBack(
                value: "HELLO",
                targetType: null,
                parameter: null,
                culture: null));
}