namespace Atc.Wpf.Forms.Dialogs;

/// <summary>
/// The button row of the dialog boxes, built from <see cref="DialogBoxSettings"/>: the affirmative button, the
/// negative button when <see cref="DialogBoxSettings.ShowNegativeButton"/> is set and the Cancel button when
/// <see cref="DialogBoxSettings.ShowCancelButton"/> is set. Enter clicks <see cref="DialogBoxSettings.DefaultButton"/>;
/// Escape clicks Cancel, or the negative button when there is no Cancel button.
/// </summary>
public sealed class DialogBoxButtonBar : ContentControl
{
    /// <summary>
    /// The automation id of the affirmative button.
    /// </summary>
    public const string AffirmativeButtonAutomationId = "DialogBoxAffirmativeButton";

    /// <summary>
    /// The automation id of the negative button.
    /// </summary>
    public const string NegativeButtonAutomationId = "DialogBoxNegativeButton";

    /// <summary>
    /// The automation id of the Cancel button.
    /// </summary>
    public const string CancelButtonAutomationId = "DialogBoxCancelButton";

    /// <summary>
    /// The automation id of the copy button.
    /// </summary>
    public const string CopyButtonAutomationId = "DialogBoxCopyButton";

    private const string CopyIconData =
        "M16,1 L4,1 C2.9,1 2,1.9 2,3 L2,17 L4,17 L4,3 L16,3 Z M19,5 L8,5 C6.9,5 6,5.9 6,7 L6,21 C6,22.1 6.9,23 8,23 " +
        "L19,23 C20.1,23 21,22.1 21,21 L21,7 C21,5.9 20.1,5 19,5 Z M19,21 L8,21 L8,7 L19,7 Z";

    private const string CopiedIconData = "M9,16.17 L4.83,12 L3.41,13.41 L9,19 L21,7 L19.59,5.59 Z";

    private static readonly TimeSpan CopiedFeedbackDuration = TimeSpan.FromSeconds(1.5);

    /// <summary>
    /// Identifies the <see cref="Settings"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SettingsProperty = DependencyProperty.Register(
        nameof(Settings),
        typeof(DialogBoxSettings),
        typeof(DialogBoxButtonBar),
        new PropertyMetadata(defaultValue: null, OnSettingsChanged));

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogBoxButtonBar"/> class.
    /// </summary>
    public DialogBoxButtonBar()
    {
        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment = VerticalAlignment.Center;
        Focusable = false;
        IsTabStop = false;
    }

    /// <summary>
    /// Occurs when one of the buttons is clicked.
    /// </summary>
    public event EventHandler<DialogBoxButtonClickedEventArgs>? ButtonClicked;

    /// <summary>
    /// Occurs when the copy button (shown with <see cref="DialogBoxSettings.ShowCopyButton"/>) is clicked. The
    /// dialog copies its text; the button shows a check mark for a moment.
    /// </summary>
    public event EventHandler? CopyClicked;

    /// <summary>
    /// Gets or sets the settings that give the buttons, their texts and colors and the default button.
    /// </summary>
    public DialogBoxSettings? Settings
    {
        get => (DialogBoxSettings?)GetValue(SettingsProperty);
        set => SetValue(SettingsProperty, value);
    }

    /// <summary>
    /// Gets the button shown for <paramref name="button"/>, or <see langword="null"/> when it is not shown.
    /// </summary>
    /// <param name="button">The dialog box button.</param>
    /// <returns>The button, or <see langword="null"/>.</returns>
    public Button? GetButton(DialogBoxButton button)
        => (Content as Panel)?.Children
            .OfType<Button>()
            .FirstOrDefault(x => x.Tag is DialogBoxButton tag && tag == button);

    /// <summary>
    /// Gets the copy button, or <see langword="null"/> when it is not shown.
    /// </summary>
    /// <returns>The copy button, or <see langword="null"/>.</returns>
    public Button? GetCopyButton()
        => (Content as Panel)?.Children
            .OfType<Button>()
            .FirstOrDefault(x => AutomationProperties.GetAutomationId(x) == CopyButtonAutomationId);

    private static void OnSettingsChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((DialogBoxButtonBar)d).BuildButtons();

    private void BuildButtons()
    {
        var settings = Settings;
        if (settings is null)
        {
            Content = null;
            return;
        }

        var panel = new Atc.Wpf.Controls.Layouts.UniformSpacingPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 20,
        };

        if (settings.ShowCopyButton)
        {
            panel.Children.Add(CreateCopyButton());
        }

        panel.Children.Add(CreateButton(DialogBoxButton.Affirmative, settings.AffirmativeButtonText, settings.AffirmativeButtonBackground, settings.AffirmativeButtonForeground, AffirmativeButtonAutomationId));

        if (settings.ShowNegativeButton)
        {
            panel.Children.Add(CreateButton(DialogBoxButton.Negative, settings.NegativeButtonText, settings.NegativeButtonBackground, settings.NegativeButtonForeground, NegativeButtonAutomationId));
        }

        if (settings.ShowCancelButton)
        {
            panel.Children.Add(CreateButton(DialogBoxButton.Cancel, settings.CancelButtonText, settings.NegativeButtonBackground, settings.NegativeButtonForeground, CancelButtonAutomationId));
        }

        var buttons = panel.Children.OfType<Button>().Where(x => x.Tag is DialogBoxButton).ToList();
        var defaultButton = buttons.Find(x => (DialogBoxButton)x.Tag == settings.DefaultButton) ?? buttons[0];
        defaultButton.IsDefault = true;

        var cancelButton = buttons.Find(x => (DialogBoxButton)x.Tag == DialogBoxButton.Cancel) ??
                           buttons.Find(x => (DialogBoxButton)x.Tag == DialogBoxButton.Negative);
        if (cancelButton is not null)
        {
            cancelButton.IsCancel = true;
        }

        Content = panel;
    }

    private Button CreateButton(
        DialogBoxButton button,
        string text,
        Brush background,
        Brush foreground,
        string automationId)
    {
        var result = new Button
        {
            Tag = button,
            MinWidth = 100,
            Height = 30,
            Padding = new Thickness(10, 0, 10, 0),
            Content = text,
            Background = background,
            Foreground = foreground,
        };

        AutomationProperties.SetAutomationId(result, automationId);
        result.Click += (_, _) => ButtonClicked?.Invoke(this, new DialogBoxButtonClickedEventArgs(button));
        return result;
    }

    private Button CreateCopyButton()
    {
        var icon = new System.Windows.Shapes.Path
        {
            Data = Geometry.Parse(CopyIconData),
            Stretch = Stretch.Uniform,
            Width = 16,
            Height = 16,
        };

        var button = new Button
        {
            Width = 30,
            Height = 30,
            Padding = new Thickness(0),
            Content = icon,
            ToolTip = Atc.Wpf.Resources.Miscellaneous.CopyToClipboard,
        };

        // The icon takes the button's text color, so it suits the light and the dark theme.
        icon.SetBinding(
            System.Windows.Shapes.Shape.FillProperty,
            new Binding(nameof(Foreground)) { Source = button });

        AutomationProperties.SetAutomationId(button, CopyButtonAutomationId);
        AutomationProperties.SetName(button, Atc.Wpf.Resources.Miscellaneous.CopyToClipboard);

        var feedbackTimer = new System.Windows.Threading.DispatcherTimer { Interval = CopiedFeedbackDuration };
        feedbackTimer.Tick += (_, _) =>
        {
            feedbackTimer.Stop();
            icon.Data = Geometry.Parse(CopyIconData);
        };

        button.Click += (_, _) =>
        {
            CopyClicked?.Invoke(this, EventArgs.Empty);
            icon.Data = Geometry.Parse(CopiedIconData);
            feedbackTimer.Stop();
            feedbackTimer.Start();
        };

        return button;
    }
}