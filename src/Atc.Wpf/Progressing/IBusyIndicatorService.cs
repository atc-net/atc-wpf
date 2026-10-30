namespace Atc.Wpf.Progressing;

/// <summary>
/// Service interface for managing busy overlays in an MVVM-friendly way.
/// Supports global and region-scoped busy states, progress reporting, and cancellation.
/// </summary>
public interface IBusyIndicatorService
{
    /// <summary>
    /// Shows a busy overlay.
    /// </summary>
    /// <param name="message">Optional message to display.</param>
    /// <param name="regionName">Optional region name for targeting a specific BusyOverlay.</param>
    /// <param name="allowCancellation">If true, a Cancel button is shown.</param>
    /// <returns>A token that can be used to hide or report progress.</returns>
    BusyToken Show(
        string message = "",
        string regionName = "",
        bool allowCancellation = false);

    /// <summary>
    /// Hides the busy overlay for the given token.
    /// </summary>
    /// <param name="token">The token returned from <see cref="Show"/>.</param>
    void Hide(BusyToken token);

    /// <summary>
    /// Hides all busy overlays for a given region.
    /// </summary>
    /// <param name="regionName">The region name, or empty string for the default region.</param>
    void HideAll(string regionName = "");

    /// <summary>
    /// Reports progress for an active busy indicator session.
    /// </summary>
    /// <param name="token">The token returned from <see cref="Show"/>.</param>
    /// <param name="info">The progress information to display.</param>
    void Report(
        BusyToken token,
        BusyInfo info);

    /// <summary>
    /// Runs an async operation with automatic busy overlay lifecycle.
    /// </summary>
    /// <remarks>
    /// When the user presses the Cancel button, the <see cref="OperationCanceledException"/> it causes is
    /// swallowed and the task completes. Any other exception, including an <see cref="OperationCanceledException"/>
    /// from another token (for example an HTTP timeout), reaches the caller. The overlay is hidden in both cases.
    /// </remarks>
    /// <param name="operation">The async operation receiving a progress reporter and cancellation token.</param>
    /// <param name="message">Optional message to display.</param>
    /// <param name="regionName">Optional region name for targeting a specific BusyOverlay.</param>
    /// <param name="allowCancellation">If true, a Cancel button is shown.</param>
    Task RunAsync(
        Func<IProgress<BusyInfo>, CancellationToken, Task> operation,
        string message = "",
        string regionName = "",
        bool allowCancellation = false);

    /// <summary>
    /// Runs an async operation with automatic busy overlay lifecycle and a return value.
    /// </summary>
    /// <remarks>
    /// When the user presses the Cancel button, the <see cref="OperationCanceledException"/> it causes is
    /// swallowed and the task returns <see langword="default"/>. Any other exception, including an
    /// <see cref="OperationCanceledException"/> from another token (for example an HTTP timeout), reaches the
    /// caller. The overlay is hidden in both cases.
    /// </remarks>
    /// <typeparam name="T">The return type of the operation.</typeparam>
    /// <param name="operation">The async operation receiving a progress reporter and cancellation token.</param>
    /// <param name="message">Optional message to display.</param>
    /// <param name="regionName">Optional region name for targeting a specific BusyOverlay.</param>
    /// <param name="allowCancellation">If true, a Cancel button is shown.</param>
    /// <returns>The result of the operation, or <see langword="default"/> when the user cancelled it.</returns>
    Task<T> RunAsync<T>(
        Func<IProgress<BusyInfo>, CancellationToken, Task<T>> operation,
        string message = "",
        string regionName = "",
        bool allowCancellation = false);
}