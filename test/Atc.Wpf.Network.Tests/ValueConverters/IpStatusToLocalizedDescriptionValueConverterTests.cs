namespace Atc.Wpf.Network.Tests.ValueConverters;

public sealed class IpStatusToLocalizedDescriptionValueConverterTests
{
    private readonly IValueConverter converter = new IpStatusToLocalizedDescriptionValueConverter();

    [Fact]
    public void Instance_Is_Not_Null()
    {
        Assert.NotNull(IpStatusToLocalizedDescriptionValueConverter.Instance);
    }

    [Fact]
    public void Convert_Null_Returns_DoNothing()
    {
        // Act
        var actual = converter.Convert(
            value: null,
            targetType: typeof(object),
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(Binding.DoNothing, actual);
    }

    [Fact]
    public void Convert_Non_IPStatus_Returns_DoNothing()
    {
        // Act
        var actual = converter.Convert(
            value: "NotAnIPStatus",
            targetType: typeof(object),
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(Binding.DoNothing, actual);
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