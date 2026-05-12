namespace Atc.Wpf.FontIcons.ValueConverters;

/// <summary>
/// ValueConverter: font-icon enum value (FontAwesome / Bootstrap / IcoFont / Material / Weather)
/// → <see cref="System.Windows.Media.ImageSource"/>. <c>ConverterParameter</c> may be a
/// <see cref="System.Windows.Media.Brush"/> (foreground) or a <see cref="double"/> em-size;
/// otherwise falls back to <see cref="DefaultBrush"/> at <see cref="DefaultEmSize"/>em.
/// </summary>
/// <remarks>
/// Built-in defaults: <c>Brushes.Black</c> at <c>100</c>em. Override via the static properties
/// at application startup to retheme all FontIcon usages library-wide.
/// </remarks>
public sealed class FontIconImageSourceValueConverter : MarkupExtension, IValueConverter
{
    public static readonly FontIconImageSourceValueConverter Instance = new();

    /// <summary>The built-in default foreground brush (<c>Brushes.Black</c>).</summary>
    public static readonly Brush BuiltInDefaultBrush = Brushes.Black;

    /// <summary>The built-in default em-size (<c>100</c>).</summary>
    public static readonly double BuiltInDefaultEmSize = 100;

    /// <summary>
    /// Default foreground brush used when <c>ConverterParameter</c> is not a <see cref="Brush"/>.
    /// </summary>
    public static Brush DefaultBrush { get; set; } = BuiltInDefaultBrush;

    /// <summary>
    /// Default em-size used when <c>ConverterParameter</c> is not a <see cref="double"/>.
    /// </summary>
    public static double DefaultEmSize { get; set; } = BuiltInDefaultEmSize;

    /// <summary>Restores <see cref="DefaultBrush"/> and <see cref="DefaultEmSize"/> to their built-in values.</summary>
    public static void ResetToDefaults()
    {
        DefaultBrush = BuiltInDefaultBrush;
        DefaultEmSize = BuiltInDefaultEmSize;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
        => this;

    public object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture)
    {
        var brush = parameter as Brush ?? DefaultBrush;
        var emSize = parameter as double? ?? DefaultEmSize;
        return value switch
        {
            FontAwesomeRegularType type => ImageAwesomeRegular.CreateImageSource(
                type,
                brush,
                emSize),
            FontAwesomeSolidType type => ImageAwesomeSolid.CreateImageSource(
                type,
                brush,
                emSize),
            FontAwesomeBrandType type => ImageAwesomeBrand.CreateImageSource(
                type,
                brush,
                emSize),
            FontAwesomeRegular7Type type => ImageAwesomeRegular7.CreateImageSource(
                type,
                brush,
                emSize),
            FontAwesomeSolid7Type type => ImageAwesomeSolid7.CreateImageSource(
                type,
                brush,
                emSize),
            FontAwesomeBrand7Type type => ImageAwesomeBrand7.CreateImageSource(
                type,
                brush,
                emSize),
            FontBootstrapType bootstrapType => ImageBootstrap.CreateImageSource(
                bootstrapType,
                brush,
                emSize),
            IcoFontType icoFontType => ImageIcoFont.CreateImageSource(
                icoFontType,
                brush,
                emSize),
            FontMaterialDesignType materialDesignType => ImageMaterialDesign.CreateImageSource(
                materialDesignType,
                brush,
                emSize),
            FontWeatherType weatherType => ImageWeather.CreateImageSource(
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