namespace Atc.Wpf.Sample.SamplesWpf.ValueConverters;

/// <summary>
/// Row view-model for the LogLevel-palette samples.
/// Setting <see cref="Color"/> pushes through to <see cref="LogLevelToColorValueConverter"/>;
/// <see cref="Brush"/> and <see cref="HexCode"/> re-notify so bindings update.
/// </summary>
public class LogLevelPaletteEntry : ViewModelBase
{
    private Color color;

    public LogLevelPaletteEntry(LogLevel level)
    {
        Level = level;
        color = LogLevelToColorValueConverter.GetColor(level);
    }

    public LogLevel Level { get; }

    public Color Color
    {
        get => color;
        set
        {
            if (color == value)
            {
                return;
            }

            color = value;
            LogLevelToColorValueConverter.SetColor(Level, value);
            RaiseChangeNotifications();
        }
    }

    public SolidColorBrush Brush
        => LogLevelToBrushValueConverter.GetBrush(Level);

    public string HexCode
        => Color.ToString(CultureInfo.InvariantCulture);

    public void RefreshFromPalette()
    {
        var paletteColor = LogLevelToColorValueConverter.GetColor(Level);
        if (color == paletteColor)
        {
            return;
        }

        color = paletteColor;
        RaiseChangeNotifications();
    }

    private void RaiseChangeNotifications()
    {
        OnPropertyChanged(nameof(Color));
        OnPropertyChanged(nameof(Brush));
        OnPropertyChanged(nameof(HexCode));
    }
}