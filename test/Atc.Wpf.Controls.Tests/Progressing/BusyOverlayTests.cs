namespace Atc.Wpf.Controls.Tests.Progressing;

// Changing the UI culture is process-wide, so these tests don't run alongside others.
[Collection(nameof(UiCultureTestGroup))]
public sealed class BusyOverlayTests : IDisposable
{
    private readonly CultureInfo originalUiCulture = Atc.Wpf.Translation.CultureManager.UiCulture;

    public BusyOverlayTests()
        => Atc.Wpf.Translation.CultureManager.UiCulture = new CultureInfo("en-US");

    public void Dispose()
    {
        Atc.Wpf.Translation.CultureManager.UiCulture = originalUiCulture;
        Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();
    }

    [StaFact]
    public void UiCultureChanged_CustomBusyContent_IsKept()
    {
        var overlay = new BusyOverlay { BusyContent = "Saving invoice..." };

        Atc.Wpf.Translation.CultureManager.UiCulture = new CultureInfo("da-DK");

        Assert.Equal("Saving invoice...", overlay.BusyContent);
    }

    [StaFact]
    public void UiCultureChanged_DefaultBusyContent_IsTranslated()
    {
        var overlay = new BusyOverlay { BusyContent = Atc.Wpf.Resources.Miscellaneous.PleaseWait3Dots };
        Assert.Equal("Please wait...", overlay.BusyContent);

        Atc.Wpf.Translation.CultureManager.UiCulture = new CultureInfo("da-DK");

        Assert.Equal("Vent venligst...", overlay.BusyContent);
    }

    [StaFact]
    public void UiCultureChanged_BoundDefaultBusyContent_KeepsTheBinding()
    {
        var overlay = new BusyOverlay();
        BindingOperations.SetBinding(
            overlay,
            BusyOverlay.BusyContentProperty,
            new Binding { Source = Atc.Wpf.Resources.Miscellaneous.PleaseWait3Dots });

        Atc.Wpf.Translation.CultureManager.UiCulture = new CultureInfo("da-DK");

        Assert.NotNull(BindingOperations.GetBindingExpression(overlay, BusyOverlay.BusyContentProperty));
        Assert.Equal("Vent venligst...", overlay.BusyContent);
    }
}