namespace Atc.Wpf.Controls.W3cSvg;

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
public sealed class SvgIconExtension : SvgIconBase
{
    private string? svgPath;

    /// <overloads>
    /// Initializes a new instance of the <see cref="SvgIconExtension"/> class.
    /// </overloads>
    /// <summary>
    /// Initializes a new instance of the <see cref="SvgIconExtension"/>
    /// class with the default parameters.
    /// </summary>
    public SvgIconExtension()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SvgIconExtension"/>
    /// class with the specified SVG file path.
    /// </summary>
    /// <param name="svgPath">The SVG path.</param>
    public SvgIconExtension(string svgPath)
        : this()
    {
        this.svgPath = svgPath ?? throw new ArgumentNullException(nameof(svgPath));
    }

    /// <summary>
    /// Gets or sets the SVG source file.
    /// </summary>
    /// <value>
    /// A string specifying the path of the SVG source file.
    /// The default is <see langword="null"/>.
    /// </value>
    public string? Source
    {
        get => this.svgPath;
        set
        {
            this.svgPath = value;

            if (string.IsNullOrWhiteSpace(this.ApplicationName) &&
                DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                this.GetAppName();
            }
        }
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
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        if (serviceProvider is null)
        {
            throw new ArgumentNullException(nameof(serviceProvider));
        }

        try
        {
            if (string.IsNullOrWhiteSpace(this.ApplicationName) &&
                DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                this.GetAppName();
            }

            var svgSource = this.ResolveUri(serviceProvider);
            return svgSource is null
                ? null
                : this.GetImage(svgSource);
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

    /// <summary>
    /// Converts the SVG source file to <see cref="Uri"/>
    /// </summary>
    /// <param name="serviceProvider">
    /// Object that can provide services for the markup extension.
    /// </param>
    /// <returns>
    /// Returns the valid <see cref="Uri"/> of the SVG source path if
    /// successful; otherwise, it returns <see langword="null"/>.
    /// </returns>
    private Uri? ResolveUri(IServiceProvider serviceProvider)
    {
        if (string.IsNullOrWhiteSpace(svgPath))
        {
            return null;
        }

        if (Uri.TryCreate(svgPath, UriKind.RelativeOrAbsolute, out var svgSource))
        {
            if (svgSource.IsAbsoluteUri)
            {
                return svgSource;
            }

            var tmp = this.svgPath;
            if (this.svgPath[0] == '\\' || this.svgPath[0] == '/')
            {
                tmp = this.svgPath.Substring(1);
            }

            tmp = tmp.Replace('/', '\\');
            var assembly = GetExecutingAssembly();
            if (assembly is not null)
            {
                string localFile = Path.Combine(Path.GetDirectoryName(assembly.Location), tmp);

                if (File.Exists(localFile))
                {
                    return new Uri(localFile);
                }
            }

            if (serviceProvider.GetService(typeof(IUriContext)) is IUriContext uriContext && uriContext.BaseUri is not null)
            {
                return new Uri(uriContext.BaseUri, svgSource);
            }

            var asmName = this.ApplicationName;
            if (assembly is not null)
            {
                asmName = assembly.GetName().Name;
            }

            tmp = this.svgPath;
            if (this.svgPath.StartsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                tmp = tmp.TrimStart('/');
            }

            bool designTime = DesignerProperties.GetIsInDesignMode(new DependencyObject());
            if (designTime && !string.IsNullOrWhiteSpace(this.ApplicationName))
            {
                return new Uri($"/{this.ApplicationName};component/{tmp}", UriKind.Relative);
            }

            return new Uri($"pack://application:,,,/{asmName};component/{tmp}");
        }

        return null;
    }
}