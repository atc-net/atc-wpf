namespace Atc.Wpf.Displays;

/// <summary>
/// The kind of colour panel a colour graphic display imitates.
/// </summary>
public enum ColorPanelKind
{
    /// <summary>
    /// A TFT LCD: a lit backlight gives a dark pixel a faint glow instead of true black.
    /// </summary>
    Tft,

    /// <summary>
    /// A colour OLED: every pixel emits its own light, so black is truly black.
    /// </summary>
    Oled,
}