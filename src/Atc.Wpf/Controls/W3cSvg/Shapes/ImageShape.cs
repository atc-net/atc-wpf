using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

// ReSharper disable StringLiteralTypo
namespace Atc.Wpf.Controls.W3cSvg.Shapes
{
    internal class ImageShape : Shape
    {
        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
        public ImageShape(Svg svg, XmlNode node)
            : base(svg, node)
        {
            if (svg == null)
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
                        if (svg.ExternalFileLoader != null)
                        {
                            imageStream = svg.ExternalFileLoader.LoadFile(hRef, svg.Filename);
                        }
                    }

                    if (imageStream == null)
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

        [SuppressMessage("Design", "MA0076:Do not use implicit culture-sensitive ToString in interpolated strings", Justification = "OK.")]
        public override string ToString() => $"{base.ToString()}, {nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Width)}: {Width}, {nameof(Height)}: {Height}, {nameof(ImageSource)}: {ImageSource}";
    }
}