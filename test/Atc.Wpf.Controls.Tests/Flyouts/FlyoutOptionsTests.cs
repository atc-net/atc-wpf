namespace Atc.Wpf.Controls.Tests.Flyouts;

public sealed class FlyoutOptionsTests
{
    [Fact]
    public void Constructor_SetsDefaultValues()
    {
        // Arrange & Act
        var options = new FlyoutOptions();

        // Assert
        Assert.Equal(FlyoutPosition.Right, options.Position);
        Assert.Equal(400, options.Width);
        Assert.Equal(300, options.Height);
        Assert.True(options.IsLightDismissEnabled);
        Assert.True(options.ShowOverlay);
        Assert.Equal(0.5, options.OverlayOpacity);
        Assert.True(options.ShowCloseButton);
        Assert.True(options.CloseOnEscape);
        Assert.Equal(300, options.AnimationDuration);
        Assert.Equal(new CornerRadius(0), options.CornerRadius);
        Assert.Equal(new Thickness(16), options.Padding);
        Assert.False(options.IsPinned);
        Assert.False(options.IsResizable);
        Assert.Equal(200, options.MinWidth);
        Assert.Equal(800, options.MaxWidth);
        Assert.Equal(150, options.MinHeight);
        Assert.Equal(600, options.MaxHeight);
        Assert.Null(options.EasingFunction);
        Assert.True(options.IsFocusTrapEnabled);
    }

    [Fact]
    public void Default_ReturnsDefaultOptions()
    {
        // Act
        var options = FlyoutOptions.Default;

        // Assert
        Assert.Equal(FlyoutPosition.Right, options.Position);
        Assert.Equal(400, options.Width);
    }

    [Fact]
    public void Wide_ReturnsWideOptions()
    {
        // Act
        var options = FlyoutOptions.Wide;

        // Assert
        Assert.Equal(600, options.Width);
    }

    [Fact]
    public void Narrow_ReturnsNarrowOptions()
    {
        // Act
        var options = FlyoutOptions.Narrow;

        // Assert
        Assert.Equal(300, options.Width);
    }

    [Fact]
    public void Left_ReturnsLeftPositionOptions()
    {
        // Act
        var options = FlyoutOptions.Left;

        // Assert
        Assert.Equal(FlyoutPosition.Left, options.Position);
    }

    [Fact]
    public void Top_ReturnsTopPositionOptions()
    {
        // Act
        var options = FlyoutOptions.Top;

        // Assert
        Assert.Equal(FlyoutPosition.Top, options.Position);
    }

    [Fact]
    public void Bottom_ReturnsBottomPositionOptions()
    {
        // Act
        var options = FlyoutOptions.Bottom;

        // Assert
        Assert.Equal(FlyoutPosition.Bottom, options.Position);
    }

    [Fact]
    public void Center_ReturnsCenterPositionOptions()
    {
        // Act
        var options = FlyoutOptions.Center;

        // Assert
        Assert.Equal(FlyoutPosition.Center, options.Position);
        Assert.Equal(500, options.Width);
        Assert.Equal(400, options.Height);
        Assert.Equal(new CornerRadius(8), options.CornerRadius);
    }

    [Fact]
    public void Modal_ReturnsModalOptions()
    {
        // Act
        var options = FlyoutOptions.Modal;

        // Assert
        Assert.False(options.IsLightDismissEnabled);
        Assert.False(options.CloseOnEscape);
    }

    [Fact]
    public void Resizable_ReturnsResizableOptions()
    {
        // Act
        var options = FlyoutOptions.Resizable;

        // Assert
        Assert.True(options.IsResizable);
    }

    [Fact]
    public void Pinned_ReturnsPinnedOptions()
    {
        // Act
        var options = FlyoutOptions.Pinned;

        // Assert
        Assert.True(options.IsPinned);
    }

    [Theory]
    [InlineData(FlyoutPosition.Right)]
    [InlineData(FlyoutPosition.Left)]
    [InlineData(FlyoutPosition.Top)]
    [InlineData(FlyoutPosition.Bottom)]
    [InlineData(FlyoutPosition.Center)]
    public void Position_CanBeSet(FlyoutPosition expectedPosition)
    {
        // Arrange
        var options = new FlyoutOptions();

        // Act
        options.Position = expectedPosition;

        // Assert
        Assert.Equal(expectedPosition, options.Position);
    }

    [Theory]
    [InlineData(100)]
    [InlineData(400)]
    [InlineData(800)]
    public void Width_CanBeSet(double expectedWidth)
    {
        // Arrange
        var options = new FlyoutOptions();

        // Act
        options.Width = expectedWidth;

        // Assert
        Assert.Equal(expectedWidth, options.Width);
    }

