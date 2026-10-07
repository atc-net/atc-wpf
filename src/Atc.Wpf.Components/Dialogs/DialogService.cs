namespace Atc.Wpf.Components.Dialogs;

/// <summary>
/// Default implementation of <see cref="IDialogService"/> that wraps
/// the existing dialog box controls for MVVM-friendly dialog display.
/// </summary>
/// <remarks>
/// Dialogs are shown on the application dispatcher without blocking a thread-pool thread.
/// Cancelling the token closes an open dialog and cancels the returned task.
/// </remarks>
public class DialogService : IDialogService
{
    private readonly Func<Window?>? ownerResolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogService"/> class.
    /// </summary>
    public DialogService()
        : this(ownerResolver: null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogService"/> class.
    /// </summary>
    /// <param name="ownerResolver">Function to resolve the owner window for dialogs.</param>
    public DialogService(Func<Window?>? ownerResolver)
        => this.ownerResolver = ownerResolver;

    /// <inheritdoc />
    public Task<bool> ShowInformation(
        string title,
        string message,
        CancellationToken cancellationToken = default)
        => ShowDialogAsync(
            Application.Current?.Dispatcher,
            () =>
            {
                var settings = DialogBoxSettings.Create(DialogBoxType.Ok);
                settings.TitleBarText = title;
                settings.ContentSvgImageColor = Colors.DodgerBlue;

                return new InfoDialogBox(GetOwnerWindow(), settings, message);
            },
            (_, dialogResult) => dialogResult == true,
            fallback: false,
            cancellationToken);

    /// <inheritdoc />
    public Task<bool> ShowWarning(
        string title,
        string message,
        CancellationToken cancellationToken = default)
        => ShowDialogAsync(
            Application.Current?.Dispatcher,
            () =>
            {
                var settings = new DialogBoxSettings(
                    DialogBoxType.Ok,
                    LogCategoryType.Warning)
                {
                    TitleBarText = title,
                };

                return new InfoDialogBox(GetOwnerWindow(), settings, message);
            },
            (_, dialogResult) => dialogResult == true,
            fallback: false,
            cancellationToken);

    /// <inheritdoc />
    public Task<bool> ShowError(
        string title,
        string message,
        CancellationToken cancellationToken = default)
        => ShowDialogAsync(
            Application.Current?.Dispatcher,
            () =>
            {
                var settings = new DialogBoxSettings(
                    DialogBoxType.Ok,
                    LogCategoryType.Error)
                {
                    TitleBarText = title,
                };

                return new InfoDialogBox(GetOwnerWindow(), settings, message);
            },
            (_, dialogResult) => dialogResult == true,
            fallback: false,
            cancellationToken);

    /// <inheritdoc />
    public Task<bool> ShowConfirmation(
        string title,
        string message,
        CancellationToken cancellationToken = default)
        => ShowDialogAsync(
            Application.Current?.Dispatcher,
            () => new QuestionDialogBox(GetOwnerWindow(), title, message),
            (_, dialogResult) => dialogResult == true,
            fallback: false,
            cancellationToken);

    /// <inheritdoc />
    public Task<bool> ShowOkCancel(
        string title,
        string message,
        CancellationToken cancellationToken = default)
        => ShowDialogAsync(
            Application.Current?.Dispatcher,
            () =>
            {
                var settings = DialogBoxSettings.Create(DialogBoxType.OkCancel);
                settings.TitleBarText = title;

                return new QuestionDialogBox(GetOwnerWindow(), settings, message);
            },
            (_, dialogResult) => dialogResult == true,
            fallback: false,
            cancellationToken);

    /// <inheritdoc />
    public Task<string?> ShowInput(
        string title,
        string label,
        string? defaultValue = null,
        CancellationToken cancellationToken = default)
    {
        LabelTextBox? labelTextBox = null;

        return ShowDialogAsync(
            Application.Current?.Dispatcher,
            () =>
            {
                labelTextBox = new LabelTextBox
                {
                    LabelText = label,
                    Text = defaultValue ?? string.Empty,
                };

                return new InputDialogBox(GetOwnerWindow(), title, labelTextBox);
            },
            (_, dialogResult) => dialogResult == true
                ? labelTextBox?.Text
                : null,
            fallback: null,
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<Color?> ShowColorPicker(
        string title,
        Color? initialColor = null,
        CancellationToken cancellationToken = default)
        => ShowDialogAsync<Color?>(
            Application.Current?.Dispatcher,
            () =>
            {
                var settings = DialogBoxSettings.Create(DialogBoxType.OkCancel);
                settings.TitleBarText = title;
                settings.Width = 770;
                settings.Height = 700;

                return new ColorPickerDialogBox(GetOwnerWindow(), settings, initialColor ?? Colors.Black);
            },
            (dialog, dialogResult) => dialogResult == true
                ? ((ColorPickerDialogBox)dialog).Color
                : null,
            fallback: null,
            cancellationToken);

    /// <summary>
    /// Shows a modal dialog on <paramref name="dispatcher"/> and maps its outcome.
    /// </summary>
    /// <remarks>
    /// The dialog runs on the dispatcher's own message loop, so no thread-pool thread is held while it is open.
    /// Cancelling <paramref name="cancellationToken"/> closes an open dialog and cancels the returned task;
    /// a token that is already cancelled prevents the dialog from being created.
    /// </remarks>
    /// <returns><paramref name="fallback"/> when there is no usable dispatcher; otherwise the mapped dialog result.</returns>
    internal static async Task<T> ShowDialogAsync<T>(
        Dispatcher? dispatcher,
        Func<Window> createDialog,
        Func<Window, bool?, T> mapResult,
        T fallback,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (dispatcher is not { HasShutdownStarted: false })
        {
            return fallback;
        }

        return await dispatcher
            .InvokeAsync(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                var dialog = createDialog();
                using var registration = cancellationToken.Register(
                    () => _ = dispatcher.BeginInvoke(() =>
                    {
                        if (dialog.IsVisible)
                        {
                            dialog.Close();
                        }
                    }));

                var dialogResult = dialog.ShowDialog();

                cancellationToken.ThrowIfCancellationRequested();
                return mapResult(dialog, dialogResult);
            })
            .Task
            .ConfigureAwait(false);
    }

    private Window GetOwnerWindow()
    {
        var owner = ownerResolver?.Invoke();
        return owner ?? Application.Current.MainWindow ?? throw new InvalidOperationException("No owner window available.");
    }
}