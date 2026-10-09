namespace Atc.Wpf.Forms.Dialogs;

/// <summary>
/// Settings for a dialog box: size, title, icon, button texts and colors, and form layout.
/// </summary>
public sealed class DialogBoxSettings
{
    private const string DefaultButtonBackgroundResourceKey = "AtcApps.Brushes.Gray10";
    private const string DefaultButtonForegroundResourceKey = "AtcApps.Brushes.ThemeForeground";
    private const string DefaultButtonForegroundIdealResourceKey = "AtcApps.Brushes.IdealForeground";
    private static readonly Theme CurrentTheme = ThemeManager.Current.DetectTheme()!;
    private readonly DialogBoxType dialogBoxType;
    private readonly LogCategoryType logCategoryTypeToContentSvgImage;
    private bool usePrimaryAccentColor;
    private Color? contentSvgImageColor;

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogBoxSettings"/> class.
    /// </summary>
    /// <param name="dialogBoxType">The dialog box type, which sets the buttons shown.</param>
    /// <param name="iconShape">The log category that selects the icon; defaults to information.</param>
    /// <param name="iconColor">The icon color; defaults to dodger blue.</param>
    public DialogBoxSettings(
        DialogBoxType dialogBoxType,
        LogCategoryType? iconShape = null,
        Color? iconColor = null)
    {
        this.dialogBoxType = dialogBoxType;
        logCategoryTypeToContentSvgImage = iconShape ?? LogCategoryType.Information;
        ContentSvgImageColor = iconColor ?? Colors.DodgerBlue;

        switch (dialogBoxType)
        {
            case DialogBoxType.Unknown:
                break;
            case DialogBoxType.Ok:
                SetContentSvgImage();
                AffirmativeButtonText = Word.Ok;
                NegativeButtonText = string.Empty;
                ShowNegativeButton = false;
                break;
            case DialogBoxType.OkCancel:
                SetContentSvgImage();
                AffirmativeButtonText = Word.Ok;
                NegativeButtonText = Word.Cancel;
                ShowNegativeButton = true;
                break;
            case DialogBoxType.YesNo:
                SetContentSvgImage();
                AffirmativeButtonText = Word.Yes;
                NegativeButtonText = Word.No;
                ShowNegativeButton = true;
                break;
            default:
                throw new SwitchCaseDefaultException(dialogBoxType);
        }

        Form = new LabelInputFormPanelSettings();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogBoxSettings"/> class with a custom icon color.
    /// </summary>
    /// <param name="dialogBoxType">The dialog box type, which sets the buttons shown.</param>
    /// <param name="iconColor">The icon color.</param>
    public DialogBoxSettings(
        DialogBoxType dialogBoxType,
        Color iconColor)
        : this(
            dialogBoxType,
            iconShape: null,
            iconColor)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogBoxSettings"/> class with an icon whose shape and color come from a log category.
    /// </summary>
    /// <param name="dialogBoxType">The dialog box type, which sets the buttons shown.</param>
    /// <param name="iconShapeAndColor">The log category that selects both the icon and its color.</param>
    public DialogBoxSettings(
        DialogBoxType dialogBoxType,
        LogCategoryType iconShapeAndColor)
        : this(
            dialogBoxType,
            iconShapeAndColor,
            Convert(iconShapeAndColor))
    {
    }

    private static Color Convert(LogCategoryType iconShapeAndColor)
    {
        var converter = new LogCategoryTypeToColorValueConverter();

        var o = converter.Convert(
            iconShapeAndColor,
            typeof(Color),
            parameter: null,
            CultureInfo.CurrentUICulture);

        return o is null
            ? Colors.DeepPink
            : (Color)o;
    }

    /// <summary>
    /// Gets or sets the dialog width.
    /// </summary>
    public double Width { get; set; } = 350;

    /// <summary>
    /// Gets or sets the dialog height.
    /// </summary>
    public double Height { get; set; } = 200;

    /// <summary>
    /// Gets or sets the text shown in the dialog title bar.
    /// </summary>
    public string TitleBarText { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the color of the content icon; changing it rebuilds the icon.
    /// </summary>
    public Color? ContentSvgImageColor
    {
        get => contentSvgImageColor;
        set
        {
            contentSvgImageColor = value;
            if (ContentSvgImage is not null)
            {
                SetContentSvgImage();
            }
        }
    }

    /// <summary>
    /// Gets or sets the icon shown next to the dialog content.
    /// </summary>
    public SvgImage? ContentSvgImage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the affirmative button uses the theme's primary accent color.
    /// </summary>
    public bool UsePrimaryAccentColor
    {
        get => usePrimaryAccentColor;
        set
        {
            usePrimaryAccentColor = value;
            if (usePrimaryAccentColor)
            {
                AffirmativeButtonBackground = new SolidColorBrush(CurrentTheme.PrimaryAccentColor);
                AffirmativeButtonForeground = (SolidColorBrush)CurrentTheme.Resources[DefaultButtonForegroundIdealResourceKey]!;
            }
            else
            {
                AffirmativeButtonBackground = (SolidColorBrush)CurrentTheme.Resources[DefaultButtonBackgroundResourceKey]!;
                AffirmativeButtonForeground = (SolidColorBrush)CurrentTheme.Resources[DefaultButtonForegroundResourceKey]!;
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether the negative button is shown.
    /// </summary>
    public bool ShowNegativeButton { get; private set; }

    /// <summary>
    /// Gets or sets the text used for the Affirmative button..
    /// </summary>
    /// /// <example>
    /// "OK" or "Yes"
    /// </example>
    public string AffirmativeButtonText { get; set; } = "? Negative ?";

    /// <summary>
    /// Gets or sets the background brush of the affirmative button.
    /// </summary>
    public SolidColorBrush AffirmativeButtonBackground { get; set; } = (SolidColorBrush)CurrentTheme.Resources[DefaultButtonBackgroundResourceKey]!;

    /// <summary>
    /// Gets or sets the foreground brush of the affirmative button.
    /// </summary>
    public SolidColorBrush AffirmativeButtonForeground { get; set; } = (SolidColorBrush)CurrentTheme.Resources[DefaultButtonForegroundResourceKey]!;

    /// <summary>
    /// Gets or sets the text used for the Negative button.
    /// </summary>
    /// <example>
    /// "Cancel" or "No"
    /// </example>
    public string NegativeButtonText { get; set; } = "? Affirmative ?";

    /// <summary>
    /// Gets or sets the background brush of the negative button.
    /// </summary>
    public SolidColorBrush NegativeButtonBackground { get; set; } = (SolidColorBrush)CurrentTheme.Resources[DefaultButtonBackgroundResourceKey]!;

    /// <summary>
    /// Gets or sets the foreground brush of the negative button.
    /// </summary>
    public SolidColorBrush NegativeButtonForeground { get; set; } = (SolidColorBrush)CurrentTheme.Resources[DefaultButtonForegroundResourceKey]!;

    /// <summary>
    /// Gets or sets the layout settings for a form shown in the dialog.
    /// </summary>
    public LabelInputFormPanelSettings Form { get; set; }

    /// <summary>
    /// Creates settings for the given dialog box type with the default icon.
    /// </summary>
    /// <param name="dialogBoxType">The dialog box type.</param>
    /// <returns>The new settings.</returns>
    public static DialogBoxSettings Create(DialogBoxType dialogBoxType)
        => new(dialogBoxType);

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Width)}: {Width}, {nameof(Height)}: {Height}, {nameof(TitleBarText)}: {TitleBarText}, {nameof(Form)}: ({Form})";

    private void SetContentSvgImage()
    {
        switch (dialogBoxType)
        {
            case DialogBoxType.Unknown:
                break;
            case DialogBoxType.Ok:

                var svgImageSource = logCategoryTypeToContentSvgImage switch
                {
                    LogCategoryType.Critical => "/Atc.Wpf.Forms;component/Resources/LogCategoryIcons/error.svg",
                    LogCategoryType.Error => "/Atc.Wpf.Forms;component/Resources/LogCategoryIcons/error.svg",
                    LogCategoryType.Warning => "/Atc.Wpf.Forms;component/Resources/LogCategoryIcons/warning.svg",
                    LogCategoryType.Security => "/Atc.Wpf.Forms;component/Resources/LogCategoryIcons/information.svg",
                    LogCategoryType.Audit => "/Atc.Wpf.Forms;component/Resources/LogCategoryIcons/information.svg",
                    LogCategoryType.Service => "/Atc.Wpf.Forms;component/Resources/LogCategoryIcons/information.svg",
                    LogCategoryType.UI => "/Atc.Wpf.Forms;component/Resources/LogCategoryIcons/information.svg",
                    LogCategoryType.Information => "/Atc.Wpf.Forms;component/Resources/LogCategoryIcons/information.svg",
                    LogCategoryType.Debug => "/Atc.Wpf.Forms;component/Resources/LogCategoryIcons/information.svg",
                    LogCategoryType.Trace => "/Atc.Wpf.Forms;component/Resources/LogCategoryIcons/information.svg",
                    _ => throw new SwitchCaseDefaultException(logCategoryTypeToContentSvgImage),
                };

                ContentSvgImage = new SvgImage
                {
                    Margin = new Thickness(0.0, 0.0, 20.0, 0.0),
                    Width = 32,
                    Height = 32,
                    Source = svgImageSource,
                    OverrideColor = ContentSvgImageColor,
                };
                break;
            case DialogBoxType.OkCancel:
            case DialogBoxType.YesNo:
                ContentSvgImage = new SvgImage
                {
                    Margin = new Thickness(0.0, 0.0, 20.0, 0.0),
                    Width = 32,
                    Height = 32,
                    Source = "/Atc.Wpf.Controls;component/Resources/Icons/question-mark.svg",
                    OverrideColor = ContentSvgImageColor,
                };
                break;
            default:
                throw new SwitchCaseDefaultException(dialogBoxType);
        }
    }
}