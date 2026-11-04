namespace Atc.Wpf.Forms.Tests.Dialogs;

/// <summary>
/// The button brushes of <see cref="Atc.Wpf.Forms.Dialogs.DialogBoxSettings"/> are read from the theme when they are
/// read (not once per process), so dialogs follow theme changes; explicitly set brushes win.
/// </summary>
public sealed class DialogBoxSettingsThemeBrushTests
{
    [StaFact]
    public void WithoutATheme_TheButtonBrushesFallBackToPlainColors()
    {
        var settings = new Atc.Wpf.Forms.Dialogs.DialogBoxSettings(Atc.Wpf.Forms.Dialogs.DialogBoxType.YesNo);

        Assert.NotNull(settings.AffirmativeButtonBackground);
        Assert.NotNull(settings.AffirmativeButtonForeground);
        Assert.NotNull(settings.NegativeButtonBackground);
        Assert.NotNull(settings.NegativeButtonForeground);
    }

    [StaFact]
    public void AnExplicitBrush_WinsOverTheTheme()
    {
        var settings = new Atc.Wpf.Forms.Dialogs.DialogBoxSettings(Atc.Wpf.Forms.Dialogs.DialogBoxType.YesNo)
        {
            NegativeButtonBackground = Brushes.Red,
            AffirmativeButtonForeground = Brushes.Green,
        };

        Assert.Same(Brushes.Red, settings.NegativeButtonBackground);
        Assert.Same(Brushes.Green, settings.AffirmativeButtonForeground);
    }

    [StaFact]
    public void UsePrimaryAccentColor_ReplacesExplicitAffirmativeBrushes_WithTheAccent()
    {
        var settings = new Atc.Wpf.Forms.Dialogs.DialogBoxSettings(Atc.Wpf.Forms.Dialogs.DialogBoxType.YesNo)
        {
            AffirmativeButtonBackground = Brushes.Red,
        };

        settings.UsePrimaryAccentColor = true;

        Assert.NotSame(Brushes.Red, settings.AffirmativeButtonBackground);
        Assert.True(settings.UsePrimaryAccentColor);
    }
}