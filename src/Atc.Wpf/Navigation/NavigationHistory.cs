namespace Atc.Wpf.Navigation;

/// <summary>
/// Maintains navigation history with back/forward support.
/// </summary>
public sealed class NavigationHistory
{
    private readonly Stack<NavigationEntry> backStack = new();
    private readonly Stack<NavigationEntry> forwardStack = new();
    private NavigationEntry? current;

    /// <summary>
    /// Gets a value indicating whether back navigation is available.
    /// </summary>
    public bool CanGoBack => backStack.Count > 0;

    /// <summary>
    /// Gets a value indicating whether forward navigation is available.
    /// </summary>
    public bool CanGoForward => forwardStack.Count > 0;

    /// <summary>
    /// Gets the current navigation entry.
    /// </summary>
    public NavigationEntry? Current => current;

    /// <summary>
    /// Gets the back stack as a read-only collection.
    /// </summary>
    public IReadOnlyCollection<NavigationEntry> BackStack => backStack;

    /// <summary>
    /// Gets the forward stack as a read-only collection.
    /// </summary>
    public IReadOnlyCollection<NavigationEntry> ForwardStack => forwardStack;

    /// <summary>
    /// Navigates to a new entry, adding the current to back stack.
    /// </summary>
    /// <param name="entry">The new navigation entry.</param>
    public void Navigate(NavigationEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        if (current is not null)
        {
            backStack.Push(current);
        }

        current = entry;
        forwardStack.Clear();
    }

    /// <summary>
    /// Goes back to the previous entry.
    /// </summary>
    /// <returns>The previous entry, or null if back stack is empty.</returns>
    public NavigationEntry? GoBack()
    {
        if (!CanGoBack)
        {
            return null;
        }

        if (current is not null)
        {
            forwardStack.Push(current);
        }

        current = backStack.Pop();
        return current;
    }

    /// <summary>
    /// Goes forward to the next entry.
    /// </summary>
    /// <returns>The next entry, or null if forward stack is empty.</returns>
    public NavigationEntry? GoForward()
    {
        if (!CanGoForward)
        {
            return null;
        }

        if (current is not null)
        {
            backStack.Push(current);
        }

        current = forwardStack.Pop();
        return current;
    }

    /// <summary>
    /// Clears all navigation history.
    /// </summary>
    public void Clear()
    {
        backStack.Clear();
        forwardStack.Clear();
        current = null;
    }
}