namespace Atc.Wpf.Controls.Tests.Selectors;

// Changing the UI culture is process-wide, so these tests don't run alongside others.
[Collection(nameof(UiCultureTestGroup))]
public sealed class CheckListBoxCultureTests : IDisposable
{
    private readonly CultureInfo originalUiCulture = Atc.Wpf.Translation.CultureManager.UiCulture;

    public CheckListBoxCultureTests()
        => Atc.Wpf.Translation.CultureManager.UiCulture = new CultureInfo("en-US");

    public void Dispose()
    {
        Atc.Wpf.Translation.CultureManager.UiCulture = originalUiCulture;
        Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();
    }

    [StaFact]
    public void UiCultureChanged_translates_the_built_in_texts()
    {
        var sut = new CheckListBox
        {
            Items = new Dictionary<string, string>(StringComparer.Ordinal) { ["a"] = "A", ["b"] = "B" },
            MaxSelectionCount = 2,
        };
        Assert.Equal("Select all", sut.PartSelectAll.Content);
        Assert.Equal("0 of max 2", sut.PartCount.Text);

        Atc.Wpf.Translation.CultureManager.UiCulture = new CultureInfo("da-DK");

        Assert.Equal("Vælg alle", sut.PartSelectAll.Content);
        Assert.Equal("Ryd", sut.PartClear.Content);
        Assert.Equal("Ingen resultater", sut.PartNoMatches.Text);
        Assert.Equal("0 af maks. 2", sut.PartCount.Text);
    }
}