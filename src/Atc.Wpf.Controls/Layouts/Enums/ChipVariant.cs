namespace Atc.Wpf.Controls;

/// <summary>
/// Specifies the variant/type of a Chip control.
/// </summary>
public enum ChipVariant
{
    /// <summary>
    /// Basic chip for displaying information.
    /// </summary>
    Default,

    /// <summary>
    /// Filter chip that can be toggled on/off.
    /// </summary>
    Filter,

    /// <summary>
    /// Input chip with a remove/close button.
    /// </summary>
    Input,

    /// <summary>
    /// Action chip that triggers an action when clicked.
    /// </summary>
    Action,
}