// ReSharper disable InvertIf
namespace Atc.Wpf.Controls.ButtonControls;

public partial class ConnectivityButton
{
    public event RoutedEventHandler? IsConnectedChanged;

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
        PropertyChangedCallback = nameof(OnIsConnectedChanged))]
    private bool isConnected;

    [DependencyProperty]
    private object? connectContent;

    [DependencyProperty]
    private object? disconnectContent;

    [DependencyProperty]
    private ImageSource? connectImageSource;

    [DependencyProperty]
    private ImageSource? disconnectImageSource;

    [DependencyProperty(DefaultValue = "")]
    private string connectSvgImageSource;

    [DependencyProperty(DefaultValue = "")]
    private string disconnectSvgImageSource;

    [DependencyProperty]
    private Color? connectSvgImageOverrideColor;

    [DependencyProperty]
    private Color? disconnectSvgImageOverrideColor;

    [DependencyProperty]
    private ICommand? connectCommand;

    [DependencyProperty]
    private ICommand? disconnectCommand;

    public ConnectivityButton()
    {
        InitializeComponent();

        SetCurrentValue(ImageLocationProperty, Controls.ImageLocation.Left);

        ThemeManager.Current.ThemeChanged += OnThemeChanged;
    }

    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);

        if (ConnectContent is null or string { Length: 0 })
        {
            SetCurrentValue(ConnectContentProperty, Word.Connect);
        }

        if (DisconnectContent is null or string { Length: 0 })
        {
            SetCurrentValue(DisconnectContentProperty, Word.Disconnect);
        }

        if (ConnectImageSource is null &&
            string.IsNullOrWhiteSpace(ConnectSvgImageSource))
        {
            var fgBrush = Application.Current.TryFindResource("AtcApps.Brushes.ThemeForeground");

            var img = (ImageSource?)FontIconImageSourceValueConverter.Instance
                .Convert(
                    FontMaterialDesignType.Login,
                    typeof(ImageSource),
                    fgBrush,
                    CultureInfo.CurrentUICulture);

            if (img is not null)
            {
                SetCurrentValue(ConnectImageSourceProperty, img);
            }
        }

        if (DisconnectImageSource is null &&
            string.IsNullOrWhiteSpace(DisconnectSvgImageSource))
        {
            var accentBrush = Application.Current.TryFindResource("AtcApps.Brushes.Accent");

            var img = (ImageSource?)FontIconImageSourceValueConverter.Instance
                .Convert(
                    FontMaterialDesignType.Logout,
                    typeof(ImageSource),
                    accentBrush,
                    CultureInfo.CurrentUICulture);

            if (img is not null)
            {
                SetCurrentValue(DisconnectImageSourceProperty, img);
            }
        }
    }

    private void OnThemeChanged(
        object? sender,
        ThemeChangedEventArgs e)
    {
        SetCurrentValue(ConnectContentProperty, Word.Connect);
        SetCurrentValue(DisconnectContentProperty, Word.Disconnect);
    }

    private static void OnIsConnectedChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((ConnectivityButton)d).RaiseIsConnectedChanged();

    private void RaiseIsConnectedChanged()
        => IsConnectedChanged?.Invoke(this, new RoutedEventArgs());
}