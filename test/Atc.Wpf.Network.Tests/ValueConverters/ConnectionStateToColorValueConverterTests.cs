namespace Atc.Wpf.Network.Tests.ValueConverters;

[Collection("Sequential")]
public sealed class ConnectionStateToColorValueConverterTests
{
    private readonly IValueConverter converter = new ConnectionStateToColorValueConverter();

    [Fact]
    public void Instance_Is_Not_Null()
        => Assert.NotNull(ConnectionStateToColorValueConverter.Instance);

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
    public void Convert_KnownStates_ReturnsColor(ConnectionState state)
    {
        var actual = converter.Convert(
            value: state,
            targetType: typeof(Color),
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        var color = Assert.IsType<Color>(actual);
        Assert.NotEqual(default, color);
    }

    [Theory]
    [InlineData(ConnectionState.None, ConnectionState.Connecting)]
    [InlineData(ConnectionState.Connected, ConnectionState.Disconnected)]
    [InlineData(ConnectionState.Connecting, ConnectionState.Reconnecting)]
    [InlineData(ConnectionState.ConnectionFailed, ConnectionState.ReconnectionFailed)]
    [InlineData(ConnectionState.Disconnecting, ConnectionState.Connecting)]
    public void Convert_DistinctStates_ProduceDistinctColors(
        ConnectionState a,
        ConnectionState b)
    {
        var colorA = (Color)converter.Convert(a, typeof(Color), null!, CultureInfo.InvariantCulture);
        var colorB = (Color)converter.Convert(b, typeof(Color), null!, CultureInfo.InvariantCulture);

        Assert.NotEqual(colorA, colorB);
    }

    [Fact]
    public void Convert_Null_ReturnsNeutralColor()
    {
        var actual = converter.Convert(
            value: null,
            targetType: typeof(Color),
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        Assert.IsType<Color>(actual);
    }

    [Fact]
    public void Convert_NonConnectionState_ReturnsNeutralColor()
    {
        var actual = converter.Convert(
            value: "NotAState",
            targetType: typeof(Color),
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        Assert.IsType<Color>(actual);
    }

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
    public void Convert_ColorMatchesBrushConverterPalette(ConnectionState state)
    {
        // The color converter is the source of truth — the brush converter must wrap the same color.
        var color = ConnectionStateToColorValueConverter.GetColor(state);
        var brush = ConnectionStateToBrushValueConverter.GetBrush(state);

        Assert.Equal(color, brush.Color);
    }

    [Fact]
    public void ConvertBack_Throws_NotSupportedException()
    {
        var exception = Record.Exception(() => converter.ConvertBack(
            value: Colors.Red,
            targetType: typeof(ConnectionState),
            parameter: null,
            culture: CultureInfo.InvariantCulture));

        Assert.IsType<NotSupportedException>(exception);
    }

    [Fact]
    public void SetColor_OverridesGetColor()
    {
        try
        {
            ConnectionStateToColorValueConverter.SetColor(ConnectionState.Pulse, Colors.HotPink);
            Assert.Equal(Colors.HotPink, ConnectionStateToColorValueConverter.GetColor(ConnectionState.Pulse));
        }
        finally
        {
            ConnectionStateToColorValueConverter.ResetToDefaults();
        }
    }

    [Fact]
    public void OverrideProperty_PropagatesToConvert()
    {
        try
        {
            ConnectionStateToColorValueConverter.PulseColor = Colors.HotPink;

            var actual = converter.Convert(
                ConnectionState.Pulse,
                typeof(Color),
                null!,
                CultureInfo.InvariantCulture);

            Assert.Equal(Colors.HotPink, actual);
        }
        finally
        {
            ConnectionStateToColorValueConverter.ResetToDefaults();
        }
    }

    [Fact]
    public void OverrideColor_PropagatesToBrushConverter()
    {
        try
        {
            ConnectionStateToColorValueConverter.PulseColor = Colors.HotPink;

            var brush = ConnectionStateToBrushValueConverter.GetBrush(ConnectionState.Pulse);

            Assert.Equal(Colors.HotPink, brush.Color);
            Assert.True(brush.IsFrozen);
        }
        finally
        {
            ConnectionStateToColorValueConverter.ResetToDefaults();
        }
    }

    [Fact]
    public void ResetToDefaults_RestoresOriginalPalette()
    {
        ConnectionStateToColorValueConverter.PulseColor = Colors.HotPink;
        ConnectionStateToColorValueConverter.ConnectedColor = Colors.Black;

        ConnectionStateToColorValueConverter.ResetToDefaults();

        Assert.Equal(ConnectionStateToColorValueConverter.DefaultPulseColor, ConnectionStateToColorValueConverter.PulseColor);
        Assert.Equal(ConnectionStateToColorValueConverter.DefaultConnectedColor, ConnectionStateToColorValueConverter.ConnectedColor);
    }

    [Fact]
    public void BrushConverter_RebuildsAfterColorOverride()
    {
        var before = ConnectionStateToBrushValueConverter.GetBrush(ConnectionState.Pulse);
        try
        {
            ConnectionStateToColorValueConverter.PulseColor = Colors.HotPink;

            var after = ConnectionStateToBrushValueConverter.GetBrush(ConnectionState.Pulse);

            Assert.NotEqual(before.Color, after.Color);
            Assert.Equal(Colors.HotPink, after.Color);
        }
        finally
        {
            ConnectionStateToColorValueConverter.ResetToDefaults();
        }
    }
}