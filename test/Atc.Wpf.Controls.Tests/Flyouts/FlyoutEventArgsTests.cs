namespace Atc.Wpf.Controls.Tests.Flyouts;

public sealed class FlyoutEventArgsTests
{
    [Fact]
    public void FlyoutOpeningEventArgs_DefaultConstructor_SetsDefaults()
    {
        // Arrange & Act
        var args = new FlyoutOpeningEventArgs();

        // Assert
        Assert.False(args.Cancel);
    }

    [Fact]
    public void FlyoutOpeningEventArgs_Cancel_CanBeSet()
    {
        // Arrange
        var args = new FlyoutOpeningEventArgs();

        // Act
        args.Cancel = true;

        // Assert
        Assert.True(args.Cancel);
    }

    [StaFact]
    public void FlyoutOpeningEventArgs_WithRoutedEvent_SetsRoutedEvent()
    {
        // Arrange
        var routedEvent = Flyout.OpeningEvent;

        // Act
        var args = new FlyoutOpeningEventArgs(routedEvent);

        // Assert
        Assert.Equal(routedEvent, args.RoutedEvent);
    }

    [StaFact]
    public void FlyoutOpeningEventArgs_WithRoutedEventAndSource_SetsProperties()
    {
        // Arrange
        var routedEvent = Flyout.OpeningEvent;
        var source = new Flyout();

        // Act
        var args = new FlyoutOpeningEventArgs(routedEvent, source);

        // Assert
        Assert.Equal(routedEvent, args.RoutedEvent);
        Assert.Equal(source, args.Source);
    }

    [Fact]
    public void FlyoutClosingEventArgs_DefaultConstructor_SetsDefaults()
    {
        // Arrange & Act
        var args = new FlyoutClosingEventArgs();

        // Assert
        Assert.False(args.Cancel);
        Assert.False(args.IsLightDismiss);
    }

    [Fact]
    public void FlyoutClosingEventArgs_Cancel_CanBeSet()
    {
        // Arrange
        var args = new FlyoutClosingEventArgs();

        // Act
        args.Cancel = true;

        // Assert
        Assert.True(args.Cancel);
    }

    [Fact]
    public void FlyoutClosingEventArgs_IsLightDismiss_CanBeSet()
    {
        // Arrange
        var args = new FlyoutClosingEventArgs();

        // Act
        args.IsLightDismiss = true;

        // Assert
        Assert.True(args.IsLightDismiss);
    }

    [StaFact]
    public void FlyoutClosingEventArgs_WithRoutedEvent_SetsRoutedEvent()
    {
        // Arrange
        var routedEvent = Flyout.ClosingEvent;

        // Act
        var args = new FlyoutClosingEventArgs(routedEvent);

        // Assert
        Assert.Equal(routedEvent, args.RoutedEvent);
    }

    [StaFact]
    public void FlyoutClosingEventArgs_WithRoutedEventAndSource_SetsProperties()
    {
        // Arrange
        var routedEvent = Flyout.ClosingEvent;
        var source = new Flyout();

        // Act
        var args = new FlyoutClosingEventArgs(routedEvent, source);

        // Assert
        Assert.Equal(routedEvent, args.RoutedEvent);
        Assert.Equal(source, args.Source);
    }
}