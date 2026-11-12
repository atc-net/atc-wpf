namespace Atc.Wpf.Network.Scanner;

/// <summary>
/// Provides data for the <see cref="NetworkScannerViewModel.ScanCompleted"/> event.
/// </summary>
/// <param name="scanResults">The results, or <c>null</c> when the scan was cancelled or failed before it returned.</param>
/// <param name="isCancelled">Whether the scan was cancelled.</param>
/// <param name="errorMessage">The error message when the scan failed; otherwise <c>null</c>.</param>
public sealed class NetworkScanCompletedEventArgs(
    IPScanResults? scanResults,
    bool isCancelled,
    string? errorMessage)
    : EventArgs
{
    /// <summary>
    /// Gets the results, or <c>null</c> when the scan was cancelled or failed before it returned.
    /// </summary>
    public IPScanResults? ScanResults { get; } = scanResults;

    /// <summary>
    /// Gets a value indicating whether the scan was cancelled.
    /// </summary>
    public bool IsCancelled { get; } = isCancelled;

    /// <summary>
    /// Gets the error message when the scan failed; otherwise <c>null</c>.
    /// </summary>
    public string? ErrorMessage { get; } = errorMessage;
}