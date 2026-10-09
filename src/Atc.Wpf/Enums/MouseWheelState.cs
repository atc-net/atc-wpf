// ReSharper disable CheckNamespace
namespace Atc.Wpf;

/// <summary>
/// Specifies when the mouse wheel changes the value of a control.
/// </summary>
public enum MouseWheelState
{
    /// <summary>The mouse wheel is disabled.</summary>
    None,

    /// <summary>The mouse wheel is enabled when the control has focus.</summary>
    ControlFocused,

    /// <summary>The mouse wheel is enabled when the mouse is over the control.</summary>
    MouseHover,
}