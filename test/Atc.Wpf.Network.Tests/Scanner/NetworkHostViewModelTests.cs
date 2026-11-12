namespace Atc.Wpf.Network.Tests.Scanner;

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

    [Fact]
    public void OpenPortNumbers_WhenSet_ShouldRaisePropertyChangedForCommaList()
    {
        var sut = new NetworkHostViewModel(IPAddress.Parse("192.168.1.1"));
        var raised = new List<string?>();
        sut.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

        sut.OpenPortNumbers = [80];

        Assert.Contains(raised, x => string.Equals(x, nameof(NetworkHostViewModel.OpenPortNumbersAsCommaList), StringComparison.Ordinal));
    }

    [Fact]
    public void Constructor_WithScanResult_ShouldKeepASnapshotOfTheOpenPorts()
    {
        var result = new IPScanResult(IPAddress.Parse("192.168.1.1"));
        result.Ports.Add(OpenTcpPort(80));

        var sut = new NetworkHostViewModel(result);
        result.Ports.Add(OpenTcpPort(22));

        Assert.Equal([80], sut.OpenPortNumbers);
    }

    [Fact]
    public void Update_WithScanResult_ShouldCopyTheValues()
    {
        var ipAddress = IPAddress.Parse("192.168.1.1");
        var sut = new NetworkHostViewModel(ipAddress);
        var result = new IPScanResult(ipAddress)
        {
            PingStatus = new PingStatusResult(ipAddress, IPStatus.Success, pingInMs: 5),
            Hostname = "nas.local",
            MacAddress = "00-11-22-33-44-55",
            MacVendor = "Vendor",
        };
        result.Ports.Add(OpenTcpPort(443));
        result.Ports.Add(OpenTcpPort(22));

        sut.Update(result);

        Assert.Equal(IPStatus.Success, sut.PingStatus?.Status);
        Assert.Equal("nas.local", sut.Hostname);
        Assert.Equal("00-11-22-33-44-55", sut.MacAddress);
        Assert.Equal("Vendor", sut.MacVendor);
        Assert.Equal([22, 443], sut.OpenPortNumbers);
        Assert.Equal("22,443", sut.OpenPortNumbersAsCommaList);
        Assert.NotNull(sut.PingQualityCategoryToolTip);
    }

    [Fact]
    public void Update_WithoutPingStatus_ShouldSetUnknownStatusAndNoToolTip()
    {
        var ipAddress = IPAddress.Parse("192.168.1.1");
        var sut = new NetworkHostViewModel(ipAddress);

        sut.Update(new IPScanResult(ipAddress));

        Assert.Equal(IPStatus.Unknown, sut.PingStatus?.Status);
        Assert.Null(sut.PingQualityCategoryToolTip);
    }

    [Fact]
    public void Update_WithPingStatus_ShouldLocalizeTheToolTipInTheUiCulture()
    {
        var ipAddress = IPAddress.Parse("192.168.1.1");
        var sut = new NetworkHostViewModel(ipAddress);
        var result = new IPScanResult(ipAddress)
        {
            PingStatus = new PingStatusResult(ipAddress, IPStatus.Success, pingInMs: 20),
        };

        var previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = new CultureInfo("da-DK");
            sut.Update(result);
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }

        Assert.Equal("Fremragende - 20ms", sut.PingQualityCategoryToolTip);
    }

    private static IPScanPortResult OpenTcpPort(ushort port)
        => new()
        {
            Port = port,
            TransportProtocol = TransportProtocolType.Tcp,
            CanConnect = true,
        };
}