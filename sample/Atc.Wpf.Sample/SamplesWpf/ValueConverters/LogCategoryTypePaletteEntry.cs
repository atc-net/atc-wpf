namespace Atc.Wpf.Sample.SamplesWpf.ValueConverters;

/// <summary>
/// Row view-model for the LogCategoryType-palette samples.
/// Setting <see cref="Color"/> pushes through to <see cref="LogCategoryTypeToColorValueConverter"/>;
/// <see cref="Brush"/> and <see cref="HexCode"/> re-notify so bindings update.
/// </summary>
public class LogCategoryTypePaletteEntry : ViewModelBase
{
    private Color color;

    public LogCategoryTypePaletteEntry(LogCategoryType category)
    {
        Category = category;
        color = LogCategoryTypeToColorValueConverter.GetColor(category);
    }

    public LogCategoryType Category { get; }

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
            LogCategoryTypeToColorValueConverter.SetColor(Category, value);
            RaiseChangeNotifications();
        }
    }

    public SolidColorBrush Brush
        => LogCategoryTypeToBrushValueConverter.GetBrush(Category);

    public string HexCode
        => Color.ToString(CultureInfo.InvariantCulture);

    public void RefreshFromPalette()
    {
        var paletteColor = LogCategoryTypeToColorValueConverter.GetColor(Category);
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