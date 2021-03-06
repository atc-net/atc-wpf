using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using Atc.Data.Models;
using Atc.Wpf.Controls.W3cSvg.FileLoaders;
using Atc.Wpf.Controls.W3cSvg.PaintServers;
using Atc.Wpf.Controls.W3cSvg.Shapes;
using Group = Atc.Wpf.Controls.W3cSvg.Shapes.Group;

namespace Atc.Wpf.Controls.W3cSvg
{
    /// <summary>
    /// This is the class that reads and parses the XML file.
    /// </summary>
    internal class Svg
    {
        private List<Shape>? elements;
        private Dictionary<string, Brush>? customBrushes;

        public Svg()
        {
            this.Size = new Size(300, 150);
            this.Filename = string.Empty;
            this.Shapes = new Dictionary<string, Shape>(StringComparer.Ordinal);
            this.StyleItems = new Dictionary<string, List<KeyValueItem>>(StringComparer.Ordinal);
            this.PaintServers.Parse("black");
        }

        public Svg(IExternalFileLoader externalFileLoader)
            : this()
        {
            this.ExternalFileLoader = externalFileLoader;
        }

        public Svg(string fileName)
            : this(fileName, FileSystemLoader.Instance)
        {
        }

        public Svg(string fileName, IExternalFileLoader externalFileLoader)
            : this()
        {
            this.ExternalFileLoader = externalFileLoader;
            this.Load(fileName);
        }

        public Svg(Stream stream)
            : this(stream, FileSystemLoader.Instance)
        {
        }

        public Svg(Stream stream, IExternalFileLoader externalFileLoader)
            : this()
        {
            this.ExternalFileLoader = externalFileLoader;
            this.Load(stream);
        }

        public Svg(XmlNode svgTag, IExternalFileLoader externalFileLoader)
            : this()
        {
            this.ExternalFileLoader = externalFileLoader;
            this.Load(svgTag);
        }

        public Dictionary<string, Shape> Shapes { get; }

        public Dictionary<string, List<KeyValueItem>> StyleItems { get; }

        public string Filename { get; private set; }

        public Rect? ViewBox { get; set; }

        public Size Size { get; set; }

        public IExternalFileLoader? ExternalFileLoader { get; }

        public Dictionary<string, Brush>? CustomBrushes
        {
            get => customBrushes;
            set
            {
                customBrushes = value;
                if (customBrushes == null)
                {
                    return;
                }

                foreach (var (key, brush) in customBrushes)
                {
                    PaintServers.CreateServerFromBrush(key, brush);
                }
            }
        }

        internal IDictionary<string, List<KeyValueItem>> Styles => StyleItems;

        public void AddShape(string id, Shape shape)
        {
            this.Shapes[id] = shape;
        }

        public Shape? GetShape(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            this.Shapes.TryGetValue(id, out var shape);
            return shape;
        }

        public PaintServerManager PaintServers { get; } = new PaintServerManager();

        public IList<Shape> Elements
        {
            get
            {
                if (elements == null)
                {
                    return new List<Shape>();
                }

                return elements.AsReadOnly();
            }
        }

        public void LoadXml(string fileXml)
        {
            this.Filename = string.Empty;
            if (string.IsNullOrWhiteSpace(fileXml))
            {
                return;
            }

            var doc = new XmlDocument();
            doc.LoadXml(fileXml);

            this.Load(doc);
        }

        public void Load(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                this.Filename = string.Empty;
                return;
            }

            this.Filename = filePath;

            var fileUri = new UriBuilder(filePath);
            if (string.Equals(fileUri.Scheme, "file", StringComparison.Ordinal))
            {
                string fileExt = Path.GetExtension(filePath);
                if (string.Equals(fileExt, ".svgz", StringComparison.OrdinalIgnoreCase))
                {
                    using var fileStream = File.OpenRead(fileUri.Uri.LocalPath);
                    using var zipStream = new GZipStream(fileStream, CompressionMode.Decompress);
                    this.Load(zipStream);
                    return;
                }
            }

            var doc = new XmlDocument();
            doc.Load(filePath);
            this.Load(doc);
        }

        public void Load(Uri fileUri)
        {
            if (fileUri == null)
            {
                throw new ArgumentNullException(nameof(fileUri));
            }

            if (!fileUri.IsAbsoluteUri)
            {
                return;
            }

            var filePath = fileUri.IsFile
                ? fileUri.LocalPath
                : fileUri.AbsoluteUri;

            if (string.IsNullOrWhiteSpace(filePath))
            {
                this.Filename = string.Empty;
                return;
            }

            if (fileUri.IsFile && !File.Exists(filePath))
            {
                this.Filename = string.Empty;
                return;
            }

            this.Filename = filePath;
            var doc = new XmlDocument();
            doc.Load(filePath);
            this.Load(doc);
        }

        public void Load(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var doc = new XmlDocument();
            doc.Load(stream);
            this.Load(doc);
        }

        public void Load(XmlReader xmlReader)
        {
            if (xmlReader == null)
            {
                throw new ArgumentNullException(nameof(xmlReader));
            }

            var doc = new XmlDocument();
            doc.Load(xmlReader);
            this.Load(doc);
        }

        public void Load(TextReader txtReader)
        {
            if (txtReader == null)
            {
                throw new ArgumentNullException(nameof(txtReader));
            }

            var doc = new XmlDocument();
            doc.Load(txtReader);
            this.Load(doc);
        }

        private void Load(XmlDocument svgDocument)
        {
            if (svgDocument == null)
            {
                throw new ArgumentNullException(nameof(svgDocument));
            }

            this.LoadStyles(svgDocument);
            var svgTags = svgDocument.GetElementsByTagName("svg");
            if (svgTags.Count == 0)
            {
                return;
            }

            var svgTag = svgTags[0];
            elements = this.Parse(svgTag);
        }

        private void Load(XmlNode svgTag)
        {
            if (svgTag == null)
            {
                throw new ArgumentNullException(nameof(svgTag));
            }

            this.LoadStyles(svgTag);
            elements = this.Parse(svgTag);
        }

        [SuppressMessage("Security", "MA0009:Add regex evaluation timeout", Justification = "OK.")]
        private void LoadStyles(XmlNode doc)
        {
            if (this.ExternalFileLoader == null)
            {
                return;
            }

            var cssUrlNodes = (from XmlNode childNode in doc.ChildNodes
                               where childNode.NodeType == XmlNodeType.ProcessingInstruction &&
                                     childNode.Name == "xml-stylesheet"
                               select (XmlProcessingInstruction)childNode)
                .ToList();

            if (cssUrlNodes.Count == 0)
            {
                return;
            }

            foreach (var node in cssUrlNodes)
            {
                var url = Regex.Match(node.Data, "href=\"(?<url>.*?)\"").Groups["url"].Value;
                var stream = this.ExternalFileLoader.LoadFile(url, this.Filename);
                if (stream == null)
                {
                    continue;
                }

                using var streamTmp = stream;
                using var reader = new StreamReader(stream, Encoding.UTF8);
                var style = reader.ReadToEnd();
                CssStyleParser.ParseStyle(this, style);
            }
        }

        private List<Shape> Parse(XmlNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            var vb = node.Attributes?.GetNamedItem("viewBox");
            if (vb?.Value != null)
            {
                var cord = vb.Value.Split(' ');
                var cult = CultureInfo.InvariantCulture;
                this.ViewBox = new Rect(
                    double.Parse(cord[0], cult),
                    double.Parse(cord[1], cult),
                    double.Parse(cord[2], cult),
                    double.Parse(cord[3], cult));
            }

            this.Size = new Size(
                SvgXmlUtil.AttrValue(node, "width", 300),
                SvgXmlUtil.AttrValue(node, "height", 150));

            var lstElements = new List<Shape>();
            if (node.Name != SvgTagConstants.Svg && node.Name != SvgTagConstants.Pattern)
            {
                throw new FormatException("Not a valid SVG node");
            }

            foreach (XmlNode childNode in node.ChildNodes)
            {
                Group.AddToList(this, lstElements, childNode, parent: null);
            }

            return lstElements;
        }
    }
}