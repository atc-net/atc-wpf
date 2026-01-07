namespace Atc.Wpf.NetworkControls.Tests;

public sealed class NetworkHostViewModelTests
{
    [Fact]
    public void Constructor_WithIpAddress_ShouldSetIpAddress()
    {
        // Arrange
        var ipAddress = IPAddress.Parse("192.168.1.1");

        // Act
        var sut = new NetworkHostViewModel(ipAddress);

        // Assert
        Assert.Equal(ipAddress, sut.IpAddress);
    }

    [Fact]
    public void OpenPortNumbersAsCommaList_ShouldReturnCommaSeparatedString()
    {
        // Arrange
        var ipAddress = IPAddress.Parse("192.168.1.1");
        var sut = new NetworkHostViewModel(ipAddress)
        {
            OpenPortNumbers = [22, 80, 443],
        };

        // Act
        var result = sut.OpenPortNumbersAsCommaList;

        // Assert
        Assert.Equal("22,80,443", result);
    }

    [Fact]
    public void OpenPortNumbersAsCommaList_WhenEmpty_ShouldReturnEmptyString()
    {
        // Arrange
        var ipAddress = IPAddress.Parse("192.168.1.1");
        var sut = new NetworkHostViewModel(ipAddress)
        {
            OpenPortNumbers = [],
        };

        // Act
        var result = sut.OpenPortNumbersAsCommaList;

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void IpAddressSortable_ShouldReturnZeroPaddedFormat()
    {
        // Arrange
        var ipAddress = IPAddress.Parse("192.168.1.1");
        var sut = new NetworkHostViewModel(ipAddress);

        // Act
        var result = sut.IpAddressSortable;

        // Assert
        Assert.Equal("192.168.001.001", result);
    }

    [Theory]
    [InlineData("192.168.1.1", "192.168.001.001")]
    [InlineData("10.0.0.1", "010.000.000.001")]
    [InlineData("255.255.255.255", "255.255.255.255")]
    [InlineData("1.2.3.4", "001.002.003.004")]
    public void IpAddressSortable_ShouldPadCorrectly(
        string ip,
        string expected)
    {
        // Arrange
        var ipAddress = IPAddress.Parse(ip);
        var sut = new NetworkHostViewModel(ipAddress);

        // Act
        var result = sut.IpAddressSortable;

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Hostname_WhenSet_ShouldRaisePropertyChanged()
    {
        // Arrange
        var ipAddress = IPAddress.Parse("192.168.1.1");
        var sut = new NetworkHostViewModel(ipAddress);
        var propertyChangedRaised = false;
        sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(NetworkHostViewModel.Hostname))
            {
                propertyChangedRaised = true;
            }
        };

        // Act
        sut.Hostname = "test.local";

        // Assert
        Assert.True(propertyChangedRaised);
        Assert.Equal("test.local", sut.Hostname);
    }

    [Fact]
    public void MacAddress_WhenSet_ShouldRaisePropertyChanged()
    {
        // Arrange
        var ipAddress = IPAddress.Parse("192.168.1.1");
        var sut = new NetworkHostViewModel(ipAddress);
        var propertyChangedRaised = false;
        sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(NetworkHostViewModel.MacAddress))
            {
                propertyChangedRaised = true;
            }
        };

        // Act
        sut.MacAddress = "00:11:22:33:44:55";

        // Assert
        Assert.True(propertyChangedRaised);
        Assert.Equal("00:11:22:33:44:55", sut.MacAddress);
    }
}