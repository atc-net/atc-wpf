namespace Atc.Wpf.Helpers;

[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class ComboBoxHelper
{
    public static readonly DependencyProperty CharacterCasingProperty = DependencyProperty.RegisterAttached(
        "CharacterCasing",
        typeof(CharacterCasing),
        typeof(ComboBoxHelper),
        new FrameworkPropertyMetadata(CharacterCasing.Normal),
        value => (CharacterCasing)value >= CharacterCasing.Normal && (CharacterCasing)value <= CharacterCasing.Upper);

    public static CharacterCasing GetCharacterCasing(
        UIElement element)
        => (CharacterCasing)element.GetValue(CharacterCasingProperty);

    public static void SetCharacterCasing(
        UIElement element,
        CharacterCasing value)
        => element.SetValue(CharacterCasingProperty, value);

    public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.RegisterAttached(
        "MaxLength",
        typeof(int),
        typeof(ComboBoxHelper),
        new FrameworkPropertyMetadata(0),
        value => (int)value >= 0);

    public static int GetMaxLength(
        UIElement element)
        => (int)element.GetValue(MaxLengthProperty);

    public static void SetMaxLength(
        UIElement element,
        int value)
        => element.SetValue(MaxLengthProperty, value);
}