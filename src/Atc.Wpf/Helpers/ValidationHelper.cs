namespace Atc.Wpf.Helpers;

/// <summary>Provides attached properties that control when validation errors are shown, such as on keyboard focus or mouse over.</summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class ValidationHelper
{
    /// <summary>Identifies the ShowValidationErrorOnKeyboardFocus attached property.</summary>
    public static readonly DependencyProperty ShowValidationErrorOnKeyboardFocusProperty = DependencyProperty.RegisterAttached(
        "ShowValidationErrorOnKeyboardFocus",
        typeof(bool),
        typeof(ValidationHelper),
        new PropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>Gets the value of the ShowValidationErrorOnKeyboardFocus attached property.</summary>
    public static bool GetShowValidationErrorOnKeyboardFocus(UIElement element)
        => (bool)element.GetValue(ShowValidationErrorOnKeyboardFocusProperty);

    /// <summary>Sets the value of the ShowValidationErrorOnKeyboardFocus attached property.</summary>
    public static void SetShowValidationErrorOnKeyboardFocus(
        UIElement element,
        bool value)
        => element.SetValue(ShowValidationErrorOnKeyboardFocusProperty, BooleanBoxes.Box(value));

    /// <summary>Identifies the ShowValidationErrorOnMouseOver attached property.</summary>
    public static readonly DependencyProperty ShowValidationErrorOnMouseOverProperty = DependencyProperty.RegisterAttached(
        "ShowValidationErrorOnMouseOver",
        typeof(bool),
        typeof(ValidationHelper),
        new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>Gets the value of the ShowValidationErrorOnMouseOver attached property.</summary>
    public static bool GetShowValidationErrorOnMouseOver(UIElement element)
        => (bool)element.GetValue(ShowValidationErrorOnMouseOverProperty);

    /// <summary>Sets the value of the ShowValidationErrorOnMouseOver attached property.</summary>
    public static void SetShowValidationErrorOnMouseOver(
        UIElement element,
        bool value)
        => element.SetValue(ShowValidationErrorOnMouseOverProperty, BooleanBoxes.Box(value));
}