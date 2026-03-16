namespace Atc.Wpf.Controls.Tests.Zoom;

public class ViewportHelpersTests
{
    [Theory]
    [InlineData(800, 600, 1600, 1200, 0.5)]
    [InlineData(800, 600, 400, 300, 2.0)]
    [InlineData(800, 600, 800, 600, 1.0)]
    [InlineData(800, 600, 1600, 600, 0.5)]
    [InlineData(800, 600, 800, 1200, 0.5)]
    public void FitZoom_Returns_Expected(
        double actualWidth,
        double actualHeight,
        double contentWidth,
        double contentHeight,
        double expected)
    {
        // Act
        var result = ViewportHelpers.FitZoom(actualWidth, actualHeight, contentWidth, contentHeight);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void FitZoom_Returns_1_When_ContentWidth_Is_Null()
    {
        // Act
        var result = ViewportHelpers.FitZoom(800, 600, null, 600);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public void FitZoom_Returns_1_When_ContentHeight_Is_Null()
    {
        // Act
        var result = ViewportHelpers.FitZoom(800, 600, 800, null);

        // Assert
        result.Should().Be(1);
    }

    [Theory]
    [InlineData(800, 600, 1200, 1200, 0.6666666666666666)]
    [InlineData(800, 600, 400, 300, 2.0)]
    [InlineData(800, 600, 800, 600, 1.0)]
    [InlineData(800, 600, 1600, 600, 1.0)]
    [InlineData(800, 600, 800, 1200, 1.0)]
    public void FillZoom_Returns_Expected(
        double actualWidth,
        double actualHeight,
        double contentWidth,
        double contentHeight,
        double expected)
    {
        // Act
        var result = ViewportHelpers.FillZoom(actualWidth, actualHeight, contentWidth, contentHeight);

        // Assert
        result.Should().BeApproximately(expected, 0.0001);
    }

    [Fact]
    public void FillZoom_Returns_1_When_Content_Is_Null()
    {
        // Act
        var result = ViewportHelpers.FillZoom(800, 600, null, null);

        // Assert
        result.Should().Be(1);
    }
}