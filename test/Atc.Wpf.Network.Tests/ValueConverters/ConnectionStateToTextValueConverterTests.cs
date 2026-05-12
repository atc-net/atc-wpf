namespace Atc.Wpf.Network.Tests.ValueConverters;

public sealed class ConnectionStateToTextValueConverterTests
{
    private readonly IValueConverter converter = new ConnectionStateToTextValueConverter();

    [Theory]
    [InlineData(ConnectionState.None)]
    [InlineData(ConnectionState.Connecting)]
    [InlineData(ConnectionState.Connected)]
    [InlineData(ConnectionState.Disconnecting)]
    [InlineData(ConnectionState.Disconnected)]
    [InlineData(ConnectionState.ConnectionFailed)]
    [InlineData(ConnectionState.ReconnectionFailed)]
    [InlineData(ConnectionState.Reconnecting)]
    [InlineData(ConnectionState.Reconnected)]
    [InlineData(ConnectionState.Pulse)]
    public void Convert_KnownStates_ReturnsNonEmptyString(ConnectionState state)
    {
        var actual = converter.Convert(
            value: state,
            targetType: typeof(string),
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        var text = Assert.IsType<string>(actual);
        Assert.False(string.IsNullOrWhiteSpace(text));
    }

    [Fact]
    public void Convert_ReturnsSameTextForSameState()
    {
        var first = converter.Convert(
            ConnectionState.Connected,
            typeof(string),
            parameter: null,
            CultureInfo.InvariantCulture);
        var second = converter.Convert(
            ConnectionState.Connected,
            typeof(string),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(first, second);
    }

    [Fact]
    public void Convert_DifferentStates_ProduceDifferentText()
    {
        var connected = converter.Convert(
            ConnectionState.Connected,
            typeof(string),
            parameter: null,
            CultureInfo.InvariantCulture);
        var disconnected = converter.Convert(
            ConnectionState.Disconnected,
            typeof(string),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.NotEqual(connected, disconnected);
    }

    [Fact]
    public void Convert_Null_ReturnsUnknown()
        => Assert.Equal(
            "Unknown",
            converter.Convert(
                value: null,
                targetType: typeof(string),
                parameter: null,
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void Convert_NonConnectionState_ReturnsUnknown()
        => Assert.Equal(
            "Unknown",
            converter.Convert(
                value: "NotAConnectionState",
                targetType: typeof(string),
                parameter: null,
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void Convert_DifferentEnumType_ReturnsUnknown()
        => Assert.Equal(
            "Unknown",
            converter.Convert(
                value: DayOfWeek.Monday,
                targetType: typeof(string),
                parameter: null,
                culture: CultureInfo.InvariantCulture));

    [Fact]
    public void ConvertBack_Throws_NotSupportedException()
    {
        var exception = Record.Exception(() => converter.ConvertBack(
            value: "Connected",
            targetType: typeof(ConnectionState),
            parameter: null,
            culture: CultureInfo.InvariantCulture));

        Assert.IsType<NotSupportedException>(exception);
    }
}