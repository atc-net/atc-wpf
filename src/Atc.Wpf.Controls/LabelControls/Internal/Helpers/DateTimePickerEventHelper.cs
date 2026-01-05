namespace Atc.Wpf.Controls.LabelControls.Internal.Helpers;

/// <summary>
/// Helper class for firing validation events in date/time picker controls.
/// </summary>
internal static class DateTimePickerEventHelper
{
    /// <summary>
    /// Delegate for parsing a string value to a DateTime using a specific culture.
    /// </summary>
    /// <param name="value">The string value to parse.</param>
    /// <param name="cultureInfo">The culture to use for parsing.</param>
    /// <param name="result">The parsed DateTime result.</param>
    /// <returns>True if parsing succeeded; otherwise false.</returns>
    public delegate bool TryParseDateTime(string value, CultureInfo cultureInfo, out DateTime result);

    /// <summary>
    /// Fires the LostFocusValid event for date/time picker controls.
    /// </summary>
    /// <typeparam name="TControl">The type of control.</typeparam>
    /// <param name="control">The control instance.</param>
    /// <param name="e">The dependency property changed event args.</param>
    /// <param name="customCulture">The custom culture, or null to use current UI culture.</param>
    /// <param name="tryParse">The parsing delegate.</param>
    /// <param name="eventHandler">The event handler to invoke.</param>
    public static void FireLostFocusValidEvent<TControl>(
        TControl control,
        DependencyPropertyChangedEventArgs e,
        CultureInfo? customCulture,
        TryParseDateTime tryParse,
        EventHandler<ValueChangedEventArgs<DateTime?>>? eventHandler)
        where TControl : FrameworkElement, ILabelControl
    {
        if (eventHandler is null)
        {
            return;
        }

        var (oldValue, newValue) = ParseOldAndNewValues(e, customCulture, tryParse);

        eventHandler.Invoke(
            control,
            new ValueChangedEventArgs<DateTime?>(
                ControlHelper.GetIdentifier(control),
                oldValue,
                newValue));
    }

    /// <summary>
    /// Fires the LostFocusInvalid event for date/time picker controls.
    /// </summary>
    /// <typeparam name="TControl">The type of control.</typeparam>
    /// <param name="control">The control instance.</param>
    /// <param name="e">The dependency property changed event args.</param>
    /// <param name="customCulture">The custom culture, or null to use current UI culture.</param>
    /// <param name="tryParse">The parsing delegate.</param>
    /// <param name="eventHandler">The event handler to invoke.</param>
    public static void FireLostFocusInvalidEvent<TControl>(
        TControl control,
        DependencyPropertyChangedEventArgs e,
        CultureInfo? customCulture,
        TryParseDateTime tryParse,
        EventHandler<ValueChangedEventArgs<DateTime?>>? eventHandler)
        where TControl : FrameworkElement, ILabelControl
    {
        if (eventHandler is null)
        {
            return;
        }

        var (oldValue, newValue) = ParseOldAndNewValues(e, customCulture, tryParse);

        eventHandler.Invoke(
            control,
            new ValueChangedEventArgs<DateTime?>(
                ControlHelper.GetIdentifier(control),
                oldValue,
                newValue));
    }

    /// <summary>
    /// Coerces a text value to ensure it's never null.
    /// </summary>
    /// <param name="value">The value to coerce.</param>
    /// <returns>The value or empty string if null.</returns>
    public static object CoerceText(object? value)
        => value ?? string.Empty;

    private static (DateTime? OldValue, DateTime? NewValue) ParseOldAndNewValues(
        DependencyPropertyChangedEventArgs e,
        CultureInfo? customCulture,
        TryParseDateTime tryParse)
    {
        var cultureInfo = customCulture ?? Thread.CurrentThread.CurrentUICulture;

        DateTime? oldValue = null;
        if (e.OldValue is not null &&
            tryParse(e.OldValue.ToString()!, cultureInfo, out var resultOld))
        {
            oldValue = resultOld;
        }

        DateTime? newValue = null;
        if (e.NewValue is not null &&
            tryParse(e.NewValue.ToString()!, cultureInfo, out var resultNew))
        {
            newValue = resultNew;
        }

        return (oldValue, newValue);
    }
}