    [Theory]
    [InlineData(100)]
    [InlineData(300)]
    [InlineData(600)]
    public void Height_CanBeSet(double expectedHeight)
    {
        // Arrange
        var options = new FlyoutOptions();

        // Act
        options.Height = expectedHeight;

        // Assert
        Assert.Equal(expectedHeight, options.Height);
    }

    [Fact]
    public void IsLightDismissEnabled_CanBeSet()
    {
        // Arrange
        var options = new FlyoutOptions();

        // Act
        options.IsLightDismissEnabled = false;

        // Assert
        Assert.False(options.IsLightDismissEnabled);
    }

    [Fact]
    public void ShowOverlay_CanBeSet()
    {
        // Arrange
        var options = new FlyoutOptions();

        // Act
        options.ShowOverlay = false;

        // Assert
        Assert.False(options.ShowOverlay);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(0.5)]
    [InlineData(1.0)]
    public void OverlayOpacity_CanBeSet(double expectedOpacity)
    {
        // Arrange
        var options = new FlyoutOptions();

        // Act
        options.OverlayOpacity = expectedOpacity;

        // Assert
        Assert.Equal(expectedOpacity, options.OverlayOpacity);
    }

    [Fact]
    public void ShowCloseButton_CanBeSet()
    {
        // Arrange
        var options = new FlyoutOptions();

        // Act
        options.ShowCloseButton = false;

        // Assert
        Assert.False(options.ShowCloseButton);
    }

    [Fact]
    public void CloseOnEscape_CanBeSet()
    {
        // Arrange
        var options = new FlyoutOptions();

        // Act
        options.CloseOnEscape = false;

        // Assert
        Assert.False(options.CloseOnEscape);
    }

    [Theory]
    [InlineData(100)]
    [InlineData(300)]
    [InlineData(500)]
    public void AnimationDuration_CanBeSet(double expectedDuration)
    {
        // Arrange
        var options = new FlyoutOptions();

        // Act
        options.AnimationDuration = expectedDuration;

        // Assert
        Assert.Equal(expectedDuration, options.AnimationDuration);
    }

    [Fact]
    public void CornerRadius_CanBeSet()
    {
        // Arrange
        var options = new FlyoutOptions();
        var expectedCornerRadius = new CornerRadius(8);

        // Act
        options.CornerRadius = expectedCornerRadius;

        // Assert
        Assert.Equal(expectedCornerRadius, options.CornerRadius);
    }

    [Fact]
    public void Padding_CanBeSet()
    {
        // Arrange
        var options = new FlyoutOptions();
        var expectedPadding = new Thickness(24);

        // Act
        options.Padding = expectedPadding;

        // Assert
        Assert.Equal(expectedPadding, options.Padding);
    }

    [Fact]
    public void IsPinned_CanBeSet()
    {
        // Arrange
        var options = new FlyoutOptions();

        // Act
        options.IsPinned = true;

        // Assert
        Assert.True(options.IsPinned);
    }

    [Fact]
    public void IsResizable_CanBeSet()
    {
        // Arrange
        var options = new FlyoutOptions();

        // Act
        options.IsResizable = true;

        // Assert
        Assert.True(options.IsResizable);
    }

    [Fact]
    public void MinWidth_CanBeSet()
    {
        // Arrange
        var options = new FlyoutOptions();
        const double expectedValue = 250;

        // Act
        options.MinWidth = expectedValue;

        // Assert
        Assert.Equal(expectedValue, options.MinWidth);
    }

    [Fact]
    public void MaxWidth_CanBeSet()
    {
        // Arrange
        var options = new FlyoutOptions();
        const double expectedValue = 1000;

        // Act
        options.MaxWidth = expectedValue;

        // Assert
        Assert.Equal(expectedValue, options.MaxWidth);
    }

    [Fact]
    public void MinHeight_CanBeSet()
    {
        // Arrange
        var options = new FlyoutOptions();
        const double expectedValue = 200;

        // Act
        options.MinHeight = expectedValue;

        // Assert
        Assert.Equal(expectedValue, options.MinHeight);
    }

    [Fact]
    public void MaxHeight_CanBeSet()
    {
        // Arrange
        var options = new FlyoutOptions();
        const double expectedValue = 800;

        // Act
        options.MaxHeight = expectedValue;

        // Assert
        Assert.Equal(expectedValue, options.MaxHeight);
    }

    [Fact]
    public void IsFocusTrapEnabled_DefaultValue_IsTrue()
    {
        // Arrange & Act
        var options = new FlyoutOptions();

        // Assert
        Assert.True(options.IsFocusTrapEnabled);
    }

    [Fact]
    public void IsFocusTrapEnabled_CanBeSet()
    {
        // Arrange
        var options = new FlyoutOptions();

        // Act
        options.IsFocusTrapEnabled = false;

        // Assert
        Assert.False(options.IsFocusTrapEnabled);
    }
}