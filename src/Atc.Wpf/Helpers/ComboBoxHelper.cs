namespace Atc.Wpf.Helpers;

/// <summary>Provides attached properties for a <see cref="ComboBox"/>, such as character casing and maximum text length.</summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static partial class ComboBoxHelper
{
    /// <summary>Gets or sets the value of the CharacterCasing attached property.</summary>
    [AttachedProperty(
        DefaultValue = CharacterCasing.Normal,
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ValidateValueCallback = nameof(IsValidCharacterCasing),
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static CharacterCasing characterCasing;

    /// <summary>Gets or sets the value of the MaxLength attached property.</summary>
    [AttachedProperty(
        DefaultValue = 0,
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata,
        ValidateValueCallback = nameof(IsValidMaxLength),
        TargetType = typeof(UIElement),
        NullElementHandling = NullElementHandling.None)]
    private static int maxLength;

    private static bool IsValidCharacterCasing(object value)
        => (CharacterCasing)value >= CharacterCasing.Normal && (CharacterCasing)value <= CharacterCasing.Upper;

    private static bool IsValidMaxLength(object value)
        => (int)value >= 0;
}