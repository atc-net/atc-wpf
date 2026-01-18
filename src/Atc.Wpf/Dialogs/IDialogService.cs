namespace Atc.Wpf.Dialogs;

/// <summary>
/// Service interface for showing dialogs in an MVVM-friendly way.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Shows an information message dialog.
    /// </summary>
    /// <param name="title">The dialog title.</param>
    /// <param name="message">The message to display.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>True if the user clicked OK.</returns>
    Task<bool> ShowInformation(
        string title,
        string message,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Shows a warning message dialog.
    /// </summary>
    /// <param name="title">The dialog title.</param>
    /// <param name="message">The message to display.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>True if the user clicked OK.</returns>
    Task<bool> ShowWarning(
        string title,
        string message,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Shows an error message dialog.
    /// </summary>
    /// <param name="title">The dialog title.</param>
    /// <param name="message">The message to display.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>True if the user clicked OK.</returns>
    Task<bool> ShowError(
        string title,
        string message,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Shows a confirmation dialog with Yes/No buttons.
    /// </summary>
    /// <param name="title">The dialog title.</param>
    /// <param name="message">The question to display.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>True if the user clicked Yes, false if No.</returns>
    Task<bool> ShowConfirmation(
        string title,
        string message,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Shows a confirmation dialog with OK/Cancel buttons.
    /// </summary>
    /// <param name="title">The dialog title.</param>
    /// <param name="message">The message to display.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>True if the user clicked OK, false if Cancel.</returns>
    Task<bool> ShowOkCancel(
        string title,
        string message,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Shows an input dialog for text entry.
    /// </summary>
    /// <param name="title">The dialog title.</param>
    /// <param name="label">The label for the input field.</param>
    /// <param name="defaultValue">The default value for the input field.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The entered text, or null if cancelled.</returns>
    Task<string?> ShowInput(
        string title,
        string label,
        string? defaultValue = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Shows a color picker dialog.
    /// </summary>
    /// <param name="title">The dialog title.</param>
    /// <param name="initialColor">The initial color to display.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The selected color, or null if cancelled.</returns>
    Task<Color?> ShowColorPicker(
        string title,
        Color? initialColor = null,
        CancellationToken cancellationToken = default);
}