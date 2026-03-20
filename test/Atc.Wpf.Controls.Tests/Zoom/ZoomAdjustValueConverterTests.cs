namespace Atc.Wpf.Controls.Tests.Zoom;

public class ZoomAdjustValueConverterTests
{
    [Fact]
    public void Convert_Returns_Log_Of_Double()
    {
        // Arrange
        var converter = new ZoomAdjustValueConverter();

        // Act
        var result = converter.Convert(System.Math.E, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().NotBeNull();
        ((double)result!).Should().BeApproximately(1.0, 0.0001);
    }

    [Fact]
    public void Convert_Returns_Null_For_Non_Double()
    {
        // Arrange
        var converter = new ZoomAdjustValueConverter();

        // Act
        var result = converter.Convert("text", typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ConvertBack_Returns_Exp_Of_Double()
    {
        // Arrange
        var converter = new ZoomAdjustValueConverter();

        // Act
        var result = converter.ConvertBack(1.0, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().NotBeNull();
        ((double)result!).Should().BeApproximately(System.Math.E, 0.0001);
    }

    [Fact]
    public void ConvertBack_Returns_Null_For_Non_Double()
    {
        // Arrange
        var converter = new ZoomAdjustValueConverter();

        // Act
        var result = converter.ConvertBack("text", typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().BeNull();
    }
}