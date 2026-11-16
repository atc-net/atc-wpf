namespace Atc.Wpf.Displays;

/// <summary>
/// Specifies the color scheme of a character display.
/// </summary>
public enum CharacterDisplayColor
{
    /// <summary>
    /// LCD: dark dots on a yellow-green backlight. A VFD shows it as <see cref="BlueGreen"/>.
    /// </summary>
    YellowGreen,

    /// <summary>
    /// LCD: light dots on a blue backlight. A VFD shows it as <see cref="BlueGreen"/>.
    /// </summary>
    Blue,

    /// <summary>
    /// LCD: white dots on a black background. A VFD shows it as <see cref="BlueGreen"/>.
    /// </summary>
    WhiteOnBlack,

    /// <summary>
    /// LCD: dark dots on an amber backlight; VFD: amber dots.
    /// </summary>
    Amber,

    /// <summary>
    /// LCD: light dots on a red backlight; VFD: red dots.
    /// </summary>
    Red,

    /// <summary>
    /// LCD: dark dots on a green backlight; VFD: green dots.
    /// </summary>
    Green,

    /// <summary>
    /// VFD: blue-green dots, the classic vacuum fluorescent color. An LCD shows it as <see cref="YellowGreen"/>.
    /// </summary>
    BlueGreen,
}