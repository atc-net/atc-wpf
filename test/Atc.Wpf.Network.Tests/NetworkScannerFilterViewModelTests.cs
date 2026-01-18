namespace Atc.Wpf.Network.Tests;

public sealed class NetworkScannerFilterViewModelTests
{
    private readonly NetworkScannerFilterViewModel sut;

    public NetworkScannerFilterViewModelTests()
    {
        sut = new NetworkScannerFilterViewModel();
    }

    [Fact]
    public void Constructor_ShouldInitializeWithFiltersEnabled()
    {
        // Assert - both filters default to true
        Assert.True(sut.ShowOnlySuccess);
        Assert.True(sut.ShowOnlyWithOpenPorts);
    }

    [Fact]
    public void ShowOnlySuccess_WhenChanged_ShouldRaisePropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(NetworkScannerFilterViewModel.ShowOnlySuccess))
            {
                propertyChangedRaised = true;
            }
        };

        // Act - change from default true to false
        sut.ShowOnlySuccess = false;

        // Assert
        Assert.True(propertyChangedRaised);
        Assert.False(sut.ShowOnlySuccess);
    }

    [Fact]
    public void ShowOnlyWithOpenPorts_WhenChanged_ShouldRaisePropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(NetworkScannerFilterViewModel.ShowOnlyWithOpenPorts))
            {
                propertyChangedRaised = true;
            }
        };

        // Act - change from default true to false
        sut.ShowOnlyWithOpenPorts = false;

        // Assert
        Assert.True(propertyChangedRaised);
        Assert.False(sut.ShowOnlyWithOpenPorts);
    }
}