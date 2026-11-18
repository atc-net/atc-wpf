namespace Atc.Wpf.Helpers;

/// <summary>Provides attached properties for styling the header of headered controls, such as header brushes, alignment, margin and font.</summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static partial class HeaderedControlHelper
{
    /// <summary>Gets or sets the value of the HeaderForeground attached property.</summary>
    [AttachedProperty(
        DefaultValueExpression = "Brushes.White",
        MetadataType = DependencyPropertyMetadataType.UIPropertyMetadata,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush headerForeground;

    /// <summary>Gets or sets the value of the HeaderBackground attached property.</summary>
    [AttachedProperty(
        DefaultValueExpression = "Panel.BackgroundProperty.DefaultMetadata.DefaultValue",
        MetadataType = DependencyPropertyMetadataType.UIPropertyMetadata,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Brush headerBackground;

    /// <summary>Gets or sets the value of the HeaderHorizontalContentAlignment attached property.</summary>
    [AttachedProperty(
        DefaultValue = HorizontalAlignment.Stretch,
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static HorizontalAlignment headerHorizontalContentAlignment;

    /// <summary>Gets or sets the value of the HeaderVerticalContentAlignment attached property.</summary>
    [AttachedProperty(
        DefaultValue = VerticalAlignment.Stretch,
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static VerticalAlignment headerVerticalContentAlignment;

    /// <summary>Gets or sets the value of the HeaderMargin attached property.</summary>
    [AttachedProperty(
        DefaultValueExpression = "new Thickness(0)",
        MetadataType = DependencyPropertyMetadataType.UIPropertyMetadata,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Thickness headerMargin;

    /// <summary>Gets or sets the value of the HeaderFontFamily attached property.</summary>
    [AttachedProperty(
        DefaultValueExpression = "SystemFonts.MessageFontFamily",
        Flags = FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static FontFamily headerFontFamily;

    /// <summary>Gets or sets the value of the HeaderFontSize attached property.</summary>
    [AttachedProperty(
        DefaultValueExpression = "SystemFonts.MessageFontSize",
        Flags = FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static double headerFontSize;

    /// <summary>Gets or sets the value of the HeaderFontStretch attached property.</summary>
    [AttachedProperty(
        DefaultValueExpression = "TextElement.FontStretchProperty.DefaultMetadata.DefaultValue",
        Flags = FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static FontStretch headerFontStretch;

    /// <summary>Gets or sets the value of the HeaderFontWeight attached property.</summary>
    [AttachedProperty(
        DefaultValueExpression = "SystemFonts.MessageFontWeight",
        Flags = FrameworkPropertyMetadataOptions.Inherits,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static FontWeight headerFontWeight;
}