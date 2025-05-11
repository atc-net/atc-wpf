namespace Atc.Wpf.Controls.BaseControls;

public sealed partial class ImageButton : Button
{
    [DependencyProperty(
        DefaultValue = null,
        PropertyChangedCallback = nameof(OnImageLocationChanged))]
    private ImageLocation? imageLocation;

    [DependencyProperty(DefaultValue = 16)]
    private int imageWidth;

    [DependencyProperty(DefaultValue = 16)]
    private int imageHeight;

    [DependencyProperty(DefaultValue = 5d)]
    private double imageContentSpacing;

    [DependencyProperty(DefaultValue = 0d)]
    private double imageBorderSpacing;

    [DependencyProperty]
    private ImageSource imageSource;

    [DependencyProperty(DefaultValue = "")]
    private string svgImageSource;

    [DependencyProperty]
    private Color? svgImageOverrideColor;

    [DependencyProperty]
    private int rowIndex;

    [DependencyProperty]
    private int columnIndex;

    [DependencyProperty(DefaultValue = LoadingIndicatorType.ArcsRing)]
    private LoadingIndicatorType loadingIndicatorMode;

    [DependencyProperty]
    private bool isBusy;

    static ImageButton()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(ImageButton),
            new FrameworkPropertyMetadata(typeof(ImageButton)));
    }

    public ImageButton()
    {
        SetCurrentValue(ImageLocationProperty, Controls.ImageLocation.Left);
    }

    private static void OnImageLocationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var imageButton = (ImageButton)d;
        var newLocation = (ImageLocation?)e.NewValue ?? Controls.ImageLocation.Left;

        switch (newLocation)
        {
            case Controls.ImageLocation.Left:
                imageButton.SetCurrentValue(RowIndexProperty, 1);
                imageButton.SetCurrentValue(ColumnIndexProperty, 0);
                break;
            case Controls.ImageLocation.Top:
                imageButton.SetCurrentValue(RowIndexProperty, 0);
                imageButton.SetCurrentValue(ColumnIndexProperty, 1);
                break;
            case Controls.ImageLocation.Right:
                imageButton.SetCurrentValue(RowIndexProperty, 1);
                imageButton.SetCurrentValue(ColumnIndexProperty, 2);
                break;
            case Controls.ImageLocation.Bottom:
                imageButton.SetCurrentValue(RowIndexProperty, 2);
                imageButton.SetCurrentValue(ColumnIndexProperty, 1);
                break;
            case Controls.ImageLocation.Center:
                imageButton.SetCurrentValue(RowIndexProperty, 1);
                imageButton.SetCurrentValue(ColumnIndexProperty, 1);
                break;
            default:
                throw new SwitchCaseDefaultException(newLocation);
        }
    }
}