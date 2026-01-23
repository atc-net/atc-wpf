namespace Atc.Wpf.Controls.Tests.Layouts;

[SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK for test methods.")]
public sealed class DockPanelProTests
{
    [StaFact]
    public void Constructor_SetsDefaultValues()
    {
        // Arrange & Act
        var dockPanel = new DockPanelPro();

        // Assert
        Assert.Null(dockPanel.LayoutId);
        Assert.False(dockPanel.AutoSaveLayout);
        Assert.Equal(5.0, dockPanel.SplitterThickness);
        Assert.False(dockPanel.AllowFloating);
    }

    [StaFact]
    public void Dock_AttachedProperty_DefaultIsCenter()
    {
        // Arrange
        var region = new DockRegion();

        // Act
        var dock = DockPanelPro.GetDock(region);

        // Assert
        Assert.Equal(DockPosition.Center, dock);
    }

    [StaFact]
    public void Dock_AttachedProperty_CanBeSet()
    {
        // Arrange
        var region = new DockRegion();

        // Act
        DockPanelPro.SetDock(region, DockPosition.Left);
        var dock = DockPanelPro.GetDock(region);

        // Assert
        Assert.Equal(DockPosition.Left, dock);
    }

    [StaFact]
    public void SaveLayout_EmptyPanel_ReturnsValidJson()
    {
        // Arrange
        var dockPanel = new DockPanelPro
        {
            LayoutId = "TestLayout",
        };

        // Act
        var json = dockPanel.SaveLayout();

        // Assert
        Assert.NotNull(json);
        Assert.Contains("layoutId", json, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("TestLayout", json, StringComparison.Ordinal);
    }

    [StaFact]
    public void LoadLayout_ValidJson_UpdatesRegions()
    {
        // Arrange
        var dockPanel = new DockPanelPro();
        var region = new DockRegion
        {
            RegionId = "region1",
            Width = 100,
            Height = 100,
        };
        DockPanelPro.SetDock(region, DockPosition.Left);
        dockPanel.Children.Add(region);

        var json = """
            {
                "layoutId": "TestLayout",
                "regions": [
                    {
                        "regionId": "region1",
                        "dock": 0,
                        "width": 250,
                        "height": 150,
                        "isExpanded": false
                    }
                ]
            }
            """;

        // Act
        dockPanel.LoadLayout(json);

        // Assert
        Assert.Equal(250, region.Width);
        Assert.Equal(150, region.Height);
        Assert.False(region.IsExpanded);
    }

    [StaFact]
    public void LoadLayout_InvalidJson_DoesNotThrow()
    {
        // Arrange
        var dockPanel = new DockPanelPro();

        // Act & Assert
        var exception = Record.Exception(() => dockPanel.LoadLayout("invalid json"));
        Assert.Null(exception);
    }

    [StaFact]
    public void LoadLayout_NullOrEmpty_DoesNotThrow()
    {
        // Arrange
        var dockPanel = new DockPanelPro();

        // Act & Assert
        var exception1 = Record.Exception(() => dockPanel.LoadLayout(null!));
        var exception2 = Record.Exception(() => dockPanel.LoadLayout(string.Empty));

        Assert.Null(exception1);
        Assert.Null(exception2);
    }

    [StaFact]
    public void ResetLayout_ResetsRegionSizes()
    {
        // Arrange
        var dockPanel = new DockPanelPro();
        var region = new DockRegion
        {
            Width = 300,
            Height = 200,
            IsExpanded = false,
        };
        dockPanel.Children.Add(region);

        // Act
        dockPanel.ResetLayout();

        // Assert
        Assert.True(double.IsNaN(region.Width));
        Assert.True(double.IsNaN(region.Height));
        Assert.True(region.IsExpanded);
    }

    [StaFact]
    public void SplitterThickness_CanBeChanged()
    {
        // Arrange
        var dockPanel = new DockPanelPro();

        // Act
        dockPanel.SplitterThickness = 10;

        // Assert
        Assert.Equal(10, dockPanel.SplitterThickness);
    }

    [StaFact]
    public void SplitterBackground_CanBeSet()
    {
        // Arrange
        var dockPanel = new DockPanelPro();
        var brush = Brushes.Red;

        // Act
        dockPanel.SplitterBackground = brush;

        // Assert
        Assert.Equal(brush, dockPanel.SplitterBackground);
    }
}