// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class StringEqualsToBoolValueConverterTests
{
    private readonly IValueConverter converter = new StringEqualsToBoolValueConverter();

    [Fact]
    public void Instance_Is_Not_Null()
        => Assert.NotNull(StringEqualsToBoolValueConverter.Instance);

    [Theory]
    [InlineData(true, "Active", "Active")]
    [InlineData(true, "Active", "active")]
    [InlineData(true, "Active", "ACTIVE")]
    [InlineData(false, "Active", "Inactive")]
    [InlineData(false, "Active", "")]
    [InlineData(false, "Active", null)]
    [InlineData(false, "", "Active")]
    [InlineData(false, null, "Active")]
    [InlineData(true, null, null)]
    [InlineData(true, "", "")]
    [InlineData(true, "", null)]
    [InlineData(true, null, "")]
    public void Convert(
        bool expected,
        string? input,
        string? parameter)
        => Assert.Equal(
            expected,
            converter.Convert(
                input,
                targetType: null,
                parameter: parameter,
                culture: null));

    [Fact]
    public void Convert_NonStringValue_UsesToString()
        => Assert.True(
            (bool)converter.Convert(
                value: 42,
                targetType: null,
                parameter: "42",
                culture: null));

    [Fact]
    public void ConvertBack_ThrowsNotSupportedException()
        => Assert.Throws<NotSupportedException>(() =>
            converter.ConvertBack(
                true,
                targetType: null,
                parameter: null,
                culture: null));
}