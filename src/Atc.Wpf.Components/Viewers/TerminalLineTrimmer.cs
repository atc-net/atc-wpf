namespace Atc.Wpf.Components.Viewers;

/// <summary>
/// Enforces the <see cref="TerminalViewer.MaxLines"/> cap on the viewer's line list.
/// </summary>
internal static class TerminalLineTrimmer
{
    /// <summary>
    /// Removes the oldest unpinned lines until <paramref name="items"/> holds at most <paramref name="maxLines"/>
    /// entries. Pinned lines are kept, as they are by Clear, so the list can stay above the cap when only pinned
    /// lines remain. A <paramref name="maxLines"/> of zero or less disables the cap.
    /// </summary>
    public static void TrimToCap(
        IList items,
        int maxLines)
    {
        ArgumentNullException.ThrowIfNull(items);

        if (maxLines <= 0)
        {
            return;
        }

        var index = 0;
        while (items.Count > maxLines && index < items.Count)
        {
            if (items[index] is TerminalLineItem { IsPinned: true })
            {
                index++;
                continue;
            }

            items.RemoveAt(index);
        }
    }
}