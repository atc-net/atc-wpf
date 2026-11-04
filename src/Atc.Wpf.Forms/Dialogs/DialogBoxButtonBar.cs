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

        panel.Children.Add(CreateButton(DialogBoxButton.Affirmative, settings.AffirmativeButtonText, settings.AffirmativeButtonBackground, settings.AffirmativeButtonForeground, AffirmativeButtonAutomationId));

        if (settings.ShowNegativeButton)
        {
            panel.Children.Add(CreateButton(DialogBoxButton.Negative, settings.NegativeButtonText, settings.NegativeButtonBackground, settings.NegativeButtonForeground, NegativeButtonAutomationId));
        }

        if (settings.ShowCancelButton)
        {
            panel.Children.Add(CreateButton(DialogBoxButton.Cancel, settings.CancelButtonText, settings.NegativeButtonBackground, settings.NegativeButtonForeground, CancelButtonAutomationId));
        }

        var buttons = panel.Children.OfType<Button>().ToList();
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
}