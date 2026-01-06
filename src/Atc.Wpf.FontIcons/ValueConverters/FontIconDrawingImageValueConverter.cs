namespace Atc.Wpf.FontIcons.ValueConverters;

public sealed class FontIconDrawingImageValueConverter : MarkupExtension, IValueConverter
{
    public static readonly FontIconDrawingImageValueConverter Instance = new();

    public override object ProvideValue(IServiceProvider serviceProvider)
        => this;

    public object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture)
    {
        var brush = parameter as Brush ?? Brushes.Black;
        var emSize = parameter as double? ?? 100;
        return value switch
        {
            FontAwesomeRegularType type => ImageAwesomeRegular.CreateDrawingImage(
                type,
                brush,
                emSize),
            FontAwesomeSolidType type => ImageAwesomeSolid.CreateDrawingImage(
                type,
                brush,
                emSize),
            FontAwesomeBrandType type => ImageAwesomeBrand.CreateDrawingImage(
                type,
                brush,
                emSize),
            FontBootstrapType bootstrapType => ImageBootstrap.CreateDrawingImage(
                bootstrapType,
                brush,
                emSize),
            IcoFontType icoFontType => ImageIcoFont.CreateDrawingImage(
                icoFontType,
                brush,
                emSize),
            FontMaterialDesignType materialDesignType => ImageMaterialDesign.CreateDrawingImage(
                materialDesignType,
                brush,
                emSize),
            FontWeatherType weatherType => ImageWeather.CreateDrawingImage(
                weatherType,
                brush,
                emSize),
            _ => null,
        };
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}