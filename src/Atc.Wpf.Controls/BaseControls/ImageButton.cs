using Atc.Wpf.Controls.Progressing;

namespace Atc.Wpf.Controls.BaseControls;

public class ImageButton : Button
{
    public static readonly DependencyProperty ImageWidthProperty = DependencyProperty.Register(
        nameof(ImageWidth),
        typeof(int),
        typeof(ImageButton),
        new PropertyMetadata(16));

    public int ImageWidth
    {
        get => (int)GetValue(ImageWidthProperty);
        set => SetValue(ImageWidthProperty, value);
    }

    public static readonly DependencyProperty ImageHeightProperty = DependencyProperty.Register(
        nameof(ImageHeight),
        typeof(int),
        typeof(ImageButton),
        new PropertyMetadata(16));

    public int ImageHeight
    {
        get => (int)GetValue(ImageHeightProperty);
        set => SetValue(ImageHeightProperty, value);
    }

    public static readonly DependencyProperty ImageLocationProperty = DependencyProperty.Register(
        nameof(ImageLocation),
        typeof(ImageLocation?),
        typeof(ImageButton),
        new PropertyMetadata(
            defaultValue: null,
            PropertyChangedCallback));

    public ImageLocation? ImageLocation
    {
        get => (ImageLocation)GetValue(ImageLocationProperty);
        set => SetValue(ImageLocationProperty, value);
    }

    public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
        nameof(ImageSource),
        typeof(ImageSource),
        typeof(ImageButton),
        new PropertyMetadata(defaultValue: default));

    public ImageSource ImageSource
    {
        get => (ImageSource)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    public static readonly DependencyProperty SvgImageSourceProperty = DependencyProperty.Register(
        nameof(SvgImageSource),
        typeof(string),
        typeof(ImageButton),
        new PropertyMetadata(defaultValue: string.Empty));

    public string SvgImageSource
    {
        get => (string)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    public static readonly DependencyProperty SvgImageOverrideColorProperty = DependencyProperty.Register(
        nameof(SvgImageOverrideColor),
        typeof(Color?),
        typeof(ImageButton),
        new PropertyMetadata(default(Color?)));

    public Color? SvgImageOverrideColor
    {
        get => (Color?)GetValue(SvgImageOverrideColorProperty);
        set => SetValue(SvgImageOverrideColorProperty, value);
    }

    public static readonly DependencyProperty RowIndexProperty = DependencyProperty.Register(
        nameof(RowIndex),
        typeof(int),
        typeof(ImageButton),
        new PropertyMetadata(0));

    public int RowIndex
    {
        get => (int)GetValue(RowIndexProperty);
        set => SetValue(RowIndexProperty, value);
    }

    public static readonly DependencyProperty ColumnIndexProperty = DependencyProperty.Register(
        nameof(ColumnIndex),
        typeof(int),
        typeof(ImageButton),
        new PropertyMetadata(0));

    public int ColumnIndex
    {
        get => (int)GetValue(ColumnIndexProperty);
        set => SetValue(ColumnIndexProperty, value);
    }

    public static readonly DependencyProperty LoadingIndicatorModeProperty = DependencyProperty.Register(
        nameof(LoadingIndicatorMode),
        typeof(LoadingIndicatorType),
        typeof(ImageButton),
        new PropertyMetadata(LoadingIndicatorType.ArcsRing));

    public LoadingIndicatorType LoadingIndicatorMode
    {
        get => (LoadingIndicatorType)GetValue(LoadingIndicatorModeProperty);
        set => SetValue(LoadingIndicatorModeProperty, value);
    }

    public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register(
        nameof(IsBusy),
        typeof(bool),
        typeof(ImageButton),
        new PropertyMetadata(BooleanBoxes.FalseBox));

    public bool IsBusy
    {
        get => (bool)GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }

    static ImageButton()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(ImageButton),
            new FrameworkPropertyMetadata(typeof(ImageButton)));
    }

    public ImageButton()
    {
        this.SetCurrentValue(ImageLocationProperty, Controls.ImageLocation.Left);
    }

    private static void PropertyChangedCallback(
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