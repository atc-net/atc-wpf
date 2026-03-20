namespace Atc.Wpf.Controls.Tests.Zoom;

public class PointExtensionsTests
{
    [Fact]
    public void Clamp_Clamps_Negative_Values_To_Zero()
    {
        // Arrange
        var point = new Point(-5, -10);

        // Act
        var result = point.Clamp();

        // Assert
        result.X.Should().Be(0);
        result.Y.Should().Be(0);
    }

    [Fact]
    public void Clamp_Preserves_Positive_Values()
    {
        // Arrange
        var point = new Point(5, 10);

        // Act
        var result = point.Clamp();

        // Assert
        result.X.Should().Be(5);
        result.Y.Should().Be(10);
    }

    [Fact]
    public void Clamp_With_Bounds_Clamps_To_Bounds()
    {
        // Arrange
        var point = new Point(150, 250);
        var topLeft = new Point(0, 0);
        var bottomRight = new Point(100, 200);

        // Act
        var result = point.Clamp(topLeft, bottomRight);

        // Assert
        result.X.Should().Be(100);
        result.Y.Should().Be(200);
    }

    [Fact]
    public void FilterClamp_Returns_Null_When_Outside()
    {
        // Arrange
        var point = new Point(150, 50);

        // Act
        var result = point.FilterClamp(100, 100);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void FilterClamp_Returns_Point_When_Inside()
    {
        // Arrange
        var point = new Point(50, 50);

        // Act
        var result = point.FilterClamp(100, 100);

        // Assert
        result.Should().NotBeNull();
        result!.Value.X.Should().Be(50);
        result.Value.Y.Should().Be(50);
    }

    [Fact]
    public void FilterClamp_Returns_Null_When_Negative()
    {
        // Arrange
        var point = new Point(-1, 50);

        // Act
        var result = point.FilterClamp(100, 100);

        // Assert
        result.Should().BeNull();
    }
}