namespace Atc.Wpf.Controls.Tests.Flyouts;

public sealed class FlyoutPositionTests
{
    [Fact]
    public void FlyoutPosition_HasExpectedValues()
    {
        // Assert
        Assert.Equal(0, (int)FlyoutPosition.Right);
        Assert.Equal(1, (int)FlyoutPosition.Left);
        Assert.Equal(2, (int)FlyoutPosition.Top);
        Assert.Equal(3, (int)FlyoutPosition.Bottom);
        Assert.Equal(4, (int)FlyoutPosition.Center);
    }

    [Fact]
    public void FlyoutPosition_HasFiveValues()
    {
        // Act
        var values = Enum.GetValues<FlyoutPosition>();

        // Assert
        Assert.Equal(5, values.Length);
    }

    [Theory]
    [InlineData(FlyoutPosition.Right, "Right")]
    [InlineData(FlyoutPosition.Left, "Left")]
    [InlineData(FlyoutPosition.Top, "Top")]
    [InlineData(FlyoutPosition.Bottom, "Bottom")]
    [InlineData(FlyoutPosition.Center, "Center")]
    public void FlyoutPosition_HasExpectedStringRepresentation(
        FlyoutPosition position,
        string expectedString)
    {
        // Act
        var result = position.ToString();

        // Assert
        Assert.Equal(expectedString, result);
    }

    [Theory]
    [InlineData("Right", FlyoutPosition.Right)]
    [InlineData("Left", FlyoutPosition.Left)]
    [InlineData("Top", FlyoutPosition.Top)]
    [InlineData("Bottom", FlyoutPosition.Bottom)]
    [InlineData("Center", FlyoutPosition.Center)]
    public void FlyoutPosition_CanBeParsedFromString(
        string input,
        FlyoutPosition expectedPosition)
    {
        // Act
        var result = Enum.Parse<FlyoutPosition>(input);

        // Assert
        Assert.Equal(expectedPosition, result);
    }

    [Theory]
    [InlineData(0, FlyoutPosition.Right)]
    [InlineData(1, FlyoutPosition.Left)]
    [InlineData(2, FlyoutPosition.Top)]
    [InlineData(3, FlyoutPosition.Bottom)]
    [InlineData(4, FlyoutPosition.Center)]
    public void FlyoutPosition_CanBeCastFromInt(
        int value,
        FlyoutPosition expectedPosition)
    {
        // Act
        var result = (FlyoutPosition)value;

        // Assert
        Assert.Equal(expectedPosition, result);
    }
}