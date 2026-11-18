namespace Atc.Wpf.Helpers;

/// <summary>Provides attached properties that control when validation errors are shown, such as on keyboard focus or mouse over.</summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static partial class ValidationHelper
{
    /// <summary>Gets or sets the value of the ShowValidationErrorOnKeyboardFocus attached property.</summary>
    [AttachedProperty(DefaultValue = true, TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static bool showValidationErrorOnKeyboardFocus;

    /// <summary>Gets or sets the value of the ShowValidationErrorOnMouseOver attached property.</summary>
    [AttachedProperty(DefaultValue = false, TargetType = typeof(UIElement), NullElementHandling = NullElementHandling.None)]
    private static bool showValidationErrorOnMouseOver;
}