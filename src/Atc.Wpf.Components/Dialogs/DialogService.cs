namespace Atc.Wpf.Components.Dialogs;

/// <summary>
/// Default implementation of <see cref="IDialogService"/> that wraps
/// the existing dialog box controls for MVVM-friendly dialog display.
/// </summary>
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
        => Task.Run(
            () =>
            {
                if (Application.Current?.Dispatcher is not { HasShutdownStarted: false } dispatcher)
                {
                    return false;
                }

                var result = false;
                dispatcher.Invoke(() =>
                {
                    var owner = GetOwnerWindow();
                    var settings = DialogBoxSettings.Create(DialogBoxType.Ok);
                    settings.TitleBarText = title;
                    settings.ContentSvgImageColor = Colors.DodgerBlue;

                    var dialog = new InfoDialogBox(owner, settings, message);
                    result = dialog.ShowDialog() == true;
                });
                return result;
            },
            cancellationToken);

    /// <inheritdoc />
    public Task<bool> ShowWarning(
        string title,
        string message,
        CancellationToken cancellationToken = default)
        => Task.Run(
            () =>
            {
                if (Application.Current?.Dispatcher is not { HasShutdownStarted: false } dispatcher)
                {
                    return false;
                }

                var result = false;
                dispatcher.Invoke(() =>
                {
                    var owner = GetOwnerWindow();
                    var settings = new DialogBoxSettings(
                        DialogBoxType.Ok,
                        LogCategoryType.Warning)
                    {
                        TitleBarText = title,
                    };

                    var dialog = new InfoDialogBox(owner, settings, message);
                    result = dialog.ShowDialog() == true;
                });
                return result;
            },
            cancellationToken);

    /// <inheritdoc />
    public Task<bool> ShowError(
        string title,
        string message,
        CancellationToken cancellationToken = default)
        => Task.Run(
            () =>
            {
                if (Application.Current?.Dispatcher is not { HasShutdownStarted: false } dispatcher)
                {
                    return false;
                }

                var result = false;
                dispatcher.Invoke(() =>
                {
                    var owner = GetOwnerWindow();
                    var settings = new DialogBoxSettings(
                        DialogBoxType.Ok,
                        LogCategoryType.Error)
                    {
                        TitleBarText = title,
                    };

                    var dialog = new InfoDialogBox(owner, settings, message);
                    result = dialog.ShowDialog() == true;
                });
                return result;
            },
            cancellationToken);

    /// <inheritdoc />
    public Task<bool> ShowConfirmation(
        string title,
        string message,
        CancellationToken cancellationToken = default)
        => Task.Run(
            () =>
            {
                if (Application.Current?.Dispatcher is not { HasShutdownStarted: false } dispatcher)
                {
                    return false;
                }

                var result = false;
                dispatcher.Invoke(() =>
                {
                    var owner = GetOwnerWindow();
                    var dialog = new QuestionDialogBox(owner, title, message);
                    result = dialog.ShowDialog() == true;
                });
                return result;
            },
            cancellationToken);

    /// <inheritdoc />
    public Task<bool> ShowOkCancel(
        string title,
        string message,
        CancellationToken cancellationToken = default)
        => Task.Run(
            () =>
            {
                if (Application.Current?.Dispatcher is not { HasShutdownStarted: false } dispatcher)
                {
                    return false;
                }

                var result = false;
                dispatcher.Invoke(() =>
                {
                    var owner = GetOwnerWindow();
                    var settings = DialogBoxSettings.Create(DialogBoxType.OkCancel);
                    settings.TitleBarText = title;

                    var dialog = new QuestionDialogBox(owner, settings, message);
                    result = dialog.ShowDialog() == true;
                });
                return result;
            },
            cancellationToken);

    /// <inheritdoc />
    public Task<string?> ShowInput(
        string title,
        string label,
        string? defaultValue = null,
        CancellationToken cancellationToken = default)
        => Task.Run(
            () =>
            {
                if (Application.Current?.Dispatcher is not { HasShutdownStarted: false } dispatcher)
                {
                    return null;
                }

                string? result = null;
                dispatcher.Invoke(() =>
                {
                    var owner = GetOwnerWindow();
                    var labelTextBox = new LabelTextBox
                    {
                        LabelText = label,
                        Text = defaultValue ?? string.Empty,
                    };

                    var dialog = new InputDialogBox(owner, title, labelTextBox);
                    if (dialog.ShowDialog() == true)
                    {
                        result = labelTextBox.Text;
                    }
                });
                return result;
            },
            cancellationToken);

    /// <inheritdoc />
    public Task<Color?> ShowColorPicker(
        string title,
        Color? initialColor = null,
        CancellationToken cancellationToken = default)
        => Task.Run(
            () =>
            {
                if (Application.Current?.Dispatcher is not { HasShutdownStarted: false } dispatcher)
                {
                    return null;
                }

                Color? result = null;
                dispatcher.Invoke(() =>
                {
                    var owner = GetOwnerWindow();
                    var settings = DialogBoxSettings.Create(DialogBoxType.OkCancel);
                    settings.TitleBarText = title;
                    settings.Width = 770;
                    settings.Height = 700;

                    var dialog = new ColorPickerDialogBox(owner, settings, initialColor ?? Colors.Black);
                    if (dialog.ShowDialog() == true)
                    {
                        result = dialog.Color;
                    }
                });
                return result;
            },
            cancellationToken);

    private Window GetOwnerWindow()
    {
        var owner = ownerResolver?.Invoke();
        return owner ?? Application.Current.MainWindow ?? throw new InvalidOperationException("No owner window available.");
    }
}