namespace Atc.Wpf.Controls.Dialogs;

public class DialogBoxSettings
{
    private const string DefaultButtonBackgroundResourceKey = "AtcApps.Brushes.Gray10";
    private const string DefaultButtonForegroundResourceKey = "AtcApps.Brushes.ThemeForeground";
    private const string DefaultButtonForegroundIdealResourceKey = "AtcApps.Brushes.IdealForeground";
    private static readonly Theme CurrentTheme = ThemeManager.Current.DetectTheme()!;
    private bool usePrimaryAccentColor;

    public DialogBoxSettings(
        DialogBoxType dialogBoxType)
    {
        switch (dialogBoxType)
        {
            case DialogBoxType.Unknown:
                break;
            case DialogBoxType.Ok:
                ContentSvgImage = new SvgImage
                {
                    Margin = new Thickness(0.0, 0.0, 20.0, 0.0),
                    Width = 32,
                    Height = 32,
                    Source = "/Atc.Wpf.Controls;component/Resources/information.svg",
                    OverrideColor = Colors.DodgerBlue,
                };

                AffirmativeButtonText = Miscellaneous.Ok;
                NegativeButtonText = string.Empty;
                ShowNegativeButton = false;
                break;
            case DialogBoxType.OkCancel:
                ContentSvgImage = new SvgImage
                {
                    Margin = new Thickness(0.0, 0.0, 20.0, 0.0),
                    Width = 32,
                    Height = 32,
                    Source = "/Atc.Wpf.Controls;component/Resources/question-mark.svg",
                    OverrideColor = Colors.DodgerBlue,
                };

                AffirmativeButtonText = Miscellaneous.Ok;
                NegativeButtonText = Miscellaneous.Cancel;
                ShowNegativeButton = true;
                break;
            case DialogBoxType.YesNo:
                ContentSvgImage = new SvgImage
                {
                    Margin = new Thickness(0.0, 0.0, 20.0, 0.0),
                    Width = 32,
                    Height = 32,
                    Source = "/Atc.Wpf.Controls;component/Resources/question-mark.svg",
                    OverrideColor = Colors.DodgerBlue,
                };

                AffirmativeButtonText = Miscellaneous.Yes;
                NegativeButtonText = Miscellaneous.No;
                ShowNegativeButton = true;
                break;
            default:
                throw new SwitchCaseDefaultException(dialogBoxType);
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
                AffirmativeButtonForeground = (SolidColorBrush)CurrentTheme.Resources[DefaultButtonForegroundIdealResourceKey];
            }
            else
            {
                AffirmativeButtonBackground = (SolidColorBrush)CurrentTheme.Resources[DefaultButtonBackgroundResourceKey];
                AffirmativeButtonForeground = (SolidColorBrush)CurrentTheme.Resources[DefaultButtonForegroundResourceKey];
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

    public SolidColorBrush AffirmativeButtonBackground { get; set; } = (SolidColorBrush)CurrentTheme.Resources[DefaultButtonBackgroundResourceKey];

    public SolidColorBrush AffirmativeButtonForeground { get; set; } = (SolidColorBrush)CurrentTheme.Resources[DefaultButtonForegroundResourceKey];

    /// <summary>
    /// Gets or sets the text used for the Negative button.
    /// </summary>
    /// <example>
    /// "Cancel" or "No"
    /// </example>
    public string NegativeButtonText { get; set; } = "? Affirmative ?";

    public SolidColorBrush NegativeButtonBackground { get; set; } = (SolidColorBrush)CurrentTheme.Resources[DefaultButtonBackgroundResourceKey];

    public SolidColorBrush NegativeButtonForeground { get; set; } = (SolidColorBrush)CurrentTheme.Resources[DefaultButtonForegroundResourceKey];

    public Orientation FromControlOrientation { get; set; } = Orientation.Vertical;

    public int FromControlWidth { get; set; } = 300;

    public static DialogBoxSettings Create(DialogBoxType dialogBoxType)
        => new(dialogBoxType);
}