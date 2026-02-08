namespace Atc.Wpf.Progressing;

/// <summary>
/// Represents an active busy indicator session.
/// Returned by <see cref="IBusyIndicatorService.Show"/> and used to hide or report progress.
/// </summary>
public sealed class BusyToken
{
    /// <summary>
    /// Gets the unique identifier for this token.
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    /// Gets the region name this token is associated with.
    /// </summary>
    public string RegionName { get; init; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether this token's operation was cancelled.
    /// </summary>
    public bool IsCancelled { get; set; }
}