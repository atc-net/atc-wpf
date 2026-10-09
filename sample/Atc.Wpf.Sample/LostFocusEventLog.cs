namespace Atc.Wpf.Sample;

/// <summary>
/// Writes the lost-focus events of a sample's interactive control to a <see cref="TextBlock"/>, one line per event.
/// The UI tests read these lines.
/// </summary>
internal static class LostFocusEventLog
{
    public static EventHandler<ValueChangedEventArgs<T>> Writer<T>(
        TextBlock log,
        string eventName)
        => (_, e) =>
        {
            var line = string.Create(CultureInfo.InvariantCulture, $"{eventName}: {e.OldValue} -> {e.NewValue}");
            log.Text = log.Text.Length == 0
                ? line
                : log.Text + Environment.NewLine + line;
        };
}