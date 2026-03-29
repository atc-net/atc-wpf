namespace Atc.Wpf.Network.Tests.ValueConverters;

public sealed class NetworkQualityCategoryTypeToResourceImageValueConverterTests
{
    private readonly IValueConverter converter = new NetworkQualityCategoryTypeToResourceImageValueConverter();

    [Fact]
    public void Instance_Is_Not_Null()
    {
        Assert.NotNull(NetworkQualityCategoryTypeToResourceImageValueConverter.Instance);
    }

    [Fact]
    public void Convert_Null_Throws_ArgumentNullException()
    {
        var exception = Record.Exception(() => converter.Convert(
            value: null,
            targetType: typeof(object),
            parameter: null,
            culture: CultureInfo.InvariantCulture));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void Convert_Non_NetworkQualityCategoryType_Throws_ArgumentException()
    {
        var exception = Record.Exception(() => converter.Convert(
            value: "NotAnEnum",
            targetType: typeof(object),
            parameter: null,
            culture: CultureInfo.InvariantCulture));

        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void ConvertBack_Throws_NotSupportedException()
    {
        var exception = Record.Exception(() => converter.ConvertBack(
            value: null,
            targetType: typeof(object),
            parameter: null,
            culture: CultureInfo.InvariantCulture));

        Assert.IsType<NotSupportedException>(exception);
    }
}