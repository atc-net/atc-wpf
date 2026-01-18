namespace Atc.Wpf.Network.Tests;

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
}