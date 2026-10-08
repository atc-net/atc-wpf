namespace Atc.Wpf.Controls.Helpers;

/// <summary>
/// Maps arrow keys to the direction they move in on screen.
/// </summary>
/// <remarks>
/// A right-to-left layout is mirrored, so the Left arrow moves towards the end (and Right towards the start).
/// Controls that handle the horizontal arrow keys themselves pass the key through here first.
/// </remarks>
internal static class FlowDirectionKeyHelper
{
    /// <summary>
    /// Returns the key as it would be in a left-to-right layout: Left and Right are swapped in right-to-left.
    /// </summary>
    /// <param name="key">The pressed key.</param>
    /// <param name="flowDirection">The flow direction of the control.</param>
    /// <returns>The key to handle.</returns>
    public static Key ToLayoutKey(
        Key key,
        FlowDirection flowDirection)
    {
        if (flowDirection != FlowDirection.RightToLeft)
        {
            return key;
        }

        return key switch
        {
            Key.Left => Key.Right,
            Key.Right => Key.Left,
            _ => key,
        };
    }
}