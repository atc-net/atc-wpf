namespace Atc.Wpf.Controls.Tests.Selectors;

// Changing the UI culture is process-wide, so these tests don't run alongside others.
[Collection(nameof(UiCultureTestGroup))]
public sealed class MultiSelectComboBoxCultureTests : IDisposable
{
    private readonly CultureInfo originalUiCulture = Atc.Wpf.Translation.CultureManager.UiCulture;

    public MultiSelectComboBoxCultureTests()
        => Atc.Wpf.Translation.CultureManager.UiCulture = new CultureInfo("en-US");

    public void Dispose()
    {
        Atc.Wpf.Translation.CultureManager.UiCulture = originalUiCulture;
        Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();
    }

    [StaFact]
    public void UiCultureChanged_translates_the_default_summary_texts()
    {
        var sut = new MultiSelectComboBox
        {
            Items = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["a"] = "A",
                ["b"] = "B",
                ["c"] = "C",
                ["d"] = "D",
            },
            MaxNamesInSummary = 1,
        };

        sut.SelectedKeys.Add("a");
        sut.SelectedKeys.Add("b");
        Assert.Equal("2 of 4 selected", sut.SummaryText);

        Atc.Wpf.Translation.CultureManager.UiCulture = new CultureInfo("da-DK");
        Assert.Equal("2 af 4 valgt", sut.SummaryText);

        sut.SelectAll();
        Assert.Equal("Alle", sut.SummaryText);
    }
}