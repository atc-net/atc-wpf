namespace Atc.Wpf.Components.Dialogs;

/// <summary>
/// An <see cref="IDialogService"/> that shows the dialogs inside the window, as centred modal flyouts of an
/// <see cref="IFlyoutService"/>, instead of as separate windows. The dialogs look and answer like the windowed
/// ones (<see cref="DialogService"/>): same buttons, icons and results.
/// </summary>
/// <remarks>
/// When the flyout service has no host (a <see cref="FlyoutService"/> without
/// <see cref="FlyoutService.SetFlyoutHost"/> or <see cref="FlyoutService.SetHostPanel"/>), the dialogs are shown
/// by the fallback service as windows, and one warning is written to <see cref="System.Diagnostics.Trace"/>.
/// </remarks>
public class InWindowDialogService : IDialogService
{
    private const double MessageWidth = 500;

    private readonly IFlyoutService flyoutService;
    private readonly IDialogService fallback;
    private bool isFallbackTraced;

    /// <summary>
    /// Initializes a new instance of the <see cref="InWindowDialogService"/> class that falls back to a
    /// <see cref="DialogService"/> when the flyout service has no host.
    /// </summary>
    /// <param name="flyoutService">The flyout service of the window the dialogs are shown in.</param>
    public InWindowDialogService(IFlyoutService flyoutService)
        : this(flyoutService, new DialogService())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InWindowDialogService"/> class.
    /// </summary>
    /// <param name="flyoutService">The flyout service of the window the dialogs are shown in.</param>
    /// <param name="fallback">The service that shows the dialogs when the flyout service has no host.</param>
    public InWindowDialogService(
        IFlyoutService flyoutService,
        IDialogService fallback)
    {
        this.flyoutService = flyoutService ?? throw new ArgumentNullException(nameof(flyoutService));
        this.fallback = fallback ?? throw new ArgumentNullException(nameof(fallback));
    }

    /// <inheritdoc />
    public Task<bool> ShowInformation(
        string title,
        string message,
        CancellationToken cancellationToken = default)
        => Show(
            () => ShowMessage(title, message, DialogService.CreateInformationSettings(title), cancellationToken),
            () => fallback.ShowInformation(title, message, cancellationToken),
            cancellationToken);

    /// <inheritdoc />
    public Task<bool> ShowWarning(
        string title,
        string message,
        CancellationToken cancellationToken = default)
        => Show(
            () => ShowMessage(title, message, DialogService.CreateWarningSettings(title), cancellationToken),
            () => fallback.ShowWarning(title, message, cancellationToken),
            cancellationToken);

    /// <inheritdoc />
    public Task<bool> ShowError(
        string title,
        string message,
        CancellationToken cancellationToken = default)
        => Show(
            () => ShowMessage(title, message, DialogService.CreateErrorSettings(title), cancellationToken),
            () => fallback.ShowError(title, message, cancellationToken),
            cancellationToken);

    /// <inheritdoc />
    public Task<bool> ShowConfirmation(
        string title,
        string message,
        CancellationToken cancellationToken = default)
        => Show(
            () => ShowMessage(title, message, DialogService.CreateConfirmationSettings(title), cancellationToken),
            () => fallback.ShowConfirmation(title, message, cancellationToken),
            cancellationToken);

    /// <summary>
    /// Shows a confirmation dialog inside the window with the given settings: its title, button texts and
    /// default button, for example <see cref="DialogBoxButton.Negative"/> so that Enter answers No to a question
    /// that deletes something.
    /// </summary>
    /// <param name="settings">The dialog settings; <see cref="DialogBoxSettings.TitleBarText"/> is the title.</param>
    /// <param name="message">The question.</param>
    /// <param name="cancellationToken">Optional cancellation token. Cancelling it closes the dialog if it is open and cancels the returned task.</param>
    /// <returns>True if the user answered with the affirmative button, false otherwise.</returns>
    public Task<bool> ShowConfirmation(
        DialogBoxSettings settings,
        string message,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);

        return Show(
            () => ShowMessage(settings.TitleBarText, message, settings, cancellationToken),
            () => WindowedFallback().ShowConfirmation(settings, message, cancellationToken),
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> ShowOkCancel(
        string title,
        string message,
        CancellationToken cancellationToken = default)
        => Show(
            () => ShowMessage(title, message, DialogService.CreateOkCancelSettings(title), cancellationToken),
            () => fallback.ShowOkCancel(title, message, cancellationToken),
            cancellationToken);

    /// <inheritdoc />
    public Task<string?> ShowInput(
        string title,
        string label,
        string? defaultValue = null,
        CancellationToken cancellationToken = default)
        => Show(
            () =>
            {
                var textBox = new LabelTextBox
                {
                    LabelText = label,
                    Text = defaultValue ?? string.Empty,
                    Orientation = Orientation.Vertical,
                };

                return ShowInWindow(
                    title,
                    MessageWidth,
                    textBox,
                    DialogService.CreateOkCancelSettings(title),
                    button => button == DialogBoxButton.Affirmative ? textBox.Text : null,
                    cancelled: null,
                    textBox.IsValid,
                    () => FindDescendant<TextBox>(textBox),
                    cancellationToken);
            },
            () => fallback.ShowInput(title, label, defaultValue, cancellationToken),
            cancellationToken);

    /// <inheritdoc />
    public Task<Color?> ShowColorPicker(
        string title,
        Color? initialColor = null,
        CancellationToken cancellationToken = default)
        => Show(
            () =>
            {
                var picker = new Atc.Wpf.Forms.ColorEditing.AdvancedColorPicker { OriginalColor = initialColor ?? Colors.Black };
                var settings = DialogService.CreateColorPickerSettings(title);

                return ShowInWindow<Color?>(
                    title,
                    settings.Width,
                    picker,
                    settings,
                    button => button == DialogBoxButton.Affirmative ? picker.Color : null,
                    cancelled: null,
                    canAccept: null,
                    initialFocus: null,
                    cancellationToken);
            },
            () => fallback.ShowColorPicker(title, initialColor, cancellationToken),
            cancellationToken);

    /// <summary>
    /// Shows a font picker inside the window.
    /// </summary>
    /// <param name="title">The dialog title.</param>
    /// <param name="initialFont">The font selected when the dialog opens; the default font when omitted.</param>
    /// <param name="cancellationToken">Optional cancellation token. Cancelling it closes the dialog if it is open and cancels the returned task.</param>
    /// <returns>The selected font, or null if cancelled.</returns>
    public Task<FontDescription?> ShowFontPicker(
        string title,
        FontDescription? initialFont = null,
        CancellationToken cancellationToken = default)
        => Show(
            () =>
            {
                var picker = new AdvancedFontPicker();
                picker.SetFontDescription(initialFont ?? new FontDescription());
                var settings = DialogService.CreateFontPickerSettings(title);

                return ShowInWindow(
                    title,
                    settings.Width,
                    picker,
                    settings,
                    button =>
                    {
                        if (button != DialogBoxButton.Affirmative)
                        {
                            return null;
                        }

                        picker.RecordCurrentFamilyAsRecent();
                        return picker.GetFontDescription();
                    },
                    cancelled: null,
                    canAccept: null,
                    initialFocus: null,
                    cancellationToken);
            },
            () => WindowedFallback().ShowFontPicker(title, initialFont, cancellationToken),
            cancellationToken);

    /// <summary>
    /// Shows an input form inside the window; OK closes it only when the form is valid.
    /// </summary>
    /// <param name="title">The dialog title.</param>
    /// <param name="form">The form to show; its controls hold the entered values afterwards.</param>
    /// <param name="cancellationToken">Optional cancellation token. Cancelling it closes the dialog if it is open and cancels the returned task.</param>
    /// <returns>True if the user confirmed a valid form, false otherwise.</returns>
    public Task<bool> ShowInputForm(
        string title,
        ILabelControlsForm form,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(form);

        return Show(
            () =>
            {
                var settings = DialogService.CreateOkCancelSettings(title);
                settings.Form.UseGroupBox = form.HasMultiGroupIdentifiers();
                var panel = new LabelInputFormPanel();
                panel.Render(settings.Form, form);

                // The form's own width, the scroll bar and the flyout's padding.
                var width = form.GetMaxWidth() + 20 + 32;

                return ShowInWindow(
                    title,
                    width,
                    panel,
                    settings,
                    button => button == DialogBoxButton.Affirmative,
                    cancelled: false,
                    form.IsValid,
                    initialFocus: null,
                    cancellationToken);
            },
            () => WindowedFallback().ShowInputForm(title, form, cancellationToken),
            cancellationToken);
    }

    private bool CanShowInWindow
        => flyoutService is not FlyoutService { IsHostConfigured: false };

    private static T? FindDescendant<T>(DependencyObject root)
        where T : DependencyObject
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
        {
            var child = VisualTreeHelper.GetChild(root, i);
            if (child is T match)
            {
                return match;
            }

            if (FindDescendant<T>(child) is { } found)
            {
                return found;
            }
        }

        return null;
    }

    private static FlyoutOptions CreateOptions(double width)
        => new()
        {
            Position = FlyoutPosition.Center,
            Width = width,
            Height = double.NaN,
            CornerRadius = new CornerRadius(8),
            IsLightDismissEnabled = false,
            ShowOverlay = true,
            ShowCloseButton = true,
            CloseOnEscape = true,
            IsPinned = false,
            IsResizable = false,
            IsFocusTrapEnabled = true,
        };

    private Task<T> Show<T>(
        Func<Task<T>> showInWindow,
        Func<Task<T>> showWindowed,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled<T>(cancellationToken);
        }

        if (!CanShowInWindow)
        {
            TraceFallback();
            return showWindowed();
        }

        var dispatcher = Application.Current?.Dispatcher;
        return dispatcher is null || dispatcher.CheckAccess()
            ? showInWindow()
            : dispatcher.InvokeAsync(showInWindow).Task.Unwrap();
    }

    private Task<bool> ShowMessage(
        string title,
        string message,
        DialogBoxSettings settings,
        CancellationToken cancellationToken)
        => ShowInWindow(
            title,
            MessageWidth,
            Helpers.DialogBoxHelper.CreateContentControl(message, settings.ContentSvgImage, settings),
            settings,
            button => button == DialogBoxButton.Affirmative,
            cancelled: false,
            canAccept: null,
            initialFocus: null,
            cancellationToken);

    private async Task<T> ShowInWindow<T>(
        string title,
        double width,
        FrameworkElement body,
        DialogBoxSettings settings,
        Func<DialogBoxButton, T> mapButton,
        T cancelled,
        Func<bool>? canAccept,
        Func<IInputElement?>? initialFocus,
        CancellationToken cancellationToken)
    {
        // The answer is taken when a button is clicked (on the UI thread, while the controls are there);
        // Escape and the close button give no answer, which cancels.
        var content = new InWindowDialogContent(
            body,
            settings,
            button => new Answer<T>(mapButton(button)),
            canAccept,
            initialFocus);

        var result = await flyoutService
            .ShowAsync(title, content, CreateOptions(width), cancellationToken)
            .ConfigureAwait(false);

        return result is Answer<T> answer
            ? answer.Value
            : cancelled;
    }

    private sealed record Answer<T>(T Value);

    private DialogService WindowedFallback()
        => fallback as DialogService ?? new DialogService();

    private void TraceFallback()
    {
        if (isFallbackTraced)
        {
            return;
        }

        isFallbackTraced = true;
        System.Diagnostics.Trace.TraceWarning(
            $"{nameof(InWindowDialogService)}: the flyout service has no host, so dialogs are shown as windows. " +
            $"Call {nameof(FlyoutService)}.{nameof(FlyoutService.SetFlyoutHost)} or {nameof(FlyoutService.SetHostPanel)} to show them in the window.");
    }
}