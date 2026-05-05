namespace Atc.Wpf.Hardware.Tests.Resources;

[Collection("Localization")]
public sealed class MiscellaneousLocalizationTests
{
    [Theory]
    [InlineData("", "Refresh")]
    [InlineData("da-DK", "Opdater")]
    [InlineData("de-DE", "Aktualisieren")]
    public void Refresh_LocalizesByCulture(
        string cultureName,
        string expected)
        => AssertLocalized(cultureName, expected, () => Atc.Wpf.Hardware.Resources.Miscellaneous.Refresh);

    [Theory]
    [InlineData("", "Available")]
    [InlineData("da-DK", "Tilgængelig")]
    [InlineData("de-DE", "Verfügbar")]
    public void Available_LocalizesByCulture(
        string cultureName,
        string expected)
        => AssertLocalized(cultureName, expected, () => Atc.Wpf.Hardware.Resources.Miscellaneous.Available);

    [Theory]
    [InlineData("", "In use")]
    [InlineData("da-DK", "I brug")]
    [InlineData("de-DE", "In Verwendung")]
    public void InUse_LocalizesByCulture(
        string cultureName,
        string expected)
        => AssertLocalized(cultureName, expected, () => Atc.Wpf.Hardware.Resources.Miscellaneous.InUse);

    [Theory]
    [InlineData("", "Disconnected")]
    [InlineData("da-DK", "Frakoblet")]
    [InlineData("de-DE", "Getrennt")]
    public void Disconnected_LocalizesByCulture(
        string cultureName,
        string expected)
        => AssertLocalized(cultureName, expected, () => Atc.Wpf.Hardware.Resources.Miscellaneous.Disconnected);

    [Theory]
    [InlineData("", "Device disconnected")]
    [InlineData("da-DK", "Enhed frakoblet")]
    [InlineData("de-DE", "Gerät getrennt")]
    public void DeviceDisconnected_LocalizesByCulture(
        string cultureName,
        string expected)
        => AssertLocalized(cultureName, expected, () => Atc.Wpf.Hardware.Resources.Miscellaneous.DeviceDisconnected);

    [Theory]
    [InlineData("", "Select serial port…")]
    [InlineData("da-DK", "Vælg seriel port…")]
    [InlineData("de-DE", "Seriellen Anschluss auswählen…")]
    public void SelectSerialPort_LocalizesByCulture(
        string cultureName,
        string expected)
        => AssertLocalized(cultureName, expected, () => Atc.Wpf.Hardware.Resources.Miscellaneous.SelectSerialPort);

    [Theory]
    [InlineData("", "Audio Input")]
    [InlineData("da-DK", "Lydindgang")]
    [InlineData("de-DE", "Audioeingang")]
    public void AudioInput_LocalizesByCulture(
        string cultureName,
        string expected)
        => AssertLocalized(cultureName, expected, () => Atc.Wpf.Hardware.Resources.Miscellaneous.AudioInput);

    [Theory]
    [InlineData("", "Bluetooth Device")]
    [InlineData("da-DK", "Bluetooth-enhed")]
    [InlineData("de-DE", "Bluetooth-Gerät")]
    public void BluetoothDevice_LocalizesByCulture(
        string cultureName,
        string expected)
        => AssertLocalized(cultureName, expected, () => Atc.Wpf.Hardware.Resources.Miscellaneous.BluetoothDevice);

    [Fact]
    public void DanishCulture_FallsBackToInvariantForUntranslatedKeys()
    {
        // No specific keys are untranslated in this resx; this test instead asserts
        // that the fallback chain itself works by querying with an unrelated culture.
        var original = Atc.Wpf.Hardware.Resources.Miscellaneous.Culture;
        try
        {
            Atc.Wpf.Hardware.Resources.Miscellaneous.Culture = new CultureInfo("ja-JP");
            Assert.Equal("Refresh", Atc.Wpf.Hardware.Resources.Miscellaneous.Refresh);
        }
        finally
        {
            Atc.Wpf.Hardware.Resources.Miscellaneous.Culture = original;
        }
    }

    private static void AssertLocalized(
        string cultureName,
        string expected,
        Func<string> read)
    {
        var original = Atc.Wpf.Hardware.Resources.Miscellaneous.Culture;
        try
        {
            Atc.Wpf.Hardware.Resources.Miscellaneous.Culture = string.IsNullOrEmpty(cultureName)
                ? CultureInfo.InvariantCulture
                : new CultureInfo(cultureName);

            Assert.Equal(expected, read());
        }
        finally
        {
            Atc.Wpf.Hardware.Resources.Miscellaneous.Culture = original;
        }
    }
}