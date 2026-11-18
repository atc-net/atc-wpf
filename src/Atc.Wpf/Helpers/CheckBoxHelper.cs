namespace Atc.Wpf.Helpers;

/// <summary>Provides attached properties for styling a <see cref="CheckBox"/>, such as check size, glyphs and brushes for the unchecked, checked and indeterminate states.</summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static partial class CheckBoxHelper
{
    /// <summary>Gets or sets the value of the CheckSize attached property.</summary>
    [AttachedProperty(
        DefaultValue = 18.0,
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static double checkSize;

    /// <summary>Gets or sets the value of the CheckCornerRadius attached property.</summary>
    [AttachedProperty(
        DefaultValueExpression = "new CornerRadius(0)",
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static CornerRadius checkCornerRadius;

    /// <summary>Gets or sets the value of the CheckStrokeThickness attached property.</summary>
    [AttachedProperty(
        DefaultValue = 1d,
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static double checkStrokeThickness;

    /// <summary>Gets or sets the value of the CheckGlyphUnchecked attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static object? checkGlyphUnchecked;

    /// <summary>Gets or sets the value of the CheckGlyphUncheckedTemplate attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static DataTemplate? checkGlyphUncheckedTemplate;

    /// <summary>Gets or sets the value of the ForegroundUnchecked attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? foregroundUnchecked;

    /// <summary>Gets or sets the value of the BackgroundUnchecked attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? backgroundUnchecked;

    /// <summary>Gets or sets the value of the BorderBrushUnchecked attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? borderBrushUnchecked;

    /// <summary>Gets or sets the value of the CheckBackgroundFillUnchecked attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkBackgroundFillUnchecked;

    /// <summary>Gets or sets the value of the CheckBackgroundStrokeUnchecked attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkBackgroundStrokeUnchecked;

    /// <summary>Gets or sets the value of the CheckGlyphForegroundUnchecked attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkGlyphForegroundUnchecked;

    /// <summary>Gets or sets the value of the ForegroundUncheckedMouseOver attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? foregroundUncheckedMouseOver;

    /// <summary>Gets or sets the value of the BackgroundUncheckedMouseOver attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? backgroundUncheckedMouseOver;

    /// <summary>Gets or sets the value of the BorderBrushUncheckedMouseOver attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? borderBrushUncheckedMouseOver;

    /// <summary>Gets or sets the value of the CheckBackgroundFillUncheckedMouseOver attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkBackgroundFillUncheckedMouseOver;

    /// <summary>Gets or sets the value of the CheckBackgroundStrokeUncheckedMouseOver attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkBackgroundStrokeUncheckedMouseOver;

    /// <summary>Gets or sets the value of the CheckGlyphForegroundUncheckedMouseOver attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkGlyphForegroundUncheckedMouseOver;

    /// <summary>Gets or sets the value of the ForegroundUncheckedPressed attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? foregroundUncheckedPressed;

    /// <summary>Gets or sets the value of the BackgroundUncheckedPressed attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? backgroundUncheckedPressed;

    /// <summary>Gets or sets the value of the BorderBrushUncheckedPressed attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? borderBrushUncheckedPressed;

    /// <summary>Gets or sets the value of the CheckBackgroundFillUncheckedPressed attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkBackgroundFillUncheckedPressed;

    /// <summary>Gets or sets the value of the CheckBackgroundStrokeUncheckedPressed attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkBackgroundStrokeUncheckedPressed;

    /// <summary>Gets or sets the value of the CheckGlyphForegroundUncheckedPressed attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkGlyphForegroundUncheckedPressed;

    /// <summary>Gets or sets the value of the ForegroundUncheckedDisabled attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? foregroundUncheckedDisabled;

    /// <summary>Gets or sets the value of the BackgroundUncheckedDisabled attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? backgroundUncheckedDisabled;

    /// <summary>Gets or sets the value of the BorderBrushUncheckedDisabled attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? borderBrushUncheckedDisabled;

    /// <summary>Gets or sets the value of the CheckBackgroundFillUncheckedDisabled attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkBackgroundFillUncheckedDisabled;

    /// <summary>Gets or sets the value of the CheckBackgroundStrokeUncheckedDisabled attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkBackgroundStrokeUncheckedDisabled;

    /// <summary>Gets or sets the value of the CheckGlyphForegroundUncheckedDisabled attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkGlyphForegroundUncheckedDisabled;

    /// <summary>Gets or sets the value of the CheckGlyphChecked attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static object? checkGlyphChecked;

    /// <summary>Gets or sets the value of the CheckGlyphCheckedTemplate attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static DataTemplate? checkGlyphCheckedTemplate;

    /// <summary>Gets or sets the value of the ForegroundChecked attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? foregroundChecked;

    /// <summary>Gets or sets the value of the BackgroundChecked attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? backgroundChecked;

    /// <summary>Gets or sets the value of the BorderBrushChecked attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? borderBrushChecked;

    /// <summary>Gets or sets the value of the CheckBackgroundFillChecked attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkBackgroundFillChecked;

    /// <summary>Gets or sets the value of the CheckBackgroundStrokeChecked attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkBackgroundStrokeChecked;

    /// <summary>Gets or sets the value of the CheckGlyphForegroundChecked attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkGlyphForegroundChecked;

    /// <summary>Gets or sets the value of the ForegroundCheckedMouseOver attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? foregroundCheckedMouseOver;

    /// <summary>Gets or sets the value of the BackgroundCheckedMouseOver attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? backgroundCheckedMouseOver;

    /// <summary>Gets or sets the value of the BorderBrushCheckedMouseOver attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? borderBrushCheckedMouseOver;

    /// <summary>Gets or sets the value of the CheckBackgroundFillCheckedMouseOver attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkBackgroundFillCheckedMouseOver;

    /// <summary>Gets or sets the value of the CheckBackgroundStrokeCheckedMouseOver attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkBackgroundStrokeCheckedMouseOver;

    /// <summary>Gets or sets the value of the CheckGlyphForegroundCheckedMouseOver attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkGlyphForegroundCheckedMouseOver;

    /// <summary>Gets or sets the value of the ForegroundCheckedPressed attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? foregroundCheckedPressed;

    /// <summary>Gets or sets the value of the BackgroundCheckedPressed attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? backgroundCheckedPressed;

    /// <summary>Gets or sets the value of the BorderBrushCheckedPressed attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? borderBrushCheckedPressed;

    /// <summary>Gets or sets the value of the CheckBackgroundFillCheckedPressed attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkBackgroundFillCheckedPressed;

    /// <summary>Gets or sets the value of the CheckBackgroundStrokeCheckedPressed attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkBackgroundStrokeCheckedPressed;

    /// <summary>Gets or sets the value of the CheckGlyphForegroundCheckedPressed attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkGlyphForegroundCheckedPressed;

    /// <summary>Gets or sets the value of the ForegroundCheckedDisabled attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? foregroundCheckedDisabled;

    /// <summary>Gets or sets the value of the BackgroundCheckedDisabled attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? backgroundCheckedDisabled;

    /// <summary>Gets or sets the value of the BorderBrushCheckedDisabled attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? borderBrushCheckedDisabled;

    /// <summary>Gets or sets the value of the CheckBackgroundFillCheckedDisabled attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkBackgroundFillCheckedDisabled;

    /// <summary>Gets or sets the value of the CheckBackgroundStrokeCheckedDisabled attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkBackgroundStrokeCheckedDisabled;

    /// <summary>Gets or sets the value of the CheckGlyphForegroundCheckedDisabled attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkGlyphForegroundCheckedDisabled;

    /// <summary>Gets or sets the value of the CheckGlyphIndeterminate attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static object? checkGlyphIndeterminate;

    /// <summary>Gets or sets the value of the CheckGlyphIndeterminateTemplate attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static DataTemplate? checkGlyphIndeterminateTemplate;

    /// <summary>Gets or sets the value of the ForegroundIndeterminate attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? foregroundIndeterminate;

    /// <summary>Gets or sets the value of the BackgroundIndeterminate attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? backgroundIndeterminate;

    /// <summary>Gets or sets the value of the BorderBrushIndeterminate attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? borderBrushIndeterminate;

    /// <summary>Gets or sets the value of the CheckBackgroundFillIndeterminate attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkBackgroundFillIndeterminate;

    /// <summary>Gets or sets the value of the CheckBackgroundStrokeIndeterminate attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkBackgroundStrokeIndeterminate;

    /// <summary>Gets or sets the value of the CheckGlyphForegroundIndeterminate attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkGlyphForegroundIndeterminate;

    /// <summary>Gets or sets the value of the ForegroundIndeterminateMouseOver attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? foregroundIndeterminateMouseOver;

    /// <summary>Gets or sets the value of the BackgroundIndeterminateMouseOver attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? backgroundIndeterminateMouseOver;

    /// <summary>Gets or sets the value of the BorderBrushIndeterminateMouseOver attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? borderBrushIndeterminateMouseOver;

    /// <summary>Gets or sets the value of the CheckBackgroundFillIndeterminateMouseOver attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkBackgroundFillIndeterminateMouseOver;

    /// <summary>Gets or sets the value of the CheckBackgroundStrokeIndeterminateMouseOver attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkBackgroundStrokeIndeterminateMouseOver;

    /// <summary>Gets or sets the value of the CheckGlyphForegroundIndeterminateMouseOver attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkGlyphForegroundIndeterminateMouseOver;

    /// <summary>Gets or sets the value of the ForegroundIndeterminatePressed attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? foregroundIndeterminatePressed;

    /// <summary>Gets or sets the value of the BackgroundIndeterminatePressed attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? backgroundIndeterminatePressed;

    /// <summary>Gets or sets the value of the BorderBrushIndeterminatePressed attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? borderBrushIndeterminatePressed;

    /// <summary>Gets or sets the value of the CheckBackgroundFillIndeterminatePressed attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkBackgroundFillIndeterminatePressed;

    /// <summary>Gets or sets the value of the CheckBackgroundStrokeIndeterminatePressed attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkBackgroundStrokeIndeterminatePressed;

    /// <summary>Gets or sets the value of the CheckGlyphForegroundIndeterminatePressed attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkGlyphForegroundIndeterminatePressed;

    /// <summary>Gets or sets the value of the ForegroundIndeterminateDisabled attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? foregroundIndeterminateDisabled;

    /// <summary>Gets or sets the value of the BackgroundIndeterminateDisabled attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? backgroundIndeterminateDisabled;

    /// <summary>Gets or sets the value of the BorderBrushIndeterminateDisabled attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? borderBrushIndeterminateDisabled;

    /// <summary>Gets or sets the value of the CheckBackgroundFillIndeterminateDisabled attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkBackgroundFillIndeterminateDisabled;

    /// <summary>Gets or sets the value of the CheckBackgroundStrokeIndeterminateDisabled attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkBackgroundStrokeIndeterminateDisabled;

    /// <summary>Gets or sets the value of the CheckGlyphForegroundIndeterminateDisabled attached property.</summary>
    [AttachedProperty(
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush? checkGlyphForegroundIndeterminateDisabled;
}