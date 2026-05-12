// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class PathToFilenameValueConverterTests
{
    private readonly IValueConverter converter = new PathToFilenameValueConverter();

    [Fact]
    public void Instance_Is_Not_Null()
        => Assert.NotNull(PathToFilenameValueConverter.Instance);

    [Theory]
    [InlineData(@"C:\foo\bar.txt", "bar.txt")]
    [InlineData(@"C:\foo\bar\baz.json", "baz.json")]
    [InlineData("bar.txt", "bar.txt")]
    [InlineData("/usr/local/bin/app", "app")]
    [InlineData(@"C:\", "")]
    public void Convert_ReturnsFileName(
        string input,
        string expected)
        => Assert.Equal(
            expected,
            converter.Convert(
                input,
                targetType: typeof(string),
                parameter: null,
                culture: CultureInfo.InvariantCulture));

    [Theory]
    [InlineData(@"C:\foo\bar.txt", "bar")]
    [InlineData("bar.json", "bar")]
    [InlineData(@"C:\foo\noext", "noext")]
    public void Convert_WithoutExtension_StripsExtension(
        string input,
        string expected)
        => Assert.Equal(
            expected,
            converter.Convert(
                input,
                targetType: typeof(string),
                parameter: "WithoutExtension",
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void Convert_WithoutExtension_IsCaseInsensitive()
        => Assert.Equal(
            "bar",
            converter.Convert(
                @"C:\foo\bar.txt",
                targetType: typeof(string),
                parameter: "withoutextension",
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
    public void Convert_EmptyString_ReturnsEmpty()
        => Assert.Equal(
            string.Empty,
            converter.Convert(
                value: string.Empty,
                targetType: typeof(string),
                parameter: null,
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void ConvertBack_Throws_NotSupportedException()
    {
        var exception = Record.Exception(() => converter.ConvertBack(
            value: "bar.txt",
            targetType: typeof(string),
            parameter: null,
            culture: CultureInfo.InvariantCulture));

        Assert.IsType<NotSupportedException>(exception);
        Assert.Equal("This is a OneWay converter.", exception.Message);
    }
}