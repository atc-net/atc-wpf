namespace Atc.Wpf.Components.Tests.Flyouts;

public sealed class FlyoutHostTests
{
    [StaFact]
    public void Constructor_SetsDefaultValues()
    {
        // Arrange & Act
        var control = new FlyoutHost();

        // Assert
        Assert.Equal(5, control.MaxNestingDepth);
        Assert.Equal(0, control.OpenFlyoutCount);
        Assert.False(control.IsAnyFlyoutOpen);
        Assert.Empty(control.OpenFlyouts);
        Assert.Null(control.TopFlyout);
    }

    [StaTheory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void MaxNestingDepth_CanBeSet(int expectedDepth)
    {
        // Arrange
        var control = new FlyoutHost();

        // Act
        control.MaxNestingDepth = expectedDepth;

        // Assert
        Assert.Equal(expectedDepth, control.MaxNestingDepth);
    }

    [StaFact]
    public void OpenFlyout_AddsFlyoutToStack()
    {
        // Arrange
        var host = new FlyoutHost();
        var flyout = new Flyout();

        // Act
        var result = host.OpenFlyout(flyout);

        // Assert
        Assert.True(result);
        Assert.Equal(1, host.OpenFlyoutCount);
        Assert.True(host.IsAnyFlyoutOpen);
        Assert.Single(host.OpenFlyouts);
        Assert.Equal(flyout, host.TopFlyout);
        Assert.True(flyout.IsOpen);
    }

    [StaFact]
    public void OpenFlyout_DoesNotOpenDuplicate()
    {
        // Arrange
        var host = new FlyoutHost();
        var flyout = new Flyout();
        host.OpenFlyout(flyout);

        // Act
        var result = host.OpenFlyout(flyout);

        // Assert
        Assert.False(result);
        Assert.Equal(1, host.OpenFlyoutCount);
    }

    [StaFact]
    public void OpenFlyout_RespectsMaxNestingDepth()
    {
        // Arrange
        var host = new FlyoutHost { MaxNestingDepth = 2 };
        var flyout1 = new Flyout();
        var flyout2 = new Flyout();
        var flyout3 = new Flyout();

        host.OpenFlyout(flyout1);
        host.OpenFlyout(flyout2);

        // Act
        var result = host.OpenFlyout(flyout3);

        // Assert
        Assert.False(result);
        Assert.Equal(2, host.OpenFlyoutCount);
    }

    [StaFact]
    public void CloseTopFlyout_ClosesTopFlyout()
    {
        // Arrange
        var host = new FlyoutHost();
        var flyout1 = new Flyout();
        var flyout2 = new Flyout();

        host.OpenFlyout(flyout1);
        host.OpenFlyout(flyout2);

        // Act
        var result = host.CloseTopFlyout();

        // Assert
        Assert.True(result);
        Assert.False(flyout2.IsOpen);
    }

    [StaFact]
    public void CloseTopFlyout_ReturnsFalse_WhenNoFlyoutsOpen()
    {
        // Arrange
        var host = new FlyoutHost();

        // Act
        var result = host.CloseTopFlyout();

        // Assert
        Assert.False(result);
    }

    [StaFact]
    public void CloseAllFlyouts_ClosesAllFlyouts()
    {
        // Arrange
        var host = new FlyoutHost();
        var flyout1 = new Flyout();
        var flyout2 = new Flyout();
        var flyout3 = new Flyout();

        host.OpenFlyout(flyout1);
        host.OpenFlyout(flyout2);
        host.OpenFlyout(flyout3);

        // Act
        host.CloseAllFlyouts();

        // Assert
        Assert.False(flyout1.IsOpen);
        Assert.False(flyout2.IsOpen);
        Assert.False(flyout3.IsOpen);
    }

    [StaFact]
    public void TopFlyout_ReturnsNull_WhenNoFlyoutsOpen()
    {
        // Arrange
        var host = new FlyoutHost();

        // Act & Assert
        Assert.Null(host.TopFlyout);
    }

    [StaFact]
    public void TopFlyout_ReturnsMostRecentlyOpenedFlyout()
    {
        // Arrange
        var host = new FlyoutHost();
        var flyout1 = new Flyout();
        var flyout2 = new Flyout();

        host.OpenFlyout(flyout1);
        host.OpenFlyout(flyout2);

        // Act & Assert
        Assert.Equal(flyout2, host.TopFlyout);
    }
}