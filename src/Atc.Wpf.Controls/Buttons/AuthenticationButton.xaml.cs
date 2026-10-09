// ReSharper disable InvertIf
namespace Atc.Wpf.Controls.Buttons;

public partial class AuthenticationButton
{
    /// <summary>Occurs when the authenticated state of the button changes.</summary>
    public event RoutedEventHandler? IsAuthenticatedChanged;

    [DependencyProperty]
    private ImageLocation? imageLocation;

    [DependencyProperty(DefaultValue = 16)]
    private int imageWidth;

    [DependencyProperty(DefaultValue = 16)]
    private int imageHeight;

    [DependencyProperty(DefaultValue = 10)]
    private double imageContentSpacing;

    [DependencyProperty(DefaultValue = 0)]
    private double imageBorderSpacing;

    [DependencyProperty(DefaultValue = false)]
    private bool isBusy;

    [DependencyProperty(
        DefaultValue = false,
        PropertyChangedCallback = nameof(OnIsAuthenticatedChanged))]
    private bool isAuthenticated;

    [DependencyProperty]
    private object? loginContent;

    [DependencyProperty]
    private object? logoutContent;

    [DependencyProperty]
    private ImageSource? loginImageSource;

    [DependencyProperty]
    private ImageSource? logoutImageSource;

    [DependencyProperty(DefaultValue = "")]
    private string loginSvgImageSource;

    [DependencyProperty(DefaultValue = "")]
    private string logoutSvgImageSource;

    [DependencyProperty]
    private Color? loginSvgImageOverrideColor;

    [DependencyProperty]
    private Color? logoutSvgImageOverrideColor;

    [DependencyProperty]
    private ICommand? loginCommand;

    [DependencyProperty]
    private ICommand? logoutCommand;

    /// <summary>Initializes a new instance of the <see cref="AuthenticationButton"/> class.</summary>
    public AuthenticationButton()
    {
        InitializeComponent();

        SetCurrentValue(ImageLocationProperty, Controls.ImageLocation.Left);

        Loaded += OnLoadedSubscribeToThemeChanges;
        Unloaded += OnUnloadedUnsubscribeFromThemeChanges;
    }

    /// <inheritdoc />
    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

        if (LoginContent is null or string { Length: 0 })
        {
            SetCurrentValue(LoginContentProperty, Word.Login);
        }

        if (LogoutContent is null or string { Length: 0 })
        {
            SetCurrentValue(LogoutContentProperty, Word.Logout);
        }

        if (LoginImageSource is null &&
            string.IsNullOrWhiteSpace(LoginSvgImageSource))
        {
            var fgBrush = Application.Current?.TryFindResource("AtcApps.Brushes.ThemeForeground");

            var img = (ImageSource?)FontIconImageSourceValueConverter.Instance
                .Convert(
                    FontMaterialDesignType.Login,
                    typeof(ImageSource),
                    fgBrush,
                    CultureInfo.CurrentUICulture);

            if (img is not null)
            {
                SetCurrentValue(LoginImageSourceProperty, img);
            }
        }

        if (LogoutImageSource is null &&
            string.IsNullOrWhiteSpace(LogoutSvgImageSource))
        {
            var accentBrush = Application.Current?.TryFindResource("AtcApps.Brushes.Accent");

            var img = (ImageSource?)FontIconImageSourceValueConverter.Instance
                .Convert(
                    FontMaterialDesignType.Logout,
                    typeof(ImageSource),
                    accentBrush,
                    CultureInfo.CurrentUICulture);

            if (img is not null)
            {
                SetCurrentValue(LogoutImageSourceProperty, img);
            }
        }
    }

    private void OnLoadedSubscribeToThemeChanges(
        object sender,
        RoutedEventArgs e)
    {
        // ThemeManager is process-wide: only listen while in the visual tree, otherwise it keeps this control alive.
        ThemeManager.Current.ThemeChanged -= OnThemeChanged;
        ThemeManager.Current.ThemeChanged += OnThemeChanged;
    }

    private void OnUnloadedUnsubscribeFromThemeChanges(
        object sender,
        RoutedEventArgs e)
        => ThemeManager.Current.ThemeChanged -= OnThemeChanged;

    private void OnThemeChanged(
        object? sender,
        ThemeChangedEventArgs e)
    {
        SetCurrentValue(LoginContentProperty, Word.Login);
        SetCurrentValue(LogoutContentProperty, Word.Logout);
    }

    private static void OnIsAuthenticatedChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((AuthenticationButton)d).RaiseIsAuthenticatedChanged();

    private void RaiseIsAuthenticatedChanged()
        => IsAuthenticatedChanged?.Invoke(this, new RoutedEventArgs());
}