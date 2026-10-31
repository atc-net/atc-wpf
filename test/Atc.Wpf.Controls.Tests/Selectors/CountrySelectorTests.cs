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

    [StaFact]
    public void Items_AfterSwitchingToGerman_AreSortedByGermanRules()
    {
        var sut = new CountrySelector { UseOnlySupportedCountries = false };
        sut.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));

        Atc.Wpf.Translation.CultureManager.UiCulture = new CultureInfo("de-DE");

        AssertSortedByCulture(sut.Items.Select(x => x.Culture.CountryDisplayName).ToList(), "de-DE", "Ägypten", "Albanien");
    }

    [StaFact]
    public void Items_LoadedInGerman_AreSortedByGermanRules()
    {
        Atc.Wpf.Translation.CultureManager.UiCulture = new CultureInfo("de-DE");
        var sut = new CountrySelector { UseOnlySupportedCountries = false };

        sut.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));

        AssertSortedByCulture(sut.Items.Select(x => x.Culture.CountryDisplayName).ToList(), "de-DE", "Ägypten", "Albanien");
    }

    [StaFact]
    public void LanguageSelector_ItemsAfterSwitchingToFrench_AreSortedByFrenchRules()
    {
        var sut = new LanguageSelector { UseOnlySupportedLanguages = false, UpdateUiCultureOnChangeEvent = false };
        sut.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));

        Atc.Wpf.Translation.CultureManager.UiCulture = new CultureInfo("fr-FR");

        var names = sut.Items.Select(x => x.Culture.LanguageDisplayName).ToList();
        AssertSortedByCulture(names, "fr-FR", earlier: null, later: null);
    }

    private static void AssertSortedByCulture(
        List<string> names,
        string cultureName,
        string? earlier,
        string? later)
    {
        Assert.NotEmpty(names);
        var comparer = StringComparer.Create(new CultureInfo(cultureName), ignoreCase: true);
        Assert.Equal(names.OrderBy(x => x, comparer).ToList(), names);

        if (earlier is not null && later is not null)
        {
            Assert.Contains(earlier, names, StringComparer.Ordinal);
            Assert.Contains(later, names, StringComparer.Ordinal);
            Assert.True(names.IndexOf(earlier) < names.IndexOf(later), $"{earlier} should come before {later}.");
        }
    }
}