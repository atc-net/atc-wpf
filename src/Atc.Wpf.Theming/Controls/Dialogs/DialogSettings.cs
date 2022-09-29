namespace Atc.Wpf.Theming.Controls.Dialogs;

public class DialogSettings
{
    private const string DefaultAffirmativeButtonText = "OK";
    private const string DefaultNegativeButtonText = "Cancel";

    public DialogSettings()
    {
    }

    public DialogSettings(
        DialogSettings? source)
        : this()
    {
        if (source is null)
        {
            return;
        }

        OwnerCanCloseWithDialog = source.OwnerCanCloseWithDialog;

        AffirmativeButtonText = source.AffirmativeButtonText;
        NegativeButtonText = source.NegativeButtonText;
        DefaultText = source.DefaultText;
        FirstAuxiliaryButtonText = source.FirstAuxiliaryButtonText;
        SecondAuxiliaryButtonText = source.SecondAuxiliaryButtonText;

        ColorScheme = source.ColorScheme;
        CustomResourceDictionary = source.CustomResourceDictionary;

        AnimateShow = source.AnimateShow;
        AnimateHide = source.AnimateHide;

        MaximumBodyHeight = source.MaximumBodyHeight;

        DefaultButtonFocus = source.DefaultButtonFocus;
        CancellationToken = source.CancellationToken;
        DialogTitleFontSize = source.DialogTitleFontSize;
        DialogMessageFontSize = source.DialogMessageFontSize;
        DialogButtonFontSize = source.DialogButtonFontSize;
        DialogResultOnCancel = source.DialogResultOnCancel;

        Icon = source.Icon;
        IconTemplate = source.IconTemplate;
    }

    /// <summary>
    /// Gets or sets whether the owner of the dialog can be closed.
    /// </summary>
    public bool OwnerCanCloseWithDialog { get; set; }

    /// <summary>
    /// Gets or sets the text used for the Affirmative button. For example: "OK" or "Yes".
    /// </summary>
    public string AffirmativeButtonText { get; set; } = DefaultAffirmativeButtonText;

    /// <summary>
    /// Enable or disable dialog hiding animation
    /// "True" - play hiding animation.
    /// "False" - skip hiding animation.
    /// </summary>
    public bool AnimateHide { get; set; } = true;

    /// <summary>
    /// Enable or disable dialog showing animation.
    /// "True" - play showing animation.
    /// "False" - skip showing animation.
    /// </summary>
    public bool AnimateShow { get; set; } = true;

    /// <summary>
    /// Gets or sets a token to cancel the dialog.
    /// </summary>
    public CancellationToken CancellationToken { get; set; } = CancellationToken.None;

    /// <summary>
    /// Gets or sets whether the nice-dialog should use the default black/white appearance (theme) or try to use the current accent.
    /// </summary>
    public DialogColorScheme ColorScheme { get; set; } = DialogColorScheme.Theme;

    /// <summary>
    /// Gets or sets a custom resource dictionary which can contains custom styles, brushes or something else.
    /// </summary>
    public ResourceDictionary? CustomResourceDictionary { get; set; }

    /// <summary>
    /// Gets or sets which button should be focused by default
    /// </summary>
    public MessageDialogResultType DefaultButtonFocus { get; set; } = MessageDialogResultType.Negative;

    /// <summary>
    /// Gets or sets the default text.
    /// </summary>
    public string DefaultText { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the size of the dialog message font.
    /// </summary>
    /// <value>
    /// The size of the dialog message font.
    /// </value>
    public double DialogMessageFontSize { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets the size of the dialog button font.
    /// </summary>
    /// <value>
    /// The size of the dialog button font.
    /// </value>
    public double DialogButtonFontSize { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets the message dialog result when the user cancelled the dialog with 'ESC' key
    /// </summary>
    public MessageDialogResultType? DialogResultOnCancel { get; set; }

    /// <summary>
    /// Gets or sets the size of the dialog title font.
    /// </summary>
    /// <value>
    /// The size of the dialog title font.
    /// </value>
    public double DialogTitleFontSize { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets the text used for the first auxiliary button.
    /// </summary>
    public string? FirstAuxiliaryButtonText { get; set; }

    /// <summary>
    /// Gets or sets the maximum height. Default is unlimited height.
    /// </summary>
    public double MaximumBodyHeight { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets the text used for the Negative button. For example: "Cancel" or "No".
    /// </summary>
    public string NegativeButtonText { get; set; } = DefaultNegativeButtonText;

    /// <summary>
    /// Gets or sets the text used for the second auxiliary button.
    /// </summary>
    public string? SecondAuxiliaryButtonText { get; set; }

    /// <summary>
    /// Gets or sets the content used for the Icon ContentPresenter.
    /// </summary>
    public object? Icon { get; set; }

    /// <summary>
    /// Gets or sets the DataTemplate used for the Icon ContentPresenter.
    /// </summary>
    public DataTemplate? IconTemplate { get; set; }
}