namespace Atc.Wpf.Helpers;

/// <summary>Provides attached properties for styling a <see cref="RadioButton"/>, such as sizes and brushes for the outer ellipse and check glyph in each visual state.</summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static partial class RadioButtonHelper
{
    /// <summary>Gets or sets the value of the RadioSize attached property.</summary>
    [AttachedProperty(
        DefaultValue = 18.0,
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static double radioSize;

    /// <summary>Gets or sets the value of the RadioCheckSize attached property.</summary>
    [AttachedProperty(
        DefaultValue = 10.0,
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static double radioCheckSize;

    /// <summary>Gets or sets the value of the RadioStrokeThickness attached property.</summary>
    [AttachedProperty(
        DefaultValue = 1.0,
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static double radioStrokeThickness;

    /// <summary>Gets or sets the value of the ForegroundPointerOver attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? foregroundPointerOver;

    /// <summary>Gets or sets the value of the ForegroundPressed attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? foregroundPressed;

    /// <summary>Gets or sets the value of the ForegroundDisabled attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? foregroundDisabled;

    /// <summary>Gets or sets the value of the BackgroundPointerOver attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? backgroundPointerOver;

    /// <summary>Gets or sets the value of the BackgroundPressed attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? backgroundPressed;

    /// <summary>Gets or sets the value of the BackgroundDisabled attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? backgroundDisabled;

    /// <summary>Gets or sets the value of the BorderBrushPointerOver attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? borderBrushPointerOver;

    /// <summary>Gets or sets the value of the BorderBrushPressed attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? borderBrushPressed;

    /// <summary>Gets or sets the value of the BorderBrushDisabled attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? borderBrushDisabled;

    /// <summary>Gets or sets the value of the OuterEllipseFill attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? outerEllipseFill;

    /// <summary>Gets or sets the value of the OuterEllipseFillPointerOver attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? outerEllipseFillPointerOver;

    /// <summary>Gets or sets the value of the OuterEllipseFillPressed attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? outerEllipseFillPressed;

    /// <summary>Gets or sets the value of the OuterEllipseFillDisabled attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? outerEllipseFillDisabled;

    /// <summary>Gets or sets the value of the OuterEllipseStroke attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? outerEllipseStroke;

    /// <summary>Gets or sets the value of the OuterEllipseStrokePointerOver attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? outerEllipseStrokePointerOver;

    /// <summary>Gets or sets the value of the OuterEllipseStrokePressed attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? outerEllipseStrokePressed;

    /// <summary>Gets or sets the value of the OuterEllipseStrokeDisabled attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? outerEllipseStrokeDisabled;

    /// <summary>Gets or sets the value of the OuterEllipseCheckedFill attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? outerEllipseCheckedFill;

    /// <summary>Gets or sets the value of the OuterEllipseCheckedFillPointerOver attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? outerEllipseCheckedFillPointerOver;

    /// <summary>Gets or sets the value of the OuterEllipseCheckedFillPressed attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? outerEllipseCheckedFillPressed;

    /// <summary>Gets or sets the value of the OuterEllipseCheckedFillDisabled attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? outerEllipseCheckedFillDisabled;

    /// <summary>Gets or sets the value of the OuterEllipseCheckedStroke attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? outerEllipseCheckedStroke;

    /// <summary>Gets or sets the value of the OuterEllipseCheckedStrokePointerOver attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? outerEllipseCheckedStrokePointerOver;

    /// <summary>Gets or sets the value of the OuterEllipseCheckedStrokePressed attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? outerEllipseCheckedStrokePressed;

    /// <summary>Gets or sets the value of the OuterEllipseCheckedStrokeDisabled attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? outerEllipseCheckedStrokeDisabled;

    /// <summary>Gets or sets the value of the CheckGlyphFill attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? checkGlyphFill;

    /// <summary>Gets or sets the value of the CheckGlyphFillPointerOver attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? checkGlyphFillPointerOver;

    /// <summary>Gets or sets the value of the CheckGlyphFillPressed attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? checkGlyphFillPressed;

    /// <summary>Gets or sets the value of the CheckGlyphFillDisabled attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? checkGlyphFillDisabled;

    /// <summary>Gets or sets the value of the CheckGlyphStroke attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? checkGlyphStroke;

    /// <summary>Gets or sets the value of the CheckGlyphStrokePointerOver attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? checkGlyphStrokePointerOver;

    /// <summary>Gets or sets the value of the CheckGlyphStrokePressed attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? checkGlyphStrokePressed;

    /// <summary>Gets or sets the value of the CheckGlyphStrokeDisabled attached property.</summary>
    [AttachedProperty(TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static Brush? checkGlyphStrokeDisabled;
}