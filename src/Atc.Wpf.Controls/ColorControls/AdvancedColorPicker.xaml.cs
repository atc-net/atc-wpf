namespace Atc.Wpf.Controls.ColorControls;

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

    public static readonly DependencyProperty ShowSaturationBrightnessPickerProperty = DependencyProperty.Register(
        nameof(ShowSaturationBrightnessPicker),
        typeof(bool),
        typeof(AdvancedColorPicker),
        new PropertyMetadata(BooleanBoxes.TrueBox));

    public bool ShowSaturationBrightnessPicker
    {
        get => (bool)GetValue(ShowSaturationBrightnessPickerProperty);
        set => SetValue(ShowSaturationBrightnessPickerProperty, value);
    }

    public static readonly DependencyProperty ShowAvailableColorsProperty = DependencyProperty.Register(
        nameof(ShowAvailableColors),
        typeof(bool),
        typeof(AdvancedColorPicker),
        new PropertyMetadata(BooleanBoxes.TrueBox));

    public bool ShowAvailableColors
    {
        get => (bool)GetValue(ShowAvailableColorsProperty);
        set => SetValue(ShowAvailableColorsProperty, value);
    }

    public static readonly DependencyProperty ShowStandardColorsProperty = DependencyProperty.Register(
        nameof(ShowStandardColors),
        typeof(bool),
        typeof(AdvancedColorPicker),
        new PropertyMetadata(BooleanBoxes.TrueBox));

    public bool ShowStandardColors
    {
        get => (bool)GetValue(ShowStandardColorsProperty);
        set => SetValue(ShowStandardColorsProperty, value);
    }

    public static readonly DependencyProperty ShowHueSliderProperty = DependencyProperty.Register(
        nameof(ShowHueSlider),
        typeof(bool),
        typeof(AdvancedColorPicker),
        new PropertyMetadata(BooleanBoxes.TrueBox));

    public bool ShowHueSlider
    {
        get => (bool)GetValue(ShowHueSliderProperty);
        set => SetValue(ShowHueSliderProperty, value);
    }

    public static readonly DependencyProperty ShowTransparencySliderProperty = DependencyProperty.Register(
        nameof(ShowTransparencySlider),
        typeof(bool),
        typeof(AdvancedColorPicker),
        new PropertyMetadata(BooleanBoxes.TrueBox));

    public bool ShowTransparencySlider
    {
        get => (bool)GetValue(ShowTransparencySliderProperty);
        set => SetValue(ShowTransparencySliderProperty, value);
    }

    public static readonly DependencyProperty ShowBeforeAfterColorResultProperty = DependencyProperty.Register(
        nameof(ShowBeforeAfterColorResult),
        typeof(bool),
        typeof(AdvancedColorPicker),
        new PropertyMetadata(BooleanBoxes.TrueBox));

    public bool ShowBeforeAfterColorResult
    {
        get => (bool)GetValue(ShowBeforeAfterColorResultProperty);
        set => SetValue(ShowBeforeAfterColorResultProperty, value);
    }

    public static readonly DependencyProperty ShowHsvProperty = DependencyProperty.Register(
        nameof(ShowHsv),
        typeof(bool),
        typeof(AdvancedColorPicker),
        new PropertyMetadata(BooleanBoxes.TrueBox));

    public bool ShowHsv
    {
        get => (bool)GetValue(ShowHsvProperty);
        set => SetValue(ShowHsvProperty, value);
    }

    public static readonly DependencyProperty ShowRgbaProperty = DependencyProperty.Register(
        nameof(ShowRgba),
        typeof(bool),
        typeof(AdvancedColorPicker),
        new PropertyMetadata(BooleanBoxes.TrueBox));

    public bool ShowRgba
    {
        get => (bool)GetValue(ShowRgbaProperty);
        set => SetValue(ShowRgbaProperty, value);
    }

    public static readonly DependencyProperty ShowArgbProperty = DependencyProperty.Register(
        nameof(ShowArgb),
        typeof(bool),
        typeof(AdvancedColorPicker),
        new PropertyMetadata(BooleanBoxes.TrueBox));

    public bool ShowArgb
    {
        get => (bool)GetValue(ShowArgbProperty);
        set => SetValue(ShowArgbProperty, value);
    }

    public static readonly DependencyProperty OriginalColorProperty = DependencyProperty.Register(
        nameof(OriginalColor),
        typeof(Color),
        typeof(AdvancedColorPicker),
        new PropertyMetadata(
            Colors.Red,
            OnOldColorChanged));

    public Color OriginalColor
    {
        get => (Color)GetValue(OriginalColorProperty);
        set => SetValue(OriginalColorProperty, value);
    }

    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
        nameof(Color),
        typeof(Color),
        typeof(AdvancedColorPicker),
        new PropertyMetadata(
            Colors.Red,
            OnColorChanged));

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

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