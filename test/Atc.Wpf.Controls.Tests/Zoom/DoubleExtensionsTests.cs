namespace Atc.Wpf.Controls.Tests.Zoom;

public class DoubleExtensionsTests
{
    [Theory]
    [InlineData(1.0, 1.005, true)]
    [InlineData(1.0, 1.009, true)]
    [InlineData(1.0, 1.011, false)]
    [InlineData(100.0, 100.5, true)]
    [InlineData(100.0, 101.1, false)]
    [InlineData(0.5, 0.504, true)]
    [InlineData(0.5, 0.51, false)]
    public void IsWithinOnePercent_Returns_Expected(
        double value,
        double testValue,
        bool expected)
    {
        // Act
        var result = value.IsWithinOnePercent(testValue);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(1.5, 0, 1.5)]
    [InlineData(double.NaN, 0, 0)]
    [InlineData(double.PositiveInfinity, 0, 0)]
    [InlineData(double.NegativeInfinity, 0, 0)]
    [InlineData(double.NaN, 5.0, 5.0)]
    public void ToRealNumber_Returns_Expected(
        double value,
        double defaultValue,
        double expected)
    {
        // Act
        var result = value.ToRealNumber(defaultValue);

        // Assert
        result.Should().Be(expected);
    }
}