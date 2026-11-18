namespace Atc.Wpf.Helpers;

/// <summary>Provides attached properties for a <see cref="ToggleButton"/>, such as the content direction.</summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static partial class ToggleButtonHelper
{
    /// <summary>Gets or sets the value of the ContentDirection attached property.</summary>
    [AttachedProperty(
        DefaultValue = FlowDirection.LeftToRight,
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static FlowDirection contentDirection;
}