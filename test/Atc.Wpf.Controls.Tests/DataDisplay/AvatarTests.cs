namespace Atc.Wpf.Controls.Tests.DataDisplay;

public sealed class AvatarTests
{
    [StaFact]
    public void Constructor_SetsDefaultValues()
    {
        // Arrange & Act
        var control = new Avatar();

        // Assert
        Assert.Equal(AvatarSize.Medium, control.Size);
        Assert.Equal(AvatarStatus.None, control.Status);
        Assert.Null(control.ImageSource);
        Assert.Null(control.Initials);
        Assert.Null(control.DisplayName);
        Assert.Equal(2d, control.StatusBorderThickness);
    }

    [StaTheory]
    [InlineData(AvatarSize.ExtraSmall)]
    [InlineData(AvatarSize.Small)]
    [InlineData(AvatarSize.Medium)]
    [InlineData(AvatarSize.Large)]
    [InlineData(AvatarSize.ExtraLarge)]
    public void Size_CanBeSet(AvatarSize expectedSize)
    {
        // Arrange
        var control = new Avatar();

        // Act
        control.Size = expectedSize;

        // Assert
        Assert.Equal(expectedSize, control.Size);
    }

    [StaTheory]
    [InlineData(AvatarStatus.None)]
    [InlineData(AvatarStatus.Online)]
    [InlineData(AvatarStatus.Offline)]
    [InlineData(AvatarStatus.Away)]
    [InlineData(AvatarStatus.Busy)]
    [InlineData(AvatarStatus.DoNotDisturb)]
    public void Status_CanBeSet(AvatarStatus expectedStatus)
    {
        // Arrange
        var control = new Avatar();

        // Act
        control.Status = expectedStatus;

        // Assert
        Assert.Equal(expectedStatus, control.Status);
    }

    [StaTheory]
    [InlineData("AB")]
    [InlineData("JD")]
    [InlineData("X")]
    public void Initials_CanBeSet(string expectedInitials)
    {
        // Arrange
        var control = new Avatar();

        // Act
        control.Initials = expectedInitials;

        // Assert
        Assert.Equal(expectedInitials, control.Initials);
    }

    [StaTheory]
    [InlineData("John Doe")]
    [InlineData("Alice")]
    [InlineData("Mary Jane Watson")]
    public void DisplayName_CanBeSet(string expectedDisplayName)
    {
        // Arrange
        var control = new Avatar();

        // Act
        control.DisplayName = expectedDisplayName;

        // Assert
        Assert.Equal(expectedDisplayName, control.DisplayName);
    }

    [Theory]
    [InlineData("John", "JO")]
    [InlineData("A", "A")]
    [InlineData("AB", "AB")]
    [InlineData("Alice Bob", "AB")]
    [InlineData("John Doe", "JD")]
    [InlineData("Mary Jane Watson", "MW")]
    [InlineData("", "?")]
    [InlineData("   ", "?")]
    public void GenerateInitialsFromName_ReturnsExpectedInitials(
        string name,
        string expectedInitials)
    {
        // Act
        var result = Avatar.GenerateInitialsFromName(name);

        // Assert
        Assert.Equal(expectedInitials, result);
    }

    [Fact]
    public void GenerateColorFromName_SameName_ReturnsSameColor()
    {
        // Arrange
        const string name = "John Doe";

        // Act
        var color1 = Avatar.GenerateColorFromName(name);
        var color2 = Avatar.GenerateColorFromName(name);

        // Assert
        Assert.Equal(color1, color2);
    }

    [Fact]
    public void GenerateColorFromName_DifferentNames_ReturnsDifferentColors()
    {
        // Arrange
        const string name1 = "John Doe";
        const string name2 = "Alice Smith";
        const string name3 = "Bob Johnson";

        // Act
        var color1 = Avatar.GenerateColorFromName(name1);
        var color2 = Avatar.GenerateColorFromName(name2);
        var color3 = Avatar.GenerateColorFromName(name3);

        // Assert - at least 2 of 3 colors should be different
        var uniqueColors = new HashSet<Color> { color1, color2, color3 };
        Assert.True(uniqueColors.Count >= 2);
    }

    [Fact]
    public void GenerateColorFromName_EmptyOrWhitespace_ReturnsGray()
    {
        // Act
        var colorEmpty = Avatar.GenerateColorFromName(string.Empty);
        var colorWhitespace = Avatar.GenerateColorFromName("   ");

        // Assert
        Assert.Equal(Colors.Gray, colorEmpty);
        Assert.Equal(Colors.Gray, colorWhitespace);
    }

    [StaFact]
    public void CornerRadius_CanBeSet()
    {
        // Arrange
        var control = new Avatar();
        var expectedCornerRadius = new CornerRadius(8);

        // Act
        control.CornerRadius = expectedCornerRadius;

        // Assert
        Assert.Equal(expectedCornerRadius, control.CornerRadius);
    }

    [StaFact]
    public void StatusBorderThickness_CanBeSet()
    {
        // Arrange
        var control = new Avatar();
        const double expectedThickness = 3d;

        // Act
        control.StatusBorderThickness = expectedThickness;

        // Assert
        Assert.Equal(expectedThickness, control.StatusBorderThickness);
    }
}