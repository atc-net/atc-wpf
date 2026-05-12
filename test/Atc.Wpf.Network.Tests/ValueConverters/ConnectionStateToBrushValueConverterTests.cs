namespace Atc.Wpf.Network.Tests.ValueConverters;

[Collection("Sequential")]
public sealed class ConnectionStateToBrushValueConverterTests
{
    private readonly IValueConverter converter = new ConnectionStateToBrushValueConverter();

    [Fact]
    public void Instance_Is_Not_Null()
        => Assert.NotNull(ConnectionStateToBrushValueConverter.Instance);

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
    public void Convert_KnownStates_ReturnsFrozenSolidColorBrush(
        ConnectionState state)
    {
        var actual = converter.Convert(
            value: state,
            targetType: typeof(SolidColorBrush),
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        var brush = Assert.IsType<SolidColorBrush>(actual);
        Assert.True(brush.IsFrozen, "Returned brush must be frozen.");
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
        var brushA = (SolidColorBrush)converter.Convert(a, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);
        var brushB = (SolidColorBrush)converter.Convert(b, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        Assert.NotEqual(brushA.Color, brushB.Color);
    }

    [Fact]
    public void Convert_SameStateTwice_ReturnsSameInstance()
    {
        var first = converter.Convert(ConnectionState.Connected, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);
        var second = converter.Convert(ConnectionState.Connected, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        Assert.Same(first, second);
    }

    [Fact]
    public void Convert_Null_ReturnsNeutralBrush()
    {
        var actual = converter.Convert(
            value: null,
            targetType: typeof(SolidColorBrush),
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        Assert.IsType<SolidColorBrush>(actual);
    }

    [Fact]
    public void Convert_NonConnectionState_ReturnsNeutralBrush()
    {
        var actual = converter.Convert(
            value: "NotAState",
            targetType: typeof(SolidColorBrush),
            parameter: null,
            culture: CultureInfo.InvariantCulture);

        Assert.IsType<SolidColorBrush>(actual);
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
    public void Convert_KnownStates_ColorHasReasonableContrastForBothThemes(
        ConnectionState state)
    {
        var brush = (SolidColorBrush)converter.Convert(state, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        var luminance = RelativeLuminance(brush.Color);
        const double DarkThemeLuminance = 0.0117;
        const double LightThemeLuminance = 1.0;

        var contrastOnLight = ContrastRatio(luminance, LightThemeLuminance);
        var contrastOnDark = ContrastRatio(luminance, DarkThemeLuminance);

        // Status pips are hue-coded, not contrast-coded — WCAG 3:1 is impossible for a natural
        // amber/red palette on both white AND dark. 1.5:1 is enough to catch invisible regressions
        // (e.g., gray-on-gray or accidental white) while accommodating real status-color hues.
        Assert.True(contrastOnLight >= 1.5, $"{state} brush has only {contrastOnLight:F2}:1 contrast on light theme");
        Assert.True(contrastOnDark >= 1.5, $"{state} brush has only {contrastOnDark:F2}:1 contrast on dark theme");
    }

    private static double ContrastRatio(
        double a,
        double b)
    {
        var lighter = System.Math.Max(a, b);
        var darker = System.Math.Min(a, b);
        return (lighter + 0.05) / (darker + 0.05);
    }

    [Fact]
    public void ConvertBack_Throws_NotSupportedException()
    {
        var exception = Record.Exception(() => converter.ConvertBack(
            value: Brushes.Red,
            targetType: typeof(ConnectionState),
            parameter: null,
            culture: CultureInfo.InvariantCulture));

        Assert.IsType<NotSupportedException>(exception);
    }

    private static double RelativeLuminance(Color color)
    {
        static double Channel(byte c)
        {
            var v = c / 255.0;
            return v <= 0.03928 ? v / 12.92 : System.Math.Pow((v + 0.055) / 1.055, 2.4);
        }

        return (0.2126 * Channel(color.R)) + (0.7152 * Channel(color.G)) + (0.0722 * Channel(color.B));
    }
}