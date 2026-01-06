namespace Atc.Wpf.Helpers;

[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class ValidationHelper
{
    public static readonly DependencyProperty ShowValidationErrorOnKeyboardFocusProperty = DependencyProperty.RegisterAttached(
        "ShowValidationErrorOnKeyboardFocus",
        typeof(bool),
        typeof(ValidationHelper),
        new PropertyMetadata(BooleanBoxes.TrueBox));

    public static bool GetShowValidationErrorOnKeyboardFocus(UIElement element)
        => (bool)element.GetValue(ShowValidationErrorOnKeyboardFocusProperty);

    public static void SetShowValidationErrorOnKeyboardFocus(
        UIElement element,
        bool value)
        => element.SetValue(ShowValidationErrorOnKeyboardFocusProperty, BooleanBoxes.Box(value));

    public static readonly DependencyProperty ShowValidationErrorOnMouseOverProperty = DependencyProperty.RegisterAttached(
        "ShowValidationErrorOnMouseOver",
        typeof(bool),
        typeof(ValidationHelper),
        new PropertyMetadata(BooleanBoxes.FalseBox));

    public static bool GetShowValidationErrorOnMouseOver(UIElement element)
        => (bool)element.GetValue(ShowValidationErrorOnMouseOverProperty);

    public static void SetShowValidationErrorOnMouseOver(
        UIElement element,
        bool value)
        => element.SetValue(ShowValidationErrorOnMouseOverProperty, BooleanBoxes.Box(value));
}