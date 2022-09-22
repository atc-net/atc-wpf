namespace Atc.Wpf.Theming.Controls.Windows
{
    [Flags]
    [SuppressMessage("Design", "CA1008:Enums should have zero value", Justification = "OK.")]
    [SuppressMessage("Critical Code Smell", "S2346:Flags enumerations zero-value members should be named \"None\"", Justification = "OK.")]
    public enum OverlayBehavior
    {
        /// <summary>
        /// Doesn't overlay Overlays nor a hidden TitleBar.
        /// </summary>
        Never = 0,

        /// <summary>
        /// Overlays opened controls.
        /// </summary>
        Overlays = 1 << 0,

        /// <summary>
        /// Overlays a hidden TitleBar.
        /// </summary>
        HiddenTitleBar = 1 << 1,

        Always = ~(-1 << 2),
    }
}