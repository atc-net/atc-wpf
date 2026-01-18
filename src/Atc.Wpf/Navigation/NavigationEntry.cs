namespace Atc.Wpf.Navigation;

/// <summary>
/// Represents an entry in the navigation history.
/// </summary>
public sealed class NavigationEntry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationEntry"/> class.
    /// </summary>
    /// <param name="viewModelType">The type of the ViewModel.</param>
    /// <param name="parameters">The navigation parameters.</param>
    public NavigationEntry(
        Type viewModelType,
        NavigationParameters? parameters = null)
    {
        ViewModelType = viewModelType ?? throw new ArgumentNullException(nameof(viewModelType));
        Parameters = parameters ?? new NavigationParameters();
        Timestamp = DateTimeOffset.Now;
    }

    /// <summary>
    /// Gets the type of the ViewModel for this navigation entry.
    /// </summary>
    public Type ViewModelType { get; }

    /// <summary>
    /// Gets the navigation parameters for this entry.
    /// </summary>
    public NavigationParameters Parameters { get; }

    /// <summary>
    /// Gets the timestamp when this navigation occurred.
    /// </summary>
    public DateTimeOffset Timestamp { get; }
}