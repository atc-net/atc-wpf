namespace Atc.Wpf.Tests.ValueConverters;

public sealed class ThicknessToDoubleValueConverterTests
{
    private readonly ThicknessToDoubleValueConverter sut = new();

    [Fact]
    public void Convert_WithLeftSide_ShouldReturnLeftValue()
    {
        // Arrange
        var thickness = new Thickness(10, 20, 30, 40);

        // Act
        var result = sut.Convert(thickness, typeof(double), ThicknessSideType.Left, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(10d);
    }

    [Fact]
    public void Convert_WithTopSide_ShouldReturnTopValue()
    {
        // Arrange
        var thickness = new Thickness(10, 20, 30, 40);

        // Act
        var result = sut.Convert(thickness, typeof(double), ThicknessSideType.Top, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(20d);
    }

    [Fact]
    public void Convert_WithRightSide_ShouldReturnRightValue()
    {
        // Arrange
        var thickness = new Thickness(10, 20, 30, 40);

        // Act
        var result = sut.Convert(thickness, typeof(double), ThicknessSideType.Right, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(30d);
    }

    [Fact]
    public void Convert_WithBottomSide_ShouldReturnBottomValue()
    {
        // Arrange
        var thickness = new Thickness(10, 20, 30, 40);

        // Act
        var result = sut.Convert(thickness, typeof(double), ThicknessSideType.Bottom, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(40d);
    }

    [Fact]
    public void Convert_WithNoSide_ShouldReturnZero()
    {
        // Arrange
        var thickness = new Thickness(10, 20, 30, 40);

        // Act
        var result = sut.Convert(thickness, typeof(double), ThicknessSideType.None, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(0d);
    }

    [Fact]
    public void Convert_WithPropertySideSet_ShouldUseProperty()
    {
        // Arrange
        var converter = new ThicknessToDoubleValueConverter { TakeThicknessSide = ThicknessSideType.Top };
        var thickness = new Thickness(10, 20, 30, 40);

        // Act
        var result = converter.Convert(thickness, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(20d);
    }

    [Fact]
    public void Convert_WithInvalidValue_ShouldReturnZero()
    {
        // Act
        var result = sut.Convert("not a thickness", typeof(double), ThicknessSideType.Left, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(0d);
    }

    [Fact]
    public void ConvertBack_WithLeftSide_ShouldReturnThicknessWithLeftValue()
    {
        // Act
        var result = sut.ConvertBack(15d, typeof(Thickness), ThicknessSideType.Left, CultureInfo.InvariantCulture);

        // Assert
        result.Should().BeOfType<Thickness>();
        var thickness = (Thickness)result!;
        thickness.Left.Should().Be(15d);
        thickness.Top.Should().Be(0d);
        thickness.Right.Should().Be(0d);
        thickness.Bottom.Should().Be(0d);
    }

    [Fact]
    public void ConvertBack_WithTopSide_ShouldReturnThicknessWithTopValue()
    {
        // Act
        var result = sut.ConvertBack(25d, typeof(Thickness), ThicknessSideType.Top, CultureInfo.InvariantCulture);

        // Assert
        result.Should().BeOfType<Thickness>();
        var thickness = (Thickness)result!;
        thickness.Left.Should().Be(0d);
        thickness.Top.Should().Be(25d);
        thickness.Right.Should().Be(0d);
        thickness.Bottom.Should().Be(0d);
    }

    [Fact]
    public void ConvertBack_WithRightSide_ShouldReturnThicknessWithRightValue()
    {
        // Act
        var result = sut.ConvertBack(35d, typeof(Thickness), ThicknessSideType.Right, CultureInfo.InvariantCulture);

        // Assert
        result.Should().BeOfType<Thickness>();
        var thickness = (Thickness)result!;
        thickness.Left.Should().Be(0d);
        thickness.Top.Should().Be(0d);
        thickness.Right.Should().Be(35d);
        thickness.Bottom.Should().Be(0d);
    }

    [Fact]
    public void ConvertBack_WithBottomSide_ShouldReturnThicknessWithBottomValue()
    {
        // Act
        var result = sut.ConvertBack(45d, typeof(Thickness), ThicknessSideType.Bottom, CultureInfo.InvariantCulture);

        // Assert
        result.Should().BeOfType<Thickness>();
        var thickness = (Thickness)result!;
        thickness.Left.Should().Be(0d);
        thickness.Top.Should().Be(0d);
        thickness.Right.Should().Be(0d);
        thickness.Bottom.Should().Be(45d);
    }

    [Fact]
    public void ConvertBack_WithNoSide_ShouldReturnDefaultThickness()
    {
        // Act
        var result = sut.ConvertBack(15d, typeof(Thickness), ThicknessSideType.None, CultureInfo.InvariantCulture);

        // Assert
        result.Should().BeOfType<Thickness>();
        var thickness = (Thickness)result!;
        thickness.Should().Be(default(Thickness));
    }

    [Fact]
    public void ConvertBack_WithInvalidValue_ShouldReturnDefaultThickness()
    {
        // Act
        var result = sut.ConvertBack("not a double", typeof(Thickness), ThicknessSideType.Left, CultureInfo.InvariantCulture);

        // Assert
        result.Should().BeOfType<Thickness>();
        ((Thickness)result!).Should().Be(default(Thickness));
    }
}
