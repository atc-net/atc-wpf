namespace Atc.Wpf.Helpers;

/// <summary>
/// Class used to manage generic scoping of access keys.
/// </summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class AccessKeyHelper
{
    /// <summary>
    /// Identifies the IsAccessKeyScope attached dependency property.
    /// </summary>
    public static readonly DependencyProperty IsAccessKeyScopeProperty = DependencyProperty.RegisterAttached(
        "IsAccessKeyScope",
        typeof(bool),
        typeof(AccessKeyHelper),
        new PropertyMetadata(
            BooleanBoxes.FalseBox,
            HandleIsAccessKeyScopePropertyChanged));

    /// <summary>
    /// Sets the IsAccessKeyScope attached property value for the specified object
    /// </summary>
    /// <param name="d">The object to retrieve the value for</param>
    /// <param name="value">Whether the object is an access key scope</param>
    public static void SetIsAccessKeyScope(
        DependencyObject d,
        bool value)
        => d.SetValue(IsAccessKeyScopeProperty, BooleanBoxes.Box(value));

    /// <summary>
    /// Gets the value of the IsAccessKeyScope attached property for the specified object
    /// </summary>
    /// <param name="d">The object to retrieve the value for</param>
    /// <returns>The value of IsAccessKeyScope attached property for the specified object</returns>
    public static bool GetIsAccessKeyScope(
        DependencyObject d)
        => (bool)d.GetValue(IsAccessKeyScopeProperty);

    private static void HandleIsAccessKeyScopePropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue.Equals(true))
        {
            AccessKeyManager.AddAccessKeyPressedHandler(d, HandleScopedElementAccessKeyPressed);
        }
        else
        {
            AccessKeyManager.RemoveAccessKeyPressedHandler(d, HandleScopedElementAccessKeyPressed);
        }
    }

    private static void HandleScopedElementAccessKeyPressed(
        object sender,
        AccessKeyPressedEventArgs e)
    {
        if (e.Handled ||
            sender is not DependencyObject dependencyObject ||
            !GetIsAccessKeyScope(dependencyObject))
        {
            return;
        }

        e.Scope = sender;
        e.Handled = true;
    }
}