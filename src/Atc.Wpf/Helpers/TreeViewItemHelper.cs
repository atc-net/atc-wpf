namespace Atc.Wpf.Helpers;

/// <summary>Provides attached properties for a <see cref="TreeViewItem"/>, such as the style of its expander toggle button.</summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static partial class TreeViewItemHelper
{
    /// <summary>Gets or sets the value of the ToggleButtonStyle attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Style? toggleButtonStyle;
}