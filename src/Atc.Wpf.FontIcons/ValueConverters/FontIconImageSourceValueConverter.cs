namespace Atc.Wpf.FontIcons.ValueConverters;

public class FontIconImageSourceValueConverter : MarkupExtension, IValueConverter
{
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }

    public object? Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
    {
        var brush = parameter as Brush ?? Brushes.Black;
        var emSize = parameter as double? ?? 100;
        return value switch
        {
            FontAwesomeRegularType type => ImageAwesomeRegular.CreateImageSource(type, brush, emSize),
            FontAwesomeSolidType type => ImageAwesomeSolid.CreateImageSource(type, brush, emSize),
            FontAwesomeBrandType type => ImageAwesomeBrand.CreateImageSource(type, brush, emSize),
            FontBootstrapType bootstrapType => ImageBootstrap.CreateImageSource(bootstrapType, brush, emSize),
            IcoFontType icoFontType => ImageIcoFont.CreateImageSource(icoFontType, brush, emSize),
            FontMaterialDesignType materialDesignType => ImageMaterialDesign.CreateImageSource(materialDesignType, brush, emSize),
            FontWeatherType weatherType => ImageWeather.CreateImageSource(weatherType, brush, emSize),
            _ => null,
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("This is a OneWay converter.");
    }
}