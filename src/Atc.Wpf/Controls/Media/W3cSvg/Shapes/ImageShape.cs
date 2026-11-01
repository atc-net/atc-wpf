// ReSharper disable StringLiteralTypo
namespace Atc.Wpf.Controls.Media.W3cSvg.Shapes;

internal sealed class ImageShape : Shape
{
    private const string XLinkNamespace = "http://www.w3.org/1999/xlink";

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
    public ImageShape(
        Svg svg,
        XmlNode node)
        : base(svg, node)
    {
        ArgumentNullException.ThrowIfNull(svg);

        X = SvgXmlUtil.AttrValue(
            node,
            "x",
            0,
            svg.Size.Width);
        Y = SvgXmlUtil.AttrValue(
            node,
            "y",
            0,
            svg.Size.Height);
        Width = SvgXmlUtil.AttrValue(
            node,
            "width",
            0,
            svg.Size.Width);
        Height = SvgXmlUtil.AttrValue(
            node,
            "height",
            0,
            svg.Size.Height);
        var hRef = GetHref(node);
        if (!string.IsNullOrEmpty(hRef))
        {
            try
            {
                ImageSource = LoadImage(
                    svg,
                    hRef);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }
    }

    public double X { get; }

    public double Y { get; }

    public double Width { get; }

    public double Height { get; }

    public ImageSource? ImageSource { get; }

    /// <summary>
    /// Reads the SVG 2 <c>href</c>, falling back to the SVG 1.1 <c>xlink:href</c>.
    /// </summary>
    private static string? GetHref(XmlNode node)
    {
        var attributes = node.Attributes;
        if (attributes is null)
        {
            return null;
        }

        return attributes["href"]?.Value
               ?? attributes["href", XLinkNamespace]?.Value
               ?? attributes["xlink:href"]?.Value;
    }

    /// <summary>
    /// Loads an embedded <c>data:image/*;base64</c> image or an external image file. The image is decoded
    /// right away, so the stream is closed (an external file is not kept locked), and frozen.
    /// </summary>
    private static BitmapImage? LoadImage(
        Svg svg,
        string hRef)
    {
        Stream? imageStream = null;
        if (hRef.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
        {
            var commaIndex = hRef.IndexOf(',', StringComparison.Ordinal);
            if (commaIndex < 0)
            {
                return null;
            }

            var header = hRef.Substring(0, commaIndex);
            if (!header.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase) ||
                !header.EndsWith(";base64", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var embeddedImage = SvgFileReader.RemoveWhitespace(hRef.Substring(commaIndex + 1));
            if (string.IsNullOrEmpty(embeddedImage))
            {
                return null;
            }

            imageStream = new MemoryStream(Convert.FromBase64String(embeddedImage));
        }
        else if (svg.ExternalFileLoader is not null)
        {
            imageStream = svg.ExternalFileLoader.LoadFile(
                hRef,
                svg.Filename);
        }

        if (imageStream is null)
        {
            return null;
        }

        using (imageStream)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = imageStream;
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            return bitmapImage;
        }
    }

    public override string ToString()
        => $"{base.ToString()}, {nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Width)}: {Width}, {nameof(Height)}: {Height}, {nameof(ImageSource)}: {ImageSource}";
}