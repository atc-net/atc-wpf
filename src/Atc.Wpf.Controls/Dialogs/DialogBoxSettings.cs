namespace Atc.Wpf.Controls.Dialogs;

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
                AffirmativeButtonText = Miscellaneous.Ok;
                NegativeButtonText = string.Empty;
                ShowNegativeButton = false;
                break;
            case DialogBoxType.OkCancel:
                SetContentSvgImage();
                AffirmativeButtonText = Miscellaneous.Ok;
                NegativeButtonText = Miscellaneous.Cancel;
                ShowNegativeButton = true;
                break;
            case DialogBoxType.YesNo:
                SetContentSvgImage();
                AffirmativeButtonText = Miscellaneous.Yes;
                NegativeButtonText = Miscellaneous.No;
                ShowNegativeButton = true;
                break;
            default:
                throw new SwitchCaseDefaultException(dialogBoxType);
        }

        Form = new LabelInputFormPanelSettings();
    }

    public DialogBoxSettings(
        DialogBoxType dialogBoxType,
        Color iconColor)
        : this(
            dialogBoxType,
            iconShape: null,
            iconColor)
    {
    }

    public DialogBoxSettings(
        DialogBoxType dialogBoxType,
        LogCategoryType iconShapeAndColor)
        : this(
            dialogBoxType,
            iconShapeAndColor,
            (Color)new LogCategoryTypeToColorValueConverter().Convert(iconShapeAndColor, typeof(Color), parameter: null, CultureInfo.CurrentUICulture))
    {
    }

    public double Width { get; set; } = 350;

    public double Height { get; set; } = 200;

    public string TitleBarText { get; set; } = string.Empty;

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

    public SvgImage? ContentSvgImage { get; set; }

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

    public bool ShowNegativeButton { get; private set; }

    /// <summary>
    /// Gets or sets the text used for the Affirmative button..
    /// </summary>
    /// /// <example>
    /// "OK" or "Yes"
    /// </example>
    public string AffirmativeButtonText { get; set; } = "? Negative ?";

    public SolidColorBrush AffirmativeButtonBackground { get; set; } = (SolidColorBrush)CurrentTheme.Resources[DefaultButtonBackgroundResourceKey]!;

    public SolidColorBrush AffirmativeButtonForeground { get; set; } = (SolidColorBrush)CurrentTheme.Resources[DefaultButtonForegroundResourceKey]!;

    /// <summary>
    /// Gets or sets the text used for the Negative button.
    /// </summary>
    /// <example>
    /// "Cancel" or "No"
    /// </example>
    public string NegativeButtonText { get; set; } = "? Affirmative ?";

    public SolidColorBrush NegativeButtonBackground { get; set; } = (SolidColorBrush)CurrentTheme.Resources[DefaultButtonBackgroundResourceKey]!;

    public SolidColorBrush NegativeButtonForeground { get; set; } = (SolidColorBrush)CurrentTheme.Resources[DefaultButtonForegroundResourceKey]!;

    public LabelInputFormPanelSettings Form { get; set; }

    public static DialogBoxSettings Create(
        DialogBoxType dialogBoxType)
        => new(dialogBoxType);

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
                    LogCategoryType.Critical => "/Atc.Wpf.Controls;component/Resources/LogCategoryIcons/error.svg",
                    LogCategoryType.Error => "/Atc.Wpf.Controls;component/Resources/LogCategoryIcons/error.svg",
                    LogCategoryType.Warning => "/Atc.Wpf.Controls;component/Resources/LogCategoryIcons/warning.svg",
                    LogCategoryType.Security => "/Atc.Wpf.Controls;component/Resources/LogCategoryIcons/information.svg",
                    LogCategoryType.Audit => "/Atc.Wpf.Controls;component/Resources/LogCategoryIcons/information.svg",
                    LogCategoryType.Service => "/Atc.Wpf.Controls;component/Resources/LogCategoryIcons/information.svg",
                    LogCategoryType.UI => "/Atc.Wpf.Controls;component/Resources/LogCategoryIcons/information.svg",
                    LogCategoryType.Information => "/Atc.Wpf.Controls;component/Resources/LogCategoryIcons/information.svg",
                    LogCategoryType.Debug => "/Atc.Wpf.Controls;component/Resources/LogCategoryIcons/information.svg",
                    LogCategoryType.Trace => "/Atc.Wpf.Controls;component/Resources/LogCategoryIcons/information.svg",
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