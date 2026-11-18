namespace Atc.Wpf.Helpers;

/// <summary>
/// A helper class that provides various controls.
/// </summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static partial class ControlsHelper
{
    /// <summary>Gets or sets the value of the DisabledVisualElementVisibility attached property.</summary>
    [AttachedProperty(
        DefaultValue = Visibility.Visible,
        Flags = FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static Visibility disabledVisualElementVisibility;

    /// <summary>Gets or sets the value of the ContentCharacterCasing attached property.</summary>
    [AttachedProperty(
        DefaultValue = CharacterCasing.Normal,
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure,
        ValidateValueCallback = nameof(IsValidContentCharacterCasing),
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static CharacterCasing contentCharacterCasing;

    private static bool IsValidContentCharacterCasing(object value)
        => (CharacterCasing)value >= CharacterCasing.Normal && (CharacterCasing)value <= CharacterCasing.Upper;

    /// <summary>Gets or sets the value of the RecognizesAccessKey attached property.</summary>
    [AttachedProperty(
        DefaultValue = true,
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static bool recognizesAccessKey;

    /// <summary>Gets or sets the value of the FocusBorderBrush attached property.</summary>
    [AttachedProperty(
        DefaultValueExpression = "Brushes.Transparent",
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush focusBorderBrush;

    /// <summary>Gets or sets the value of the FocusBorderThickness attached property.</summary>
    [AttachedProperty(
        DefaultValueExpression = "default(Thickness)",
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Thickness focusBorderThickness;

    /// <summary>Gets or sets the value of the MouseOverBackgroundBrush attached property.</summary>
    [AttachedProperty(
        DefaultValueExpression = "Brushes.Transparent",
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush mouseOverBackgroundBrush;

    /// <summary>Gets or sets the value of the MouseOverForegroundBrush attached property.</summary>
    [AttachedProperty(
        DefaultValueExpression = "Brushes.Transparent",
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush mouseOverForegroundBrush;

    /// <summary>Gets or sets the value of the MouseOverBorderBrush attached property.</summary>
    [AttachedProperty(
        DefaultValueExpression = "Brushes.Transparent",
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush mouseOverBorderBrush;

    /// <summary>Gets or sets the value of the PressedBackgroundBrush attached property.</summary>
    [AttachedProperty(
        DefaultValueExpression = "Brushes.Transparent",
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush pressedBackgroundBrush;

    /// <summary>Gets or sets the value of the PressedBorderBrush attached property.</summary>
    [AttachedProperty(
        DefaultValueExpression = "Brushes.Transparent",
        Flags = FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits,
        ElementParameterName = "d",
        NullElementHandling = NullElementHandling.None)]
    private static Brush pressedBorderBrush;

    /// <summary>Gets or sets the value of the CornerRadius attached property.</summary>
    [AttachedProperty(
        DefaultValueExpression = "default(CornerRadius)",
        Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static CornerRadius cornerRadius;

    /// <summary>Gets or sets the value of the IsReadOnly attached property.</summary>
    [AttachedProperty(
        DefaultValue = false,
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static bool isReadOnly;
}