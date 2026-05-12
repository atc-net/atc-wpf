// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class ObjectToTypeNameValueConverterTests
{
    private readonly IValueConverter converter = new ObjectToTypeNameValueConverter();

    [Fact]
    public void Instance_Is_Not_Null()
        => Assert.NotNull(ObjectToTypeNameValueConverter.Instance);

    [Fact]
    public void Convert_ReturnsTypeName_ForObject()
        => Assert.Equal(
            "String",
            converter.Convert(
                value: "hello",
                targetType: typeof(string),
                parameter: null,
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void Convert_ReturnsTypeName_ForInt()
        => Assert.Equal(
            "Int32",
            converter.Convert(
                value: 42,
                targetType: typeof(string),
                parameter: null,
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void Convert_FullParameter_ReturnsFullName()
        => Assert.Equal(
            "System.String",
            converter.Convert(
                value: "hello",
                targetType: typeof(string),
                parameter: "Full",
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void Convert_FullParameter_IsCaseInsensitive()
        => Assert.Equal(
            "System.String",
            converter.Convert(
                value: "hello",
                targetType: typeof(string),
                parameter: "full",
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void Convert_Null_ReturnsEmpty()
        => Assert.Equal(
            string.Empty,
            converter.Convert(
                value: null,
                targetType: typeof(string),
                parameter: null,
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void Convert_UnknownParameter_DefaultsToShortName()
        => Assert.Equal(
            "String",
            converter.Convert(
                value: "hello",
                targetType: typeof(string),
                parameter: "Bogus",
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void ConvertBack_Throws_NotSupportedException()
    {
        var exception = Record.Exception(() => converter.ConvertBack(
            value: "String",
            targetType: typeof(object),
            parameter: null,
            culture: CultureInfo.InvariantCulture));

        Assert.IsType<NotSupportedException>(exception);
        Assert.Equal("This is a OneWay converter.", exception.Message);
    }
}