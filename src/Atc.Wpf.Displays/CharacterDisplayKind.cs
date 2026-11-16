namespace Atc.Wpf.Displays;

/// <summary>
/// Specifies the display technology a character display imitates.
/// </summary>
public enum CharacterDisplayKind
{
    /// <summary>
    /// A liquid-crystal display: dark or light dots on a backlit panel.
    /// </summary>
    Lcd,

    /// <summary>
    /// A vacuum fluorescent display: glowing dots on a dark glass.
    /// </summary>
    Vfd,
}