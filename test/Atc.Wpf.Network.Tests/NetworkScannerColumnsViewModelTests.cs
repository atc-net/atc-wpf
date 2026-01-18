namespace Atc.Wpf.Network.Tests;

public sealed class NetworkScannerColumnsViewModelTests
{
    private readonly NetworkScannerColumnsViewModel sut;

    public NetworkScannerColumnsViewModelTests()
    {
        sut = new NetworkScannerColumnsViewModel();
    }

    [Fact]
    public void Constructor_ShouldInitializeWithAllColumnsVisible()
    {
        // Assert
        Assert.True(sut.ShowPingQualityCategory);
        Assert.True(sut.ShowIpAddress);
        Assert.True(sut.ShowIpStatus);
        Assert.True(sut.ShowHostname);
        Assert.True(sut.ShowMacAddress);
        Assert.True(sut.ShowMacVendor);
        Assert.True(sut.ShowOpenPortNumbers);
        Assert.True(sut.ShowTotalInMs);
    }

    [Fact]
    public void ShowIpAddress_WhenChanged_ShouldRaisePropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(NetworkScannerColumnsViewModel.ShowIpAddress))
            {
                propertyChangedRaised = true;
            }
        };

        // Act
        sut.ShowIpAddress = false;

        // Assert
        Assert.True(propertyChangedRaised);
        Assert.False(sut.ShowIpAddress);
    }

    [Fact]
    public void ShowHostname_WhenChanged_ShouldRaisePropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(NetworkScannerColumnsViewModel.ShowHostname))
            {
                propertyChangedRaised = true;
            }
        };

        // Act
        sut.ShowHostname = false;

        // Assert
        Assert.True(propertyChangedRaised);
        Assert.False(sut.ShowHostname);
    }
}