// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.ValueConverters;

public sealed class DoubleToGridLengthValueConverterTests
{
    private readonly IValueConverter converter = new DoubleToGridLengthValueConverter();

    [Fact]
    public void Convert_WithDouble_ReturnsPixelGridLength()
    {
        var result = converter.Convert(100.0, targetType: null, parameter: null, culture: null);

        var gridLength = Assert.IsType<GridLength>(result);
        Assert.Equal(100.0, gridLength.Value);
        Assert.Equal(GridUnitType.Pixel, gridLength.GridUnitType);
    }

    [Fact]
    public void Convert_WithPixelParameter_ReturnsPixelGridLength()
    {
        var result = converter.Convert(50.0, targetType: null, parameter: "Pixel", culture: null);

        var gridLength = Assert.IsType<GridLength>(result);
        Assert.Equal(50.0, gridLength.Value);
        Assert.Equal(GridUnitType.Pixel, gridLength.GridUnitType);
    }

    [Fact]
    public void Convert_WithStarParameter_ReturnsStarGridLength()
    {
        var result = converter.Convert(2.0, targetType: null, parameter: "Star", culture: null);

        var gridLength = Assert.IsType<GridLength>(result);
        Assert.Equal(2.0, gridLength.Value);
        Assert.Equal(GridUnitType.Star, gridLength.GridUnitType);
    }

    [Fact]
    public void Convert_WithAsteriskParameter_ReturnsStarGridLength()
    {
        var result = converter.Convert(3.0, targetType: null, parameter: "*", culture: null);

        var gridLength = Assert.IsType<GridLength>(result);
        Assert.Equal(3.0, gridLength.Value);
        Assert.Equal(GridUnitType.Star, gridLength.GridUnitType);
    }

    [Fact]
    public void Convert_WithAutoParameter_ReturnsAutoGridLength()
    {
        var result = converter.Convert(100.0, targetType: null, parameter: "Auto", culture: null);

        var gridLength = Assert.IsType<GridLength>(result);
        Assert.Equal(GridUnitType.Auto, gridLength.GridUnitType);
    }

    [Fact]
    public void Convert_WithInt_ReturnsPixelGridLength()
    {
        var result = converter.Convert(75, targetType: null, parameter: null, culture: null);

        var gridLength = Assert.IsType<GridLength>(result);
        Assert.Equal(75.0, gridLength.Value);
        Assert.Equal(GridUnitType.Pixel, gridLength.GridUnitType);
    }

    [Fact]
    public void Convert_WithFloat_ReturnsPixelGridLength()
    {
        var result = converter.Convert(25.5f, targetType: null, parameter: null, culture: null);

        var gridLength = Assert.IsType<GridLength>(result);
        Assert.Equal(25.5, gridLength.Value, precision: 1);
        Assert.Equal(GridUnitType.Pixel, gridLength.GridUnitType);
    }

    [Fact]
    public void Convert_WithInvalidType_ReturnsAutoGridLength()
    {
        var result = converter.Convert("invalid", targetType: null, parameter: null, culture: null);

        var gridLength = Assert.IsType<GridLength>(result);
        Assert.Equal(GridUnitType.Auto, gridLength.GridUnitType);
    }

    [Fact]
    public void ConvertBack_WithPixelGridLength_ReturnsDouble()
    {
        var gridLength = new GridLength(100.0, GridUnitType.Pixel);
        var result = converter.ConvertBack(gridLength, targetType: null, parameter: null, culture: null);

        Assert.Equal(100.0, result);
    }

    [Fact]
    public void ConvertBack_WithStarGridLength_ReturnsDouble()
    {
        var gridLength = new GridLength(2.0, GridUnitType.Star);
        var result = converter.ConvertBack(gridLength, targetType: null, parameter: null, culture: null);

        Assert.Equal(2.0, result);
    }

    [Fact]
    public void ConvertBack_WithAutoGridLength_ReturnsNaN()
    {
        var gridLength = new GridLength(1, GridUnitType.Auto);
        var result = converter.ConvertBack(gridLength, targetType: null, parameter: null, culture: null);

        Assert.Equal(double.NaN, result);
    }

    [Fact]
    public void ConvertBack_WithInvalidType_ReturnsZero()
    {
        var result = converter.ConvertBack("invalid", targetType: null, parameter: null, culture: null);

        Assert.Equal(0.0, result);
    }
}