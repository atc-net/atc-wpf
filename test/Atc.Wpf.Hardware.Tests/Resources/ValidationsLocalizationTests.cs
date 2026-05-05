namespace Atc.Wpf.Hardware.Tests.Resources;

[Collection("Localization")]
public sealed class ValidationsLocalizationTests
{
    [Theory]
    [InlineData("", "A device must be selected")]
    [InlineData("da-DK", "Der skal vælges en enhed")]
    [InlineData("de-DE", "Es muss ein Gerät ausgewählt werden")]
    public void DeviceIsRequired_LocalizesByCulture(
        string cultureName,
        string expected)
        => AssertLocalized(cultureName, expected, () => Atc.Wpf.Hardware.Resources.Validations.DeviceIsRequired);

    [Theory]
    [InlineData("", "The selected device is no longer available")]
    [InlineData("da-DK", "Den valgte enhed er ikke længere tilgængelig")]
    [InlineData("de-DE", "Das ausgewählte Gerät ist nicht mehr verfügbar")]
    public void DeviceNoLongerAvailable_LocalizesByCulture(
        string cultureName,
        string expected)
        => AssertLocalized(cultureName, expected, () => Atc.Wpf.Hardware.Resources.Validations.DeviceNoLongerAvailable);

    [Theory]
    [InlineData("", "The selected device is currently in use")]
    [InlineData("da-DK", "Den valgte enhed er i brug")]
    [InlineData("de-DE", "Das ausgewählte Gerät wird derzeit verwendet")]
    public void DeviceCurrentlyInUse_LocalizesByCulture(
        string cultureName,
        string expected)
        => AssertLocalized(cultureName, expected, () => Atc.Wpf.Hardware.Resources.Validations.DeviceCurrentlyInUse);

    private static void AssertLocalized(
        string cultureName,
        string expected,
        Func<string> read)
    {
        var original = Atc.Wpf.Hardware.Resources.Validations.Culture;
        try
        {
            Atc.Wpf.Hardware.Resources.Validations.Culture = string.IsNullOrEmpty(cultureName)
                ? CultureInfo.InvariantCulture
                : new CultureInfo(cultureName);

            Assert.Equal(expected, read());
        }
        finally
        {
            Atc.Wpf.Hardware.Resources.Validations.Culture = original;
        }
    }
}