// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters.StringTo;

public sealed class ToLowerValueConverterTests
{
    private readonly IValueConverter converter = new ToLowerValueConverter();

    [Theory]
    [InlineData("hello", "HELLO")]
    [InlineData("hello world", "Hello World")]
    [InlineData("already", "already")]
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
                value: "hello",
                targetType: null,
                parameter: null,
                culture: null));
}