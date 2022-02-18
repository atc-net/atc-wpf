// ReSharper disable StringLiteralTypo
namespace Atc.Wpf.Controls.W3cSvg.Shapes;

internal class ImageShape : Shape
{
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "OK for now")]
    public ImageShape(Svg svg, XmlNode node)
        : base(svg, node)
    {
        if (svg is null)
        {
            throw new ArgumentNullException(nameof(svg));
        }

        this.X = SvgXmlUtil.AttrValue(node, "x", 0, svg.Size.Width);
        this.Y = SvgXmlUtil.AttrValue(node, "y", 0, svg.Size.Height);
        this.Width = SvgXmlUtil.AttrValue(node, "width", 0, svg.Size.Width);
        this.Height = SvgXmlUtil.AttrValue(node, "height", 0, svg.Size.Height);
        var hRef = SvgXmlUtil.AttrValue(node, "xlink:href", string.Empty);
        if (!string.IsNullOrEmpty(hRef))
        {
            try
            {
                Stream? imageStream = null;
                if (hRef.StartsWith("data:image/png;base64", StringComparison.OrdinalIgnoreCase))
                {
                    var embeddedImage = hRef.Substring("data:image/png;base64,".Length);
                    if (!string.IsNullOrWhiteSpace(embeddedImage))
                    {
                        imageStream = new MemoryStream(Convert.FromBase64String(embeddedImage));
                    }
                }
                else
                {
                    if (svg.ExternalFileLoader is not null)
                    {
                        imageStream = svg.ExternalFileLoader.LoadFile(hRef, svg.Filename);
                    }
                }

                if (imageStream is null)
                {
                    return;
                }

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = imageStream;
                bitmapImage.EndInit();
                this.ImageSource = bitmapImage;
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

    public override string ToString() => $"{base.ToString()}, {nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Width)}: {Width}, {nameof(Height)}: {Height}, {nameof(ImageSource)}: {ImageSource}";
}