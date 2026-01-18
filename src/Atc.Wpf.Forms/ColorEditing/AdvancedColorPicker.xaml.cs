namespace Atc.Wpf.Forms.ColorEditing;

public partial class AdvancedColorPicker : INotifyPropertyChanged
{
    private double hue;
    private double saturation = 1;
    private double brightness = 1;
    private byte red;
    private byte green;
    private byte blue;
    private byte alpha = 255;
    private bool updateFromColor;
    private bool updateFromComponents;

    [DependencyProperty(DefaultValue = true)]
    private bool showSaturationBrightnessPicker;

    [DependencyProperty(DefaultValue = true)]
    private bool showAvailableColors;

    [DependencyProperty(DefaultValue = true)]
    private bool showStandardColors;

    [DependencyProperty(DefaultValue = true)]
    private bool showHueSlider;

    [DependencyProperty(DefaultValue = true)]
    private bool showTransparencySlider;

    [DependencyProperty(DefaultValue = true)]
    private bool showBeforeAfterColorResult;

    [DependencyProperty(DefaultValue = true)]
    private bool showHsv;

    [DependencyProperty(DefaultValue = true)]
    private bool showRgba;

    [DependencyProperty(DefaultValue = true)]
    private bool showArgb;

    [DependencyProperty(
        DefaultValue = nameof(Colors.Red),
        PropertyChangedCallback = nameof(OnOldColorChanged))]
    private Color originalColor;

    [DependencyProperty(
        DefaultValue = nameof(Colors.Red),
        PropertyChangedCallback = nameof(OnColorChanged))]
    private Color color;

    public event PropertyChangedEventHandler? PropertyChanged;

    public AdvancedColorPicker()
    {
        InitializeComponent();

        DataContext = this;

        var wellKnownColorPickers = this.FindChildren<WellKnownColorPicker>();
        foreach (var wellKnownColorPicker in wellKnownColorPickers)
        {
            wellKnownColorPicker.ColorChanged += OnWellKnownColorPickerChanged;
        }
    }

    public SolidColorBrush ColorAsBrush => new(Color);

    public double Hue
    {
        get => hue;
        set
        {
            if (value.Equals(hue))
            {
                return;
            }

            hue = value;
            OnPropertyChanged();

            if (!updateFromColor &&
                !updateFromComponents)
            {
                UpdateColorFromHsb();
            }
        }
    }

    public double Saturation
    {
        get => saturation;
        set
        {
            if (value.Equals(saturation))
            {
                return;
            }

            saturation = value;
            OnPropertyChanged();

            if (!updateFromColor &&
                !updateFromComponents)
            {
                UpdateColorFromHsb();
            }
        }
    }

    public double Brightness
    {
        get => brightness;
        set
        {
            if (value.Equals(brightness))
            {
                return;
            }

            brightness = value;
            OnPropertyChanged();

            if (!updateFromColor &&
                !updateFromComponents)
            {
                UpdateColorFromHsb();
            }
        }
    }

    public byte Red
    {
        get => red;
        set
        {
            if (value == red)
            {
                return;
            }

            red = value;
            OnPropertyChanged();

            if (!updateFromColor && !updateFromComponents)
            {
                UpdateColorFromRgb();
            }
        }
    }

    public byte Green
    {
        get => green;
        set
        {
            if (value == green)
            {
                return;
            }

            green = value;
            OnPropertyChanged();

            if (!updateFromColor && !updateFromComponents)
            {
                UpdateColorFromRgb();
            }
        }
    }

    public byte Blue
    {
        get => blue;
        set
        {
            if (value == blue)
            {
                return;
            }

            blue = value;
            OnPropertyChanged();

            if (!updateFromColor && !updateFromComponents)
            {
                UpdateColorFromRgb();
            }
        }
    }

    public byte Alpha
    {
        get => alpha;
        set
        {
            if (value == alpha)
            {
                return;
            }

            alpha = value;
            OnPropertyChanged();

            if (!updateFromColor && !updateFromComponents)
            {
                UpdateColorFromRgb();
            }
        }
    }

    protected virtual void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private static void OnOldColorChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (AdvancedColorPicker)d;

        control.SetCurrentValue(ColorProperty, control.OriginalColor);
    }

    private static void OnColorChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (AdvancedColorPicker)d;

        if (!control.updateFromComponents)
        {
            control.UpdateComponents();
        }
    }

    private void OnWellKnownColorPickerChanged(
        object? sender,
        ValueChangedEventArgs<Color> e)
        => Color = e.NewValue;

    private void UpdateColorFromRgb()
    {
        updateFromComponents = true;

        Color = Color.FromArgb(Alpha, Red, Green, Blue);
        Hue = Color.GetHue();
        Saturation = Color.GetSaturation();
        Brightness = Color.GetBrightness();

        updateFromComponents = false;

        OnPropertyChanged(nameof(ColorAsBrush));
    }

    private void UpdateColorFromHsb()
    {
        updateFromComponents = true;

        var c = ColorHelper.GetColorFromHsv(Hue, Saturation, Brightness);
        c.A = Alpha;

        Color = c;
        Red = Color.R;
        Green = Color.G;
        Blue = Color.B;

        updateFromComponents = false;

        OnPropertyChanged(nameof(ColorAsBrush));
    }

    private void UpdateComponents()
    {
        updateFromColor = true;

        Red = Color.R;
        Green = Color.G;
        Blue = Color.B;
        Alpha = Color.A;

        Hue = Color.GetHue();
        Saturation = Color.GetSaturation();
        Brightness = Color.GetBrightness();

        updateFromColor = false;

        OnPropertyChanged(nameof(ColorAsBrush));
    }
}