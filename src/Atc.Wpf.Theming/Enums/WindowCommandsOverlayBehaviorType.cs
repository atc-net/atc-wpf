// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Theming;

[Flags]
[SuppressMessage("Design", "CA1008:Enums should have zero value", Justification = "OK.")]
[SuppressMessage("Critical Code Smell", "S2346:Flags enumerations zero-value members should be named \"None\"", Justification = "OK.")]
[SuppressMessage("Maintainability", "S2342:Enumeration types should comply with a naming convention", Justification = "OK.")]
public enum WindowCommandsOverlayBehaviorType
{
    /// <summary>
    /// Doesn't overlay a hidden TitleBar.
    /// </summary>
    Never = 0,

    /// <summary>
    /// Overlays a hidden TitleBar.
    /// </summary>
    HiddenTitleBar = 1 << 0,
}