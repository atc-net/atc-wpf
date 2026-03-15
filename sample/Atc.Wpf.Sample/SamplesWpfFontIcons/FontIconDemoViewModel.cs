namespace Atc.Wpf.Sample.SamplesWpfFontIcons;

public partial class FontIconDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Spin", "Animation", 1)]
    [ObservableProperty]
    private bool spin;

    [PropertyDisplay("Spin Duration", "Animation", 2)]
    [PropertyRange(0, 5, 0.1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double spinDuration;

    [PropertyDisplay("Font Size", "Size", 1)]
    [PropertyRange(5, 200, 5)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double fontSize = 100;

    [PropertyDisplay("Image Size", "Size", 2)]
    [PropertyRange(12, 100, 1)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double imageSize = 48;

    [PropertyDisplay("Flip Orientation", "Appearance", 1)]
    [ObservableProperty]
    private FlipOrientationType flipOrientation = FlipOrientationType.Normal;

    [PropertyDisplay("Font Foreground Color", "Appearance", 2)]
    [ObservableProperty]
    private Color fontForegroundColor = Colors.Green;

    [PropertyDisplay("Image Foreground Color", "Appearance", 3)]
    [ObservableProperty]
    private Color imageForegroundColor = Colors.Orange;

    [PropertyDisplay("FA Brand", "Icons", 1)]
    [ObservableProperty]
    private FontAwesomeBrandType fontAwesomeBrandIcon = FontAwesomeBrandType._500px;

    [PropertyDisplay("FA Regular", "Icons", 2)]
    [ObservableProperty]
    private FontAwesomeRegularType fontAwesomeRegularIcon = FontAwesomeRegularType.AddressBook;

    [PropertyDisplay("FA Solid", "Icons", 3)]
    [ObservableProperty]
    private FontAwesomeSolidType fontAwesomeSolidIcon = FontAwesomeSolidType.AddressBook;

    [PropertyDisplay("FA7 Brand", "Icons", 4)]
    [ObservableProperty]
    private FontAwesomeBrand7Type fontAwesomeBrand7Icon = FontAwesomeBrand7Type.Github;

    [PropertyDisplay("FA7 Regular", "Icons", 5)]
    [ObservableProperty]
    private FontAwesomeRegular7Type fontAwesomeRegular7Icon = FontAwesomeRegular7Type.AddressBook;

    [PropertyDisplay("FA7 Solid", "Icons", 6)]
    [ObservableProperty]
    private FontAwesomeSolid7Type fontAwesomeSolid7Icon = FontAwesomeSolid7Type.AddressBook;

    [PropertyDisplay("Bootstrap", "Icons", 7)]
    [ObservableProperty]
    private FontBootstrapType fontBootstrapIcon = FontBootstrapType.Adjust;

    [PropertyDisplay("IcoFont", "Icons", 8)]
    [ObservableProperty]
    private IcoFontType icoFontIcon = IcoFontType._2checkout;

    [PropertyDisplay("Material Design", "Icons", 9)]
    [ObservableProperty]
    private FontMaterialDesignType fontMaterialDesignIcon = FontMaterialDesignType.AccessPoint;

    [PropertyDisplay("Weather", "Icons", 10)]
    [ObservableProperty]
    private FontWeatherType fontWeatherIcon = FontWeatherType.Alien;
}