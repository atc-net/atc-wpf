namespace Atc.Wpf.Tests.Translation;

[Collection(nameof(UiCultureTestGroup))]
[SuppressMessage("Globalization", "CA1304:Specify CultureInfo", Justification = "The overload taking only the UI culture is what is tested.")]
public sealed class CultureManagerSetCulturesTests : IDisposable
{
    private readonly bool originalSynchronize = CultureManager.SynchronizeThreadCulture;
    private readonly CultureInfo originalUiCulture = CultureManager.UiCulture;
    private readonly CultureInfo? originalDefaultCulture = CultureInfo.DefaultThreadCurrentCulture;
    private readonly CultureInfo? originalDefaultUiCulture = CultureInfo.DefaultThreadCurrentUICulture;
    private readonly CultureInfo originalCulture = CultureInfo.CurrentCulture;
    private readonly CultureInfo originalCurrentUiCulture = CultureInfo.CurrentUICulture;

    public void Dispose()
    {
        CultureManager.SynchronizeThreadCulture = false;
        CultureManager.UiCulture = originalUiCulture;
        CultureManager.SynchronizeThreadCulture = originalSynchronize;
        CultureInfo.DefaultThreadCurrentCulture = originalDefaultCulture;
        CultureInfo.DefaultThreadCurrentUICulture = originalDefaultUiCulture;
        CultureInfo.CurrentCulture = originalCulture;
        CultureInfo.CurrentUICulture = originalCurrentUiCulture;
    }

    [StaFact]
    public void SetCultures_with_ui_culture_sets_the_ui_culture_and_an_English_backend()
    {
        CultureManager.SetCultures(new CultureInfo("da-DK"), synchronizeThreadCultures: false);

        Assert.Equal("da-DK", CultureManager.UiCulture.Name);
        Assert.Equal("en-US", CultureInfo.DefaultThreadCurrentCulture?.Name);
        Assert.Equal("en-US", CultureInfo.CurrentCulture.Name);
        Assert.False(CultureManager.SynchronizeThreadCulture);
    }

    [StaFact]
    public void SetCultures_with_ui_culture_and_sync_makes_the_backend_follow_the_ui_culture()
    {
        CultureManager.SetCultures(new CultureInfo("da-DK"), synchronizeThreadCultures: true);

        Assert.Equal("da-DK", CultureManager.UiCulture.Name);
        Assert.Equal("da-DK", CultureInfo.CurrentCulture.Name);
        Assert.True(CultureManager.SynchronizeThreadCulture);
    }

    [StaFact]
    public void SetCultures_with_culture_name_keeps_the_sync_flag()
    {
        CultureManager.SetCultures("da-DK", synchronizeThreadCultures: true);

        Assert.True(CultureManager.SynchronizeThreadCulture);
        Assert.Equal("da-DK", CultureManager.UiCulture.Name);
        Assert.Equal("da-DK", CultureInfo.CurrentCulture.Name);
    }

    [StaFact]
    public void SetCultures_with_culture_name_without_sync_keeps_an_English_backend()
    {
        CultureManager.SetCultures("de-DE", synchronizeThreadCultures: false);

        Assert.False(CultureManager.SynchronizeThreadCulture);
        Assert.Equal("de-DE", CultureManager.UiCulture.Name);
        Assert.Equal("en-US", CultureInfo.CurrentCulture.Name);
    }

    [StaFact]
    public void SetCultures_with_an_invalid_culture_name_falls_back_to_English_and_keeps_the_sync_flag()
    {
        CultureManager.SetCultures("!invalid culture!", synchronizeThreadCultures: true);

        Assert.True(CultureManager.SynchronizeThreadCulture);
        Assert.Equal("en-US", CultureManager.UiCulture.Name);
    }
}