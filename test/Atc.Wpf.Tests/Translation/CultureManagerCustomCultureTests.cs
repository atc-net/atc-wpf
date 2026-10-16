namespace Atc.Wpf.Tests.Translation;

[Collection(nameof(UiCultureTestGroup))]
public sealed class CultureManagerCustomCultureTests : IDisposable
{
    private const int LocaleCustomUnspecified = 4096;

    private static readonly string[] CandidateCultureNames =
    [
        "en-150",
        "en-DK",
        "en-001",
        "es-419",
        "ar-001",
        "ca-ES-valencia",
        "x-custom",
    ];

    private readonly CultureInfo originalUiCulture = CultureManager.UiCulture;

    public void Dispose()
        => CultureManager.UiCulture = originalUiCulture;

    [StaFact]
    public void UiCulture_can_be_changed_away_from_a_culture_without_its_own_LCID()
    {
        var custom = FindCultureWithCustomLcid();
        if (custom is null)
        {
            Assert.Skip("No culture reporting LCID 4096 (LOCALE_CUSTOM_UNSPECIFIED) is available on this machine.");
            return;
        }

        UiCultureEventArgs? raised = null;
        EventHandler<UiCultureEventArgs> handler = (_, e) => raised = e;
        CultureManager.UiCulture = custom;
        CultureManager.UiCultureChanged += handler;

        try
        {
            var exception = Record.Exception(() => CultureManager.UiCulture = new CultureInfo("da-DK"));

            Assert.Null(exception);
            Assert.NotNull(raised);
            Assert.Equal(custom.Name, raised.OldCulture.Name);
            Assert.Equal("da-DK", raised.NewCulture.Name);
        }
        finally
        {
            CultureManager.UiCultureChanged -= handler;
        }
    }

    [StaFact]
    public void UiCultureChanged_reports_the_previous_culture_by_name()
    {
        UiCultureEventArgs? raised = null;
        EventHandler<UiCultureEventArgs> handler = (_, e) => raised = e;
        CultureManager.UiCulture = new CultureInfo("de-DE");
        CultureManager.UiCultureChanged += handler;

        try
        {
            CultureManager.UiCulture = new CultureInfo("da-DK");

            Assert.NotNull(raised);
            Assert.Equal("de-DE", raised.OldCulture.Name);
            Assert.Equal("da-DK", raised.NewCulture.Name);
        }
        finally
        {
            CultureManager.UiCultureChanged -= handler;
        }
    }

    /// <summary>
    /// Finds a culture that has no LCID of its own (reports 4096) and that cannot be re-created from that LCID,
    /// which is the situation the UI culture setter used to fail in.
    /// </summary>
    private static CultureInfo? FindCultureWithCustomLcid()
    {
        foreach (var name in CandidateCultureNames)
        {
            CultureInfo culture;
            try
            {
                culture = new CultureInfo(name);
            }
            catch (CultureNotFoundException)
            {
                continue;
            }

            if (culture.LCID != LocaleCustomUnspecified)
            {
                continue;
            }

            try
            {
                _ = new CultureInfo(culture.LCID);
            }
            catch (CultureNotFoundException)
            {
                return culture;
            }
        }

        return null;
    }
}