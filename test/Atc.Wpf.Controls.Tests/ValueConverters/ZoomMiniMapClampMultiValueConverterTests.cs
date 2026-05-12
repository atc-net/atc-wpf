namespace Atc.Wpf.Controls.Tests.ValueConverters;

public sealed class ZoomMiniMapClampMultiValueConverterTests
{
    private readonly IMultiValueConverter converter = new ZoomMiniMapClampMultiValueConverter();

    [Fact]
    public void Convert_NullValues_ReturnsUnsetValue()
        => Assert.Equal(
            DependencyProperty.UnsetValue,
            converter.Convert(
                values: null!,
                targetType: typeof(double),
                parameter: "width",
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void Convert_ShortValuesArray_ReturnsUnsetValue()
        => Assert.Equal(
            DependencyProperty.UnsetValue,
            converter.Convert(
                values: new object[] { 100.0, 0.0 },
                targetType: typeof(double),
                parameter: "width",
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void Convert_NullEntries_ReturnsUnsetValue()
        => Assert.Equal(
            DependencyProperty.UnsetValue,
            converter.Convert(
                values: new object[] { null!, 0.0, 1.0, null! },
                targetType: typeof(double),
                parameter: "width",
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void Convert_NullParameter_ReturnsUnsetValue()
        => Assert.Equal(
            DependencyProperty.UnsetValue,
            converter.Convert(
                values: new object[] { 100.0, 0.0, 1.0, new object() },
                targetType: typeof(double),
                parameter: null!,
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void Convert_NonZoomBoxAtIndex3_ReturnsUnsetValue()
        => Assert.Equal(
            DependencyProperty.UnsetValue,
            converter.Convert(
                values: new object[] { 100.0, 0.0, 1.0, "not a ZoomBox" },
                targetType: typeof(double),
                parameter: "width",
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void ConvertBack_Throws_NotSupportedException()
    {
        var exception = Record.Exception(() => converter.ConvertBack(
            value: 100.0,
            targetTypes: new[] { typeof(double), typeof(double), typeof(double), typeof(object) },
            parameter: "width",
            culture: CultureInfo.InvariantCulture));

        Assert.IsType<NotSupportedException>(exception);
    }
}