namespace Atc.Wpf.Components.Dialogs;

/// <summary>
/// The body of an in-window dialog and its button bar, shown inside a <see cref="Flyout"/>.
/// A button click closes the flyout with the answer for that <see cref="DialogBoxButton"/> as its result; the
/// answer is taken on the UI thread when the button is clicked, so it can read the dialog's controls.
/// </summary>
internal sealed class InWindowDialogContent : Grid
{
    private readonly Func<DialogBoxButton, object?> answer;
    private readonly Func<bool>? canAccept;
    private readonly Func<IInputElement?>? initialFocus;
    private Flyout? flyout;

    public InWindowDialogContent(
        FrameworkElement body,
        DialogBoxSettings settings,
        Func<DialogBoxButton, object?> answer,
        Func<bool>? canAccept = null,
        Func<IInputElement?>? initialFocus = null)
    {
        ArgumentNullException.ThrowIfNull(body);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(answer);

        this.answer = answer;

        this.canAccept = canAccept;
        this.initialFocus = initialFocus;

        RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        Body = body;
        SetRow(body, 0);
        Children.Add(body);

        ButtonBar = new DialogBoxButtonBar
        {
            Settings = settings,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 16, 0, 0),
        };
        ButtonBar.ButtonClicked += OnButtonClicked;
        SetRow(ButtonBar, 1);
        Children.Add(ButtonBar);

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    public FrameworkElement Body { get; }

    public DialogBoxButtonBar ButtonBar { get; }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        flyout = FindFlyout();
        if (flyout is null)
        {
            return;
        }

        flyout.Opened += OnFlyoutOpened;
        if (flyout.IsOpen)
        {
            FocusInitialElement();
        }
    }

    private void OnUnloaded(
        object sender,
        RoutedEventArgs e)
    {
        if (flyout is not null)
        {
            flyout.Opened -= OnFlyoutOpened;
        }
    }

    // The flyout focuses itself right after it raises Opened, so the dialog takes the focus after that.
    private void OnFlyoutOpened(
        object? sender,
        RoutedEventArgs e)
        => FocusInitialElement();

    private void FocusInitialElement()
        => _ = Dispatcher.BeginInvoke(
            DispatcherPriority.Input,
            () =>
            {
                var target = initialFocus?.Invoke() ?? DefaultButton();
                _ = target?.Focus();
            });

    private Button? DefaultButton()
        => new[] { DialogBoxButton.Affirmative, DialogBoxButton.Negative, DialogBoxButton.Cancel }
            .Select(ButtonBar.GetButton)
            .FirstOrDefault(button => button is { IsDefault: true });

    private void OnButtonClicked(
        object? sender,
        DialogBoxButtonClickedEventArgs e)
    {
        if (e.Button == DialogBoxButton.Affirmative &&
            canAccept is not null &&
            !canAccept())
        {
            return;
        }

        _ = (flyout ?? FindFlyout())?.CloseWithResultAsync(answer(e.Button));
    }

    private Flyout? FindFlyout()
    {
        DependencyObject? current = this;
        while (current is not null and not Flyout)
        {
            current = VisualTreeHelper.GetParent(current) ?? LogicalTreeHelper.GetParent(current);
        }

        return current as Flyout;
    }
}