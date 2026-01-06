namespace Atc.Wpf.Controls.Media.W3cSvg;

/// <summary>
/// This implements a markup extension that enables the creation
/// of <see cref="DrawingImage"/> from SVG sources.
/// </summary>
/// <remarks>
/// The SVG source file can be:
/// <list type="bullet">
/// <item>
/// <description>From the web</description>
/// </item>
/// <item>
/// <description>From the local computer (relative or absolute paths)</description>
/// </item>
/// <item>
/// <description>From the resources.</description>
/// </item>
/// </list>
/// <para>
/// The rendering settings are provided as properties for customizations.
/// </para>
/// </remarks>
[MarkupExtensionReturnType(typeof(DrawingImage))]
public sealed class SvgIconConverterExtension : SvgIconBase, IValueConverter
{
    private readonly UriTypeConverter uriConverter;
    private Uri? baseUri;

    /// <overloads>
    /// Initializes a new instance of the <see cref="SvgIconConverterExtension"/> class.
    /// </overloads>
    /// <summary>
    /// Initializes a new instance of the <see cref="SvgIconConverterExtension"/>
    /// class with the default parameters.
    /// </summary>
    public SvgIconConverterExtension()
    {
        uriConverter = new UriTypeConverter();
    }

    public SvgIconConverterExtension(Uri baseUri)
        : this()
    {
        this.baseUri = baseUri;
    }

    /// <summary>
    /// Performs the conversion of a valid SVG source file to the
    /// <see cref="DrawingImage"/> that is set as the value of the target
    /// property for this markup extension.
    /// </summary>
    /// <param name="serviceProvider">
    /// Object that can provide services for the markup extension.
    /// </param>
    /// <returns>
    /// This returns <see cref="DrawingImage"/> if successful; otherwise, it
    /// returns <see langword="null"/>.
    /// </returns>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        if (serviceProvider.GetService(typeof(IUriContext)) is IUriContext uriContext)
        {
            baseUri = uriContext.BaseUri;
        }

        return this;
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ApplicationName) && DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                GetAppName();
            }

            Uri? inputUri = null;
            if (parameter is not null)
            {
                inputUri = ResolveUri(parameter.ToString()!);
            }
            else if (value is not null)
            {
                inputUri = uriConverter.ConvertFrom(value) as Uri;
                if (inputUri is null || !inputUri.IsAbsoluteUri)
                {
                    inputUri = ResolveUri(value.ToString()!);
                }
            }

            if (inputUri is null || baseUri is null)
            {
                return null;
            }

            var svgSource = inputUri.IsAbsoluteUri
                ? inputUri
                : new Uri(baseUri, inputUri);
            return GetImage(svgSource);
        }
        catch
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return null;
            }
        }

        return null;
    }

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => null;

    /// <summary>
    /// Converts the SVG source file to <see cref="Uri"/>
    /// </summary>
    /// <param name="inputParameter">
    /// Object that can provide services for the markup extension.
    /// </param>
    /// <returns>
    /// Returns the valid <see cref="Uri"/> of the SVG source path if
    /// successful; otherwise, it returns <see langword="null"/>.
    /// </returns>
    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK - for now.")]
    private Uri? ResolveUri(string inputParameter)
    {
        if (string.IsNullOrWhiteSpace(inputParameter))
        {
            return null;
        }

        if (Uri.TryCreate(inputParameter, UriKind.RelativeOrAbsolute, out var svgSource))
        {
            if (svgSource.IsAbsoluteUri)
            {
                return svgSource;
            }

            // Try getting a local file in the same directory....
            var svgPath = inputParameter;
            if (inputParameter[0] == '\\' || inputParameter[0] == '/')
            {
                svgPath = inputParameter.Substring(1);
            }

            svgPath = svgPath.Replace('/', '\\');

            var assembly = GetExecutingAssembly();
            if (assembly is not null)
            {
                var location = Path.GetDirectoryName(assembly.Location);
                if (location is not null)
                {
                    var localFile = Path.Combine(location, svgPath);
                    if (File.Exists(localFile))
                    {
                        return new Uri(localFile);
                    }
                }
            }

            // Try getting it as resource file...
            if (uriConverter.ConvertFrom(inputParameter) is Uri inputUri)
            {
                if (inputUri.IsAbsoluteUri)
                {
                    return inputUri;
                }

                if (baseUri is not null)
                {
                    var validUri = new Uri(baseUri, inputUri);

                    return validUri;
                }
            }

            var asmName = ApplicationName;
            if (assembly is not null)
            {
                asmName = assembly.GetName().Name;
            }

            svgPath = inputParameter;
            if (inputParameter.StartsWith('/'))
            {
                svgPath = svgPath.TrimStart('/');
            }

            // A little hack to display preview in design mode
            var designTime = DesignerProperties.GetIsInDesignMode(new DependencyObject());
            if (designTime && !string.IsNullOrWhiteSpace(ApplicationName))
            {
                // The relative path is not working with the Converter...
                return new Uri($"pack://application:,,,/{ApplicationName};component/{svgPath}");
            }

            return new Uri($"pack://application:,,,/{asmName};component/{svgPath}");
        }

        return null;
    }
}