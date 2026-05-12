namespace Atc.Wpf.Controls.Tests.ValueConverters;

public sealed class BoolToPopoverTriggerModeValueConverterTests
{
    private readonly IValueConverter converter = new BoolToPopoverTriggerModeValueConverter();

    [Fact]
    public void Instance_Is_Not_Null()
        => Assert.NotNull(BoolToPopoverTriggerModeValueConverter.Instance);

    [Theory]
    [InlineData(PopoverTriggerMode.Hover, true)]
    [InlineData(PopoverTriggerMode.Manual, false)]
    public void Convert(
        PopoverTriggerMode expected,
        bool input)
        => Assert.Equal(
            expected,
            converter.Convert(
                input,
                targetType: null,
                parameter: null,
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void Convert_Null_ReturnsManual()
        => Assert.Equal(
            PopoverTriggerMode.Manual,
            converter.Convert(
                value: null,
                targetType: null,
                parameter: null,
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void Convert_NonBool_ReturnsManual()
        => Assert.Equal(
            PopoverTriggerMode.Manual,
            converter.Convert(
                value: "NotABool",
                targetType: null,
                parameter: null,
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void ConvertBack_Throws_NotSupportedException()
    {
        var exception = Record.Exception(() => converter.ConvertBack(
            value: PopoverTriggerMode.Hover,
            targetType: null,
            parameter: null,
            culture: CultureInfo.InvariantCulture));

        Assert.IsType<NotSupportedException>(exception);
        Assert.Equal("This is a OneWay converter.", exception.Message);
    }
}