namespace Atc.Wpf.Controls.Tests.Layouts;

public sealed class DockRegionTests
{
    [StaFact]
    public void Constructor_SetsDefaultValues()
    {
        // Arrange & Act
        var region = new DockRegion();

        // Assert
        Assert.Null(region.RegionId);
        Assert.False(region.IsCollapsible);
        Assert.True(region.IsExpanded);
        Assert.True(region.IsResizable);
        Assert.Equal(new Thickness(8), region.HeaderPadding);
        Assert.Equal(new Thickness(0), region.ContentPadding);
        Assert.Equal(new CornerRadius(0), region.CornerRadius);
    }

    [StaFact]
    public void IsExpanded_Changed_RaisesExpandedEvent()
    {
        // Arrange
        var region = new DockRegion { IsCollapsible = true };
        var eventRaised = false;
        region.Expanded += (s, e) => eventRaised = true;
        region.IsExpanded = false;

        // Act
        region.IsExpanded = true;

        // Assert
        Assert.True(eventRaised);
    }

    [StaFact]
    public void IsExpanded_Changed_RaisesCollapsedEvent()
    {
        // Arrange
        var region = new DockRegion { IsCollapsible = true };
        var eventRaised = false;
        region.Collapsed += (s, e) => eventRaised = true;

        // Act
        region.IsExpanded = false;

        // Assert
        Assert.True(eventRaised);
    }

    [StaFact]
    public void ToggleExpanded_WhenCollapsible_TogglesState()
    {
        // Arrange
        var region = new DockRegion
        {
            IsCollapsible = true,
            IsExpanded = true,
        };

        // Act
        region.ToggleExpanded();

        // Assert
        Assert.False(region.IsExpanded);

        // Act again
        region.ToggleExpanded();

        // Assert
        Assert.True(region.IsExpanded);
    }

    [StaFact]
    public void ToggleExpanded_WhenNotCollapsible_DoesNotToggleState()
    {
        // Arrange
        var region = new DockRegion
        {
            IsCollapsible = false,
            IsExpanded = true,
        };

        // Act
        region.ToggleExpanded();

        // Assert
        Assert.True(region.IsExpanded);
    }

    [StaFact]
    public void Header_CanBeSet()
    {
        // Arrange
        var region = new DockRegion();

        // Act
        region.Header = "Test Header";

        // Assert
        Assert.Equal("Test Header", region.Header);
    }

    [StaFact]
    public void HeaderBackground_CanBeSet()
    {
        // Arrange
        var region = new DockRegion();
        var brush = Brushes.Blue;

        // Act
        region.HeaderBackground = brush;

        // Assert
        Assert.Equal(brush, region.HeaderBackground);
    }

    [StaFact]
    public void HeaderForeground_CanBeSet()
    {
        // Arrange
        var region = new DockRegion();
        var brush = Brushes.White;

        // Act
        region.HeaderForeground = brush;

        // Assert
        Assert.Equal(brush, region.HeaderForeground);
    }

    [StaFact]
    public void RegionId_CanBeSet()
    {
        // Arrange
        var region = new DockRegion();

        // Act
        region.RegionId = "TestRegion";

        // Assert
        Assert.Equal("TestRegion", region.RegionId);
    }

    [StaFact]
    public void ContentPadding_CanBeSet()
    {
        // Arrange
        var region = new DockRegion();
        var padding = new Thickness(10, 20, 10, 20);

        // Act
        region.ContentPadding = padding;

        // Assert
        Assert.Equal(padding, region.ContentPadding);
    }

    [StaFact]
    public void CornerRadius_CanBeSet()
    {
        // Arrange
        var region = new DockRegion();
        var cornerRadius = new CornerRadius(8);

        // Act
        region.CornerRadius = cornerRadius;

        // Assert
        Assert.Equal(cornerRadius, region.CornerRadius);
    }

    [SuppressMessage("", "CA1030: Consider making 'RaiseRegionSizeChanged_RaisesEvent' an event", Justification = "OK.")]
    [StaFact]
    public void RaiseRegionSizeChanged_RaisesEvent()
    {
        // Arrange
        var region = new DockRegion();
        var eventRaised = false;
        region.RegionSizeChanged += (s, e) => eventRaised = true;

        // Act
        region.RaiseRegionSizeChanged();

        // Assert
        Assert.True(eventRaised);
    }
}