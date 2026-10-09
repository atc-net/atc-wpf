namespace Atc.Wpf.Helpers;

/// <summary>Provides attached properties for a <see cref="ComboBox"/>, such as character casing and maximum text length.</summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class ComboBoxHelper
{
    /// <summary>Identifies the CharacterCasing attached property.</summary>
    public static readonly DependencyProperty CharacterCasingProperty = DependencyProperty.RegisterAttached(
        "CharacterCasing",
        typeof(CharacterCasing),
        typeof(ComboBoxHelper),
        new FrameworkPropertyMetadata(CharacterCasing.Normal),
        value => (CharacterCasing)value >= CharacterCasing.Normal && (CharacterCasing)value <= CharacterCasing.Upper);

    /// <summary>Gets the value of the CharacterCasing attached property.</summary>
    public static CharacterCasing GetCharacterCasing(UIElement element)
        => (CharacterCasing)element.GetValue(CharacterCasingProperty);

    /// <summary>Sets the value of the CharacterCasing attached property.</summary>
    public static void SetCharacterCasing(
        UIElement element,
        CharacterCasing value)
        => element.SetValue(CharacterCasingProperty, value);

    /// <summary>Identifies the MaxLength attached property.</summary>
    public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.RegisterAttached(
        "MaxLength",
        typeof(int),
        typeof(ComboBoxHelper),
        new FrameworkPropertyMetadata(0),
        value => (int)value >= 0);

    /// <summary>Gets the value of the MaxLength attached property.</summary>
    public static int GetMaxLength(UIElement element)
        => (int)element.GetValue(MaxLengthProperty);

    /// <summary>Sets the value of the MaxLength attached property.</summary>
    public static void SetMaxLength(
        UIElement element,
        int value)
        => element.SetValue(MaxLengthProperty, value);
}