namespace Atc.Wpf.Controls.Tests.Selectors;

// Changing the UI culture is process-wide, so these tests don't run alongside others.
[Collection(nameof(UiCultureTestGroup))]
public sealed class CountrySelectorTests : IDisposable
{
    private readonly CultureInfo originalUiCulture = Atc.Wpf.Translation.CultureManager.UiCulture;

    public CountrySelectorTests()
        => Atc.Wpf.Translation.CultureManager.UiCulture = new CultureInfo("en-US");

    public void Dispose()
    {
        Atc.Wpf.Translation.CultureManager.UiCulture = originalUiCulture;
        Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();
    }

    [StaFact]
    public void UpdateUiCultureOnChangeEvent_IsOffByDefault()
    {
        var sut = new CountrySelector();

        Assert.False(sut.UpdateUiCultureOnChangeEvent);
    }

    [StaFact]
    public void SelectedKey_Changed_WithoutUpdateUiCulture_KeepsTheUiCulture()
    {
        var sut = new CountrySelector();

        sut.SelectedKey = "1030";

        Assert.Equal("en-US", Atc.Wpf.Translation.CultureManager.UiCulture.Name);
    }

    [StaFact]
    public void SelectedKey_Changed_WithUpdateUiCulture_SetsTheUiCulture()
    {
        var sut = new CountrySelector { UpdateUiCultureOnChangeEvent = true };

        sut.SelectedKey = "1030";

        Assert.Equal("da-DK", Atc.Wpf.Translation.CultureManager.UiCulture.Name);
    }

    [StaFact]
    public void SelectedKey_SetToAFirstItemPlaceholder_WithUpdateUiCulture_KeepsTheUiCulture()
    {
        var sut = new CountrySelector { UpdateUiCultureOnChangeEvent = true };

        sut.SelectedKey = "-1";

        Assert.Equal("en-US", Atc.Wpf.Translation.CultureManager.UiCulture.Name);
    }
}