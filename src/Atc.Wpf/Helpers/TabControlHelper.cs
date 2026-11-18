namespace Atc.Wpf.Helpers;

/// <summary>Provides attached properties for a <see cref="TabControl"/>, such as the tab item underline and its brushes.</summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static partial class TabControlHelper
{
    /// <summary>Gets or sets the value of the Underlined attached property.</summary>
    [AttachedProperty(DefaultValue = UnderlinedType.None, TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static UnderlinedType underlined;

    /// <summary>Gets or sets the value of the UnderlineBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? underlineBrush;

    /// <summary>Gets or sets the value of the UnderlineSelectedBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? underlineSelectedBrush;

    /// <summary>Gets or sets the value of the UnderlineMouseOverBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? underlineMouseOverBrush;

    /// <summary>Gets or sets the value of the UnderlineMouseOverSelectedBrush attached property.</summary>
    [AttachedProperty(
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush? underlineMouseOverSelectedBrush;

    /// <summary>Gets or sets the value of the UnderlineMargin attached property.</summary>
    [AttachedProperty(
        DefaultValueExpression = "new Thickness(0)",
        MetadataType = DependencyPropertyMetadataType.UIPropertyMetadata,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Thickness underlineMargin;

    /// <summary>Gets or sets the value of the UnderlinePlacement attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Dock? underlinePlacement;
}