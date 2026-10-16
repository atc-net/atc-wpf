namespace Atc.Wpf.Network.Tests.Scanner;

[SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "Test cleanup handled by xUnit.")]
public sealed class NetworkScannerViewModelTests
{
    private readonly NetworkScannerViewModel sut;

    public NetworkScannerViewModelTests()
    {
        sut = new NetworkScannerViewModel();
    }

    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        // Assert
        Assert.NotNull(sut.Entries);
        Assert.NotNull(sut.Columns);
        Assert.NotNull(sut.Filter);
        Assert.Equal("0 / 0", sut.EntryCountInfo);
        Assert.Empty(sut.PortsNumbers);
        Assert.Null(sut.SelectedEntry);
        Assert.Null(sut.StartIpAddress);
        Assert.Null(sut.EndIpAddress);
        Assert.False(sut.IsBusy);
        Assert.False(sut.HasError);
        Assert.Null(sut.ErrorMessage);
    }

    [Theory]
    [InlineData("22, 80, 443", new ushort[] { 22, 80, 443 })]
    [InlineData("22,80,443", new ushort[] { 22, 80, 443 })]
    [InlineData("22;80;443", new ushort[] { 22, 80, 443 })]
    [InlineData("22 80 443", new ushort[] { 22, 80, 443 })]
    [InlineData("22, 80, invalid, 443", new ushort[] { 22, 80, 443 })]
    [InlineData("", new ushort[] { })]
    [InlineData("   ", new ushort[] { })]
    [InlineData("invalid", new ushort[] { })]
    public void PortsNumbersText_ShouldParseCorrectly(
        string input,
        ushort[] expected)
    {
        // Act
        sut.PortsNumbersText = input;

        // Assert
        Assert.Equal(expected, sut.PortsNumbers);
    }

    [Fact]
    public void PortsNumbersText_Get_ShouldReturnCommaSeparatedString()
    {
        // Arrange
        sut.PortsNumbers = [22, 80, 443];

        // Act
        var result = sut.PortsNumbersText;

        // Assert
        Assert.Equal("22, 80, 443", result);
    }

    [Fact]
    public void HasError_ShouldReturnTrue_WhenErrorMessageIsSet()
    {
        // Act
        sut.ErrorMessage = "Test error message";

        // Assert
        Assert.True(sut.HasError);
    }

    [Fact]
    public void HasError_ShouldReturnFalse_WhenErrorMessageIsNull()
    {
        // Act
        sut.ErrorMessage = null;

        // Assert
        Assert.False(sut.HasError);
    }

    [Fact]
    public void HasError_ShouldReturnFalse_WhenErrorMessageIsEmpty()
    {
        // Act
        sut.ErrorMessage = string.Empty;

        // Assert
        Assert.False(sut.HasError);
    }

    [Fact]
    public void Dispose_ShouldNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() => sut.Dispose());
        Assert.Null(exception);
    }

    [Fact]
    public void Dispose_ShouldBeIdempotent()
    {
        // Act & Assert
        var exception = Record.Exception(() =>
        {
            sut.Dispose();
            sut.Dispose();
        });
        Assert.Null(exception);
    }

    [Theory]
    [InlineData("192.168.1.10", "192.168.1.50", true)]
    [InlineData("192.168.1.10", "192.168.1.10", true)]
    [InlineData("10.0.0.1", "10.0.1.0", true)]
    [InlineData("192.168.1.50", "192.168.1.10", false)]
    [InlineData("::1", "::2", false)]
    [InlineData("not-an-ip", "192.168.1.1", false)]
    [InlineData(null, "192.168.1.1", false)]
    [InlineData("192.168.1.1", null, false)]
    public void IsValidIpv4Range_AcceptsOnlyOrderedIpv4Ranges(
        string? start,
        string? end,
        bool expected)
        => Assert.Equal(expected, NetworkScannerViewModel.IsValidIpv4Range(start, end));

    [Fact]
    public void ScanCommand_WithAReversedRange_CannotExecute()
    {
        sut.StartIpAddress = "192.168.1.50";
        sut.EndIpAddress = "192.168.1.10";

        Assert.False(sut.ScanCommand.CanExecute(null));
    }

    [Fact]
    public void ScanCommand_WithAnOrderedRange_CanExecute()
    {
        sut.StartIpAddress = "192.168.1.10";
        sut.EndIpAddress = "192.168.1.50";

        Assert.True(sut.ScanCommand.CanExecute(null));
    }
}