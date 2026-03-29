namespace Atc.Wpf.Components.Tests.ValueConverters;

public sealed class LogCategoryTypeToResourceImageValueConverterTests
{
    private readonly IValueConverter converter = new LogCategoryTypeToResourceImageValueConverter();

    [Fact]
    public void Instance_Is_Not_Null()
    {
        Assert.NotNull(LogCategoryTypeToResourceImageValueConverter.Instance);
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
    public void Convert_Non_LogCategoryType_Throws_ArgumentException()
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