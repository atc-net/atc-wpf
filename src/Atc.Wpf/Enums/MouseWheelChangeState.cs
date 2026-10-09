// ReSharper disable CheckNamespace
namespace Atc.Wpf;

/// <summary>
/// Specifies how much a value changes when the mouse wheel is scrolled.
/// </summary>
public enum MouseWheelChangeState
{
    /// <summary>No change is specified.</summary>
    None,

    /// <summary>The value changes by the small change amount.</summary>
    SmallChange,

    /// <summary>The value changes by the large change amount.</summary>
    LargeChange,
}