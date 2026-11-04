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
    /// Shows a font picker dialog.
    /// </summary>
    /// <remarks>
    /// Not part of <see cref="IDialogService"/>, which lives in Atc.Wpf and cannot refer to
    /// <see cref="FontDescription"/>.
    /// </remarks>
    /// <param name="title">The dialog title.</param>
    /// <param name="initialFont">The font selected when the dialog opens; the default font when omitted.</param>
    /// <param name="cancellationToken">Optional cancellation token. Cancelling it closes the dialog if it is open and cancels the returned task.</param>
    /// <returns>The selected font, or null if cancelled.</returns>
    public Task<FontDescription?> ShowFontPicker(
        string title,
        FontDescription? initialFont = null,
        CancellationToken cancellationToken = default)
        => ShowDialogAsync<FontDescription?>(
            Application.Current?.Dispatcher,
            () =>
            {
                var settings = DialogBoxSettings.Create(DialogBoxType.OkCancel);
                settings.TitleBarText = title;
                settings.Width = 925;
                settings.Height = 720;

                return new FontPickerDialogBox(GetOwnerWindow(), settings, initialFont ?? new FontDescription());
            },
            (dialog, dialogResult) => dialogResult == true
                ? ((FontPickerDialogBox)dialog).SelectedFontDescription
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

    /// <summary>
    /// The owner for a dialog: the window from the owner resolver when there is one, otherwise the application's
    /// active window (so a dialog raised from a second window is owned by and centred on that window), otherwise
    /// the main window.
    /// </summary>
    /// <param name="resolvedOwner">The window returned by the owner resolver, if any.</param>
    /// <param name="openWindows">The application's open windows.</param>
    /// <param name="mainWindow">The application's main window, if any.</param>
    /// <param name="isActive">Whether a window is the active window.</param>
    /// <returns>The owner window, or <see langword="null"/> when there is none.</returns>
    internal static Window? ResolveOwnerWindow(
        Window? resolvedOwner,
        IEnumerable<Window> openWindows,
        Window? mainWindow,
        Func<Window, bool> isActive)
        => resolvedOwner
           ?? openWindows.FirstOrDefault(window => window.IsVisible && isActive(window))
           ?? mainWindow;

    private Window GetOwnerWindow()
    {
        var application = Application.Current;
        var openWindows = application?.Windows.OfType<Window>() ?? [];

        return ResolveOwnerWindow(
                   ownerResolver?.Invoke(),
                   openWindows,
                   application?.MainWindow,
                   window => window.IsActive)
               ?? throw new InvalidOperationException("No owner window available.");
    }
}