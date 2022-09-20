// ReSharper disable StringLiteralTypo
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable VirtualMemberNeverOverridden.Global
namespace Atc.Wpf.Controls.W3cSvg;

/// <summary>
/// This is an <see langword="abstract"/> implementation of a markup extension that enables the creation
/// of <see cref="DrawingImage"/> from SVG sources.
/// </summary>
[MarkupExtensionReturnType(typeof(DrawingImage))]
public abstract class SvgIconBase : MarkupExtension
{
    private string? applicationName;
    private CultureInfo? culture;

    /// <summary>
    /// Initializes a new instance of the <see cref="SvgIconBase"/>
    /// class with the default parameters.
    /// </summary>
    protected SvgIconBase()
    {
        GetAppName();
    }

    public Color? OverrideColor { get; set; }

    /// <summary>
    /// Gets or sets the main culture information used for rendering texts.
    /// </summary>
    /// <value>
    /// An instance of the <see cref="CultureInfo"/> specifying the main
    /// culture information for texts. The default is the English culture.
    /// </value>
    /// <remarks>
    /// <para>
    /// This is the culture information passed to the <see cref="FormattedText"/>
    /// class instance for the text rendering.
    /// </para>
    /// <para>
    /// The library does not currently provide any means of splitting texts
    /// into its multi-language parts.
    /// </para>
    /// </remarks>
    public CultureInfo? CultureInfo
    {
        get => culture;
        set
        {
            if (value is not null)
            {
                culture = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the project or application name of the target assembly.
    /// </summary>
    /// <value>
    /// A string specifying the application project name.
    /// </value>
    /// <remarks>
    /// This is optional and is only used to resolve the resource Uri at the design time.
    /// </remarks>
    public string? ApplicationName
    {
        get => applicationName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                GetAppName();
            }
            else
            {
                applicationName = value;
            }
        }
    }

    /// <summary>
    /// This converts the SVG resource specified by the Uri to <see cref="DrawingGroup"/>.
    /// </summary>
    /// <param name="svgSource">A <see cref="Uri"/> specifying the source of the SVG resource.</param>
    /// <returns>A <see cref="DrawingGroup"/> of the converted SVG resource.</returns>
    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK - for now.")]
    protected virtual DrawingGroup? GetDrawing(Uri svgSource)
    {
        ArgumentNullException.ThrowIfNull(svgSource);

        string scheme;
        var designTime = DesignerProperties.GetIsInDesignMode(new DependencyObject());
        if (designTime && !svgSource.IsAbsoluteUri)
        {
            scheme = "pack";
        }
        else
        {
            scheme = svgSource.Scheme;
        }

        if (string.IsNullOrWhiteSpace(scheme))
        {
            return null;
        }

        switch (scheme)
        {
            case "file":
            case "https":
            case "http":
                using (var reader = new SvgFileReader(OverrideColor))
                {
                    return reader.Read(svgSource);
                }

            case "pack":
                var svgStreamInfo = svgSource.ToString().IndexOf("siteoforigin", StringComparison.OrdinalIgnoreCase) != -1
                    ? Application.GetRemoteStream(svgSource)
                    : Application.GetResourceStream(svgSource);

                var svgStream = svgStreamInfo?.Stream;
                if (svgStream is not null)
                {
                    var fileExt = Path.GetExtension(svgSource.ToString());
                    var isCompressed = !string.IsNullOrWhiteSpace(fileExt) &&
                                       string.Equals(fileExt, ".svgz", StringComparison.OrdinalIgnoreCase);

                    if (isCompressed)
                    {
                        using var stream = svgStream;
                        using var zipStream = new GZipStream(svgStream, CompressionMode.Decompress);
                        using var reader = new SvgFileReader(OverrideColor);
                        var drawGroup = reader.Read(zipStream);
                        if (drawGroup is not null)
                        {
                            return drawGroup;
                        }
                    }
                    else
                    {
                        using var stream = svgStream;
                        using var reader = new SvgFileReader(OverrideColor);
                        var drawGroup = reader.Read(svgStream);
                        if (drawGroup is not null)
                        {
                            return drawGroup;
                        }
                    }
                }

                break;
            case "data":
                var sourceData = svgSource.OriginalString.Replace(" ", string.Empty, StringComparison.Ordinal);
                var nColon = sourceData.IndexOf(":", StringComparison.OrdinalIgnoreCase);
                var nSemiColon = sourceData.IndexOf(";", StringComparison.OrdinalIgnoreCase);
                var nComma = sourceData.IndexOf(",", StringComparison.OrdinalIgnoreCase);
                var sMimeType = sourceData.Substring(nColon + 1, nSemiColon - nColon - 1);
                var sEncoding = sourceData.Substring(nSemiColon + 1, nComma - nSemiColon - 1);
                if (string.Equals(sMimeType.Trim(), "image/svg+xml", StringComparison.OrdinalIgnoreCase)
                    && string.Equals(sEncoding.Trim(), "base64", StringComparison.OrdinalIgnoreCase))
                {
                    var sContent = SvgFileReader.RemoveWhitespace(sourceData.Substring(nComma + 1));
                    var imageBytes = Convert.FromBase64CharArray(sContent.ToCharArray(), 0, sContent.Length);
                    var isGZipped = sContent.StartsWith(SvgFileReader.GZipSignature, StringComparison.Ordinal);
                    if (isGZipped)
                    {
                        using var stream = new MemoryStream(imageBytes);
                        using var zipStream = new GZipStream(stream, CompressionMode.Decompress);
                        using var reader = new SvgFileReader(OverrideColor);
                        var drawGroup = reader.Read(zipStream);
                        if (drawGroup is not null)
                        {
                            return drawGroup;
                        }
                    }
                    else
                    {
                        using var stream = new MemoryStream(imageBytes);
                        using var reader = new SvgFileReader(OverrideColor);
                        var drawGroup = reader.Read(stream);
                        if (drawGroup is not null)
                        {
                            return drawGroup;
                        }
                    }
                }

                break;
        }

        return null;
    }

    /// <summary>
    /// This converts the SVG resource specified by the Uri to <see cref="DrawingImage"/>.
    /// </summary>
    /// <param name="svgSource">A <see cref="Uri"/> specifying the source of the SVG resource.</param>
    /// <returns>A <see cref="DrawingImage"/> of the converted SVG resource.</returns>
    /// <remarks>
    /// This uses the <see cref="GetDrawing(Uri)"/> method to convert the SVG resource to <see cref="DrawingGroup"/>,
    /// which is then wrapped in <see cref="DrawingImage"/>.
    /// </remarks>
    protected virtual DrawingImage? GetImage(Uri svgSource)
    {
        var drawGroup = GetDrawing(svgSource);
        return drawGroup is null
            ? null
            : new DrawingImage(drawGroup);
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
    protected void GetAppName()
    {
        try
        {
            var asm = GetEntryAssembly();
            if (asm is not null)
            {
                applicationName = asm.GetName().Name;
            }
        }
        catch
        {
            // Dummy
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
    protected static Assembly? GetEntryAssembly()
    {
        var xDesProc = "XDesProc"; // WPF designer process
        Assembly? asm = null;
        try
        {
            asm = Assembly.GetEntryAssembly();
            if (asm is not null)
            {
                var assemblyName = asm.GetName().Name;
                if (string.Equals(assemblyName, xDesProc, StringComparison.OrdinalIgnoreCase))
                {
                    asm = null;
                }
            }

            if (asm is null)
            {
                asm = (
                        from assembly in AppDomain.CurrentDomain.GetAssemblies()
                        where !assembly.IsDynamic
                        let assemblyName = Path.GetFileName(assembly.Location).Trim()
                        where assemblyName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
                              && !string.Equals(assemblyName, "XDesProc.exe", StringComparison.OrdinalIgnoreCase)
                        select assembly
                    )
                    .FirstOrDefault();

                if (asm is null)
                {
                    asm = Application.ResourceAssembly;
                    if (asm is not null)
                    {
                        var assemblyName = asm.GetName().Name;
                        if (string.Equals(assemblyName, xDesProc, StringComparison.OrdinalIgnoreCase))
                        {
                            asm = null;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            if (asm is null)
            {
                asm = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                        where !assembly.IsDynamic
                        let assemblyName = Path.GetFileName(assembly.Location).Trim()
                        where assemblyName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
                              && !string.Equals(assemblyName, "XDesProc.exe", StringComparison.OrdinalIgnoreCase)
                        select assembly
                    )
                    .FirstOrDefault();
            }

            Trace.TraceError(ex.ToString());
        }

        return asm;
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
    protected static Assembly? GetExecutingAssembly()
    {
        Assembly? asm;
        try
        {
            asm = Assembly.GetExecutingAssembly();
        }
        catch
        {
            asm = Assembly.GetEntryAssembly();
        }

        return asm;
    }
}