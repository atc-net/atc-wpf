namespace Atc.Wpf.Controls.Tests.Flyouts;

public sealed class FlyoutPresenterTests
{
    [StaFact]
    public void Constructor_SetsDefaultValues()
    {
        // Arrange & Act
        var control = new FlyoutPresenter();

        // Assert
        Assert.Null(control.Header);
        Assert.Null(control.HeaderTemplate);
        Assert.Null(control.HeaderBackground);
        Assert.Null(control.HeaderForeground);
        Assert.Equal(new Thickness(16, 12, 16, 12), control.HeaderPadding);
        Assert.Null(control.Footer);
        Assert.Null(control.FooterTemplate);
        Assert.Null(control.FooterBackground);
        Assert.Equal(new Thickness(16, 12, 16, 12), control.FooterPadding);
        Assert.True(control.ShowHeader);
        Assert.True(control.ShowFooter);
        Assert.True(control.IsContentScrollable);
        Assert.Equal(ScrollBarVisibility.Auto, control.VerticalScrollBarVisibility);
        Assert.Equal(ScrollBarVisibility.Disabled, control.HorizontalScrollBarVisibility);
        Assert.Equal(new Thickness(16), control.ContentPadding);
        Assert.True(control.ShowHeaderSeparator);
        Assert.True(control.ShowFooterSeparator);
        Assert.Equal(1.0, control.SeparatorThickness);
    }

    [StaTheory]
    [InlineData("Settings")]
    [InlineData("Edit Profile")]
    [InlineData("Configuration")]
    public void Header_CanBeSet(string expectedHeader)
    {
        // Arrange
        var control = new FlyoutPresenter();

        // Act
        control.Header = expectedHeader;

        // Assert
        Assert.Equal(expectedHeader, control.Header);
    }

    [StaTheory]
    [InlineData("Save")]
    [InlineData("Cancel")]
    [InlineData("Apply")]
    public void Footer_CanBeSet(string expectedFooter)
    {
        // Arrange
        var control = new FlyoutPresenter();

        // Act
        control.Footer = expectedFooter;

        // Assert
        Assert.Equal(expectedFooter, control.Footer);
    }

    [StaFact]
    public void ShowHeader_CanBeSet()
    {
        // Arrange
        var control = new FlyoutPresenter();

        // Act
        control.ShowHeader = false;

        // Assert
        Assert.False(control.ShowHeader);
    }

    [StaFact]
    public void ShowFooter_CanBeSet()
    {
        // Arrange
        var control = new FlyoutPresenter();

        // Act
        control.ShowFooter = false;

        // Assert
        Assert.False(control.ShowFooter);
    }

    [StaFact]
    public void IsContentScrollable_CanBeSet()
    {
        // Arrange
        var control = new FlyoutPresenter();

        // Act
        control.IsContentScrollable = false;

        // Assert
        Assert.False(control.IsContentScrollable);
    }

    [StaTheory]
    [InlineData(ScrollBarVisibility.Auto)]
    [InlineData(ScrollBarVisibility.Disabled)]
    [InlineData(ScrollBarVisibility.Hidden)]
    [InlineData(ScrollBarVisibility.Visible)]
    public void VerticalScrollBarVisibility_CanBeSet(
        ScrollBarVisibility expected)
    {
        // Arrange
        var control = new FlyoutPresenter();

        // Act
        control.VerticalScrollBarVisibility = expected;

        // Assert
        Assert.Equal(expected, control.VerticalScrollBarVisibility);
    }

    [StaTheory]
    [InlineData(ScrollBarVisibility.Auto)]
    [InlineData(ScrollBarVisibility.Disabled)]
    [InlineData(ScrollBarVisibility.Hidden)]
    [InlineData(ScrollBarVisibility.Visible)]
    public void HorizontalScrollBarVisibility_CanBeSet(
        ScrollBarVisibility expected)
    {
        // Arrange
        var control = new FlyoutPresenter();

        // Act
        control.HorizontalScrollBarVisibility = expected;

        // Assert
        Assert.Equal(expected, control.HorizontalScrollBarVisibility);
    }

    [StaFact]
    public void ContentPadding_CanBeSet()
    {
        // Arrange
        var control = new FlyoutPresenter();
        var expectedPadding = new Thickness(24);

        // Act
        control.ContentPadding = expectedPadding;

        // Assert
        Assert.Equal(expectedPadding, control.ContentPadding);
    }

    [StaFact]
    public void HeaderPadding_CanBeSet()
    {
        // Arrange
        var control = new FlyoutPresenter();
        var expectedPadding = new Thickness(20, 16, 20, 16);

        // Act
        control.HeaderPadding = expectedPadding;

        // Assert
        Assert.Equal(expectedPadding, control.HeaderPadding);
    }

    [StaFact]
    public void FooterPadding_CanBeSet()
    {
        // Arrange
        var control = new FlyoutPresenter();
        var expectedPadding = new Thickness(20, 16, 20, 16);

        // Act
        control.FooterPadding = expectedPadding;

        // Assert
        Assert.Equal(expectedPadding, control.FooterPadding);
    }

    [StaFact]
    public void CornerRadius_CanBeSet()
    {
        // Arrange
        var control = new FlyoutPresenter();
        var expectedCornerRadius = new CornerRadius(8);

        // Act
        control.CornerRadius = expectedCornerRadius;

        // Assert
        Assert.Equal(expectedCornerRadius, control.CornerRadius);
    }

    [StaFact]
    public void ShowHeaderSeparator_CanBeSet()
    {
        // Arrange
        var control = new FlyoutPresenter();

        // Act
        control.ShowHeaderSeparator = false;

        // Assert
        Assert.False(control.ShowHeaderSeparator);
    }

    [StaFact]
    public void ShowFooterSeparator_CanBeSet()
    {
        // Arrange
        var control = new FlyoutPresenter();

        // Act
        control.ShowFooterSeparator = false;

        // Assert
        Assert.False(control.ShowFooterSeparator);
    }

    [StaTheory]
    [InlineData(0.5)]
    [InlineData(1.0)]
    [InlineData(2.0)]
    public void SeparatorThickness_CanBeSet(double expectedThickness)
    {
        // Arrange
        var control = new FlyoutPresenter();

        // Act
        control.SeparatorThickness = expectedThickness;

        // Assert
        Assert.Equal(expectedThickness, control.SeparatorThickness);
    }
}