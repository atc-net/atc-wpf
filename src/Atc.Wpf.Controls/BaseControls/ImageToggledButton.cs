namespace Atc.Wpf.Controls.BaseControls;

public sealed partial class ImageToggledButton : ImageButton
{
    [DependencyProperty(
        DefaultValue = false,
        PropertyChangedCallback = nameof(OnToggledChanged))]
    private bool isToggled;

    [DependencyProperty]
    private object? onContent;

    [DependencyProperty]
    private object? offContent;

    [DependencyProperty]
    private ImageSource? onImageSource;

    [DependencyProperty]
    private ImageSource? offImageSource;

    [DependencyProperty(DefaultValue = "")]
    private string onSvgImageSource;

    [DependencyProperty(DefaultValue = "")]
    private string offSvgImageSource;

    [DependencyProperty]
    private Color? onSvgImageOverrideColor;

    [DependencyProperty]
    private Color? offSvgImageOverrideColor;

    [DependencyProperty]
    private ICommand? onCommand;

    [DependencyProperty]
    private ICommand? offCommand;

    static ImageToggledButton()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(ImageToggledButton),
            new FrameworkPropertyMetadata(typeof(ImageButton)));
    }

    public ImageToggledButton()
    {
        // Initialise with the Off state visual when loaded.
        Loaded += (_, _) => ApplyVisualState(IsToggled);
    }

    protected override void OnClick()
    {
        base.OnClick();

        IsToggled = !IsToggled;

        ExecuteCurrentCommand();
    }

    private static void OnToggledChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ImageToggledButton)d;
        ctl.ApplyVisualState((bool)e.NewValue);
    }

    private void ApplyVisualState(
        bool toggled)
    {
        if (toggled)
        {
            // ON state
            if (OnContent is not null)
            {
                SetCurrentValue(ContentProperty, OnContent);
            }

            if (OnImageSource is not null)
            {
                SetCurrentValue(ImageSourceProperty, OnImageSource);
            }

            if (!string.IsNullOrEmpty(OnSvgImageSource))
            {
                SetCurrentValue(SvgImageSourceProperty, OnSvgImageSource);
            }

            if (OnSvgImageOverrideColor is not null)
            {
                SetCurrentValue(SvgImageOverrideColorProperty, OnSvgImageOverrideColor);
            }
        }
        else
        {
            // OFF state
            if (OffContent is not null)
            {
                SetCurrentValue(ContentProperty, OffContent);
            }

            if (OffImageSource is not null)
            {
                SetCurrentValue(ImageSourceProperty, OffImageSource);
            }

            if (!string.IsNullOrEmpty(OffSvgImageSource))
            {
                SetCurrentValue(SvgImageSourceProperty, OffSvgImageSource);
            }

            if (OffSvgImageOverrideColor is not null)
            {
                SetCurrentValue(SvgImageOverrideColorProperty, OffSvgImageOverrideColor);
            }
        }
    }

    private void ExecuteCurrentCommand()
    {
        var cmd = IsToggled
            ? OffCommand
            : OnCommand;

        if (cmd?.CanExecute(parameter: null) == true)
        {
            cmd.Execute(parameter: null);
        }
    }
}