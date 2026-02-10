namespace Atc.Wpf.Components.Tests.Flyouts;

public sealed class FlyoutTests
{
    [StaFact]
    public void Constructor_SetsDefaultValues()
    {
        // Arrange & Act
        var control = new Flyout();

        // Assert
        Assert.False(control.IsOpen);
        Assert.Equal(FlyoutPosition.Right, control.Position);
        Assert.Equal(400.0, control.FlyoutWidth);
        Assert.Equal(300.0, control.FlyoutHeight);
        Assert.Null(control.Header);
        Assert.Null(control.HeaderTemplate);
        Assert.True(control.IsLightDismissEnabled);
        Assert.True(control.ShowOverlay);
        Assert.Equal(0.5, control.OverlayOpacity);
        Assert.True(control.ShowCloseButton);
        Assert.Equal(300.0, control.AnimationDuration);
        Assert.True(control.CloseOnEscape);
        Assert.False(control.IsPinned);
        Assert.False(control.ShowPinButton);
        Assert.False(control.IsResizable);
        Assert.Equal(200.0, control.MinFlyoutWidth);
        Assert.Equal(800.0, control.MaxFlyoutWidth);
        Assert.Equal(150.0, control.MinFlyoutHeight);
        Assert.Equal(600.0, control.MaxFlyoutHeight);
        Assert.Null(control.EasingFunction);
        Assert.Null(control.Result);
    }

    [StaTheory]
    [InlineData(FlyoutPosition.Right)]
    [InlineData(FlyoutPosition.Left)]
    [InlineData(FlyoutPosition.Top)]
    [InlineData(FlyoutPosition.Bottom)]
    [InlineData(FlyoutPosition.Center)]
    public void Position_CanBeSet(FlyoutPosition expectedPosition)
    {
        // Arrange
        var control = new Flyout();

        // Act
        control.Position = expectedPosition;

        // Assert
        Assert.Equal(expectedPosition, control.Position);
    }

    [StaTheory]
    [InlineData(100.0)]
    [InlineData(400.0)]
    [InlineData(800.0)]
    public void FlyoutWidth_CanBeSet(double expectedWidth)
    {
        // Arrange
        var control = new Flyout();

        // Act
        control.FlyoutWidth = expectedWidth;

        // Assert
        Assert.Equal(expectedWidth, control.FlyoutWidth);
    }

    [StaTheory]
    [InlineData(100.0)]
    [InlineData(300.0)]
    [InlineData(600.0)]
    public void FlyoutHeight_CanBeSet(double expectedHeight)
    {
        // Arrange
        var control = new Flyout();

        // Act
        control.FlyoutHeight = expectedHeight;

        // Assert
        Assert.Equal(expectedHeight, control.FlyoutHeight);
    }

    [StaTheory]
    [InlineData("Settings")]
    [InlineData("Edit Profile")]
    [InlineData("Configuration")]
    public void Header_CanBeSet(string expectedHeader)
    {
        // Arrange
        var control = new Flyout();

        // Act
        control.Header = expectedHeader;

        // Assert
        Assert.Equal(expectedHeader, control.Header);
    }

    [StaFact]
    public void IsLightDismissEnabled_CanBeSet()
    {
        // Arrange
        var control = new Flyout();

        // Act
        control.IsLightDismissEnabled = false;

        // Assert
        Assert.False(control.IsLightDismissEnabled);
    }

    [StaFact]
    public void ShowOverlay_CanBeSet()
    {
        // Arrange
        var control = new Flyout();

        // Act
        control.ShowOverlay = false;

        // Assert
        Assert.False(control.ShowOverlay);
    }

    [StaTheory]
    [InlineData(0.0)]
    [InlineData(0.5)]
    [InlineData(1.0)]
    public void OverlayOpacity_CanBeSet(double expectedOpacity)
    {
        // Arrange
        var control = new Flyout();

        // Act
        control.OverlayOpacity = expectedOpacity;

        // Assert
        Assert.Equal(expectedOpacity, control.OverlayOpacity);
    }

    [StaFact]
    public void ShowCloseButton_CanBeSet()
    {
        // Arrange
        var control = new Flyout();

        // Act
        control.ShowCloseButton = false;

        // Assert
        Assert.False(control.ShowCloseButton);
    }

    [StaTheory]
    [InlineData(100.0)]
    [InlineData(300.0)]
    [InlineData(500.0)]
    public void AnimationDuration_CanBeSet(double expectedDuration)
    {
        // Arrange
        var control = new Flyout();

        // Act
        control.AnimationDuration = expectedDuration;

        // Assert
        Assert.Equal(expectedDuration, control.AnimationDuration);
    }

    [StaFact]
    public void CloseOnEscape_CanBeSet()
    {
        // Arrange
        var control = new Flyout();

        // Act
        control.CloseOnEscape = false;

        // Assert
        Assert.False(control.CloseOnEscape);
    }

    [StaFact]
    public void IsPinned_CanBeSet()
    {
        // Arrange
        var control = new Flyout();

        // Act
        control.IsPinned = true;

        // Assert
        Assert.True(control.IsPinned);
    }

    [StaFact]
    public void ShowPinButton_CanBeSet()
    {
        // Arrange
        var control = new Flyout();

        // Act
        control.ShowPinButton = true;

        // Assert
        Assert.True(control.ShowPinButton);
    }

    [StaFact]
    public void IsResizable_CanBeSet()
    {
        // Arrange
        var control = new Flyout();

        // Act
        control.IsResizable = true;

        // Assert
        Assert.True(control.IsResizable);
    }

    [StaFact]
    public void CornerRadius_CanBeSet()
    {
        // Arrange
        var control = new Flyout();
        var expectedCornerRadius = new CornerRadius(8);

        // Act
        control.CornerRadius = expectedCornerRadius;

        // Assert
        Assert.Equal(expectedCornerRadius, control.CornerRadius);
    }

    [StaFact]
    public void FlyoutBorderThickness_CanBeSet()
    {
        // Arrange
        var control = new Flyout();
        var expectedThickness = new Thickness(2);

        // Act
        control.FlyoutBorderThickness = expectedThickness;

        // Assert
        Assert.Equal(expectedThickness, control.FlyoutBorderThickness);
    }

    [StaTheory]
    [InlineData("Success")]
    [InlineData(42)]
    [InlineData(true)]
    public void Result_CanBeSet(object expectedResult)
    {
        // Arrange
        var control = new Flyout();

        // Act
        control.Result = expectedResult;

        // Assert
        Assert.Equal(expectedResult, control.Result);
    }

    [StaFact]
    public void MinFlyoutWidth_CanBeSet()
    {
        // Arrange
        var control = new Flyout();
        const double expectedValue = 250.0;

        // Act
        control.MinFlyoutWidth = expectedValue;

        // Assert
        Assert.Equal(expectedValue, control.MinFlyoutWidth);
    }

    [StaFact]
    public void MaxFlyoutWidth_CanBeSet()
    {
        // Arrange
        var control = new Flyout();
        const double expectedValue = 1000.0;

        // Act
        control.MaxFlyoutWidth = expectedValue;

        // Assert
        Assert.Equal(expectedValue, control.MaxFlyoutWidth);
    }

    [StaFact]
    public void MinFlyoutHeight_CanBeSet()
    {
        // Arrange
        var control = new Flyout();
        const double expectedValue = 200.0;

        // Act
        control.MinFlyoutHeight = expectedValue;

        // Assert
        Assert.Equal(expectedValue, control.MinFlyoutHeight);
    }

    [StaFact]
    public void MaxFlyoutHeight_CanBeSet()
    {
        // Arrange
        var control = new Flyout();
        const double expectedValue = 800.0;

        // Act
        control.MaxFlyoutHeight = expectedValue;

        // Assert
        Assert.Equal(expectedValue, control.MaxFlyoutHeight);
    }

    [StaFact]
    public void IsFocusTrapEnabled_DefaultValue_IsTrue()
    {
        // Arrange & Act
        var control = new Flyout();

        // Assert
        Assert.True(control.IsFocusTrapEnabled);
    }

    [StaFact]
    public void IsFocusTrapEnabled_CanBeSet()
    {
        // Arrange
        var control = new Flyout();

        // Act
        control.IsFocusTrapEnabled = false;

        // Assert
        Assert.False(control.IsFocusTrapEnabled);
    }
}