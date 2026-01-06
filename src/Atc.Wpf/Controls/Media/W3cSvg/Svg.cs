namespace Atc.Wpf.Controls.Media.W3cSvg;

/// <summary>
/// This is the class that reads and parses the XML file.
/// </summary>
[SuppressMessage("Maintainability", "S1144:Unused private types or members should be removed", Justification = "OK.")]
internal sealed class Svg
{
    private List<Shape>? elements;
    private Dictionary<string, Brush>? customBrushes;

    public Svg()
    {
        Size = new Size(
            width: 300,
            height: 150);
        Filename = string.Empty;
        Shapes = new Dictionary<string, Shape>(StringComparer.Ordinal);
        StyleItems = new Dictionary<string, List<KeyValueItem>>(StringComparer.Ordinal);
        PaintServers.Parse("black");
    }

    public Svg(IExternalFileLoader externalFileLoader)
        : this()
    {
        ExternalFileLoader = externalFileLoader;
    }

    public Svg(string fileName)
        : this(
            fileName,
            FileSystemLoader.Instance)
    {
    }

    public Svg(
        string fileName,
        IExternalFileLoader externalFileLoader)
        : this()
    {
        ExternalFileLoader = externalFileLoader;
        Load(fileName);
    }

    public Svg(Stream stream)
        : this(
            stream,
            FileSystemLoader.Instance)
    {
    }

    public Svg(
        Stream stream,
        IExternalFileLoader externalFileLoader)
        : this()
    {
        ExternalFileLoader = externalFileLoader;
        Load(stream);
    }

    public Svg(
        XmlNode svgTag,
        IExternalFileLoader externalFileLoader)
        : this()
    {
        ExternalFileLoader = externalFileLoader;
        Load(svgTag);
    }

    public Dictionary<string, Shape> Shapes { get; }

    public Dictionary<string, List<KeyValueItem>> StyleItems { get; }

    public string Filename { get; private set; }

    public Rect? ViewBox { get; set; }

    public Size Size { get; set; }

    public Brush? Fill { get; set; }

    public Brush? Stroke { get; set; }

    public IExternalFileLoader? ExternalFileLoader { get; }

    public Dictionary<string, Brush>? CustomBrushes
    {
        get => customBrushes;
        set
        {
            customBrushes = value;
            if (customBrushes is null)
            {
                return;
            }

            foreach (var (key, brush) in customBrushes)
            {
                PaintServers.CreateServerFromBrush(
                    key,
                    brush);
            }
        }
    }

    internal IDictionary<string, List<KeyValueItem>> Styles => StyleItems;

    public void AddShape(
        string id,
        Shape shape)
    {
        Shapes[id] = shape;
    }

    public Shape? GetShape(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        Shapes.TryGetValue(
            id,
            out var shape);
        return shape;
    }

    public PaintServerManager PaintServers { get; } = new();

    public IList<Shape> Elements
    {
        get
        {
            if (elements is null)
            {
                return new List<Shape>();
            }

            return elements.AsReadOnly();
        }
    }

    public void LoadXml(string fileXml)
    {
        Filename = string.Empty;
        if (string.IsNullOrWhiteSpace(fileXml))
        {
            return;
        }

        var doc = new XmlDocument();
        doc.LoadXml(fileXml);

        Load(doc);
    }

    public void Load(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        {
            Filename = string.Empty;
            return;
        }

        Filename = filePath;

        var fileUri = new UriBuilder(filePath);
        if (string.Equals(
                fileUri.Scheme,
                "file",
                StringComparison.Ordinal))
        {
            var fileExt = Path.GetExtension(filePath);
            if (string.Equals(
                    fileExt,
                    ".svgz",
                    StringComparison.OrdinalIgnoreCase))
            {
                using var fileStream = File.OpenRead(fileUri.Uri.LocalPath);
                using var zipStream = new GZipStream(
                    fileStream,
                    CompressionMode.Decompress);
                Load(zipStream);
                return;
            }
        }

        var doc = new XmlDocument();
        doc.Load(filePath);
        Load(doc);
    }

    public void Load(Uri fileUri)
    {
        ArgumentNullException.ThrowIfNull(fileUri);

        if (!fileUri.IsAbsoluteUri)
        {
            return;
        }

        var filePath = fileUri.IsFile
            ? fileUri.LocalPath
            : fileUri.AbsoluteUri;

        if (string.IsNullOrWhiteSpace(filePath))
        {
            Filename = string.Empty;
            return;
        }

        if (fileUri.IsFile && !File.Exists(filePath))
        {
            Filename = string.Empty;
            return;
        }

        Filename = filePath;
        var doc = new XmlDocument();
        doc.Load(filePath);
        Load(doc);
    }

    public void Load(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var doc = new XmlDocument();
        doc.Load(stream);
        Load(doc);
    }

    public void Load(XmlReader xmlReader)
    {
        ArgumentNullException.ThrowIfNull(xmlReader);

        var doc = new XmlDocument();
        doc.Load(xmlReader);
        Load(doc);
    }

    public void Load(TextReader txtReader)
    {
        ArgumentNullException.ThrowIfNull(txtReader);

        var doc = new XmlDocument();
        doc.Load(txtReader);
        Load(doc);
    }

    private void Load(XmlDocument svgDocument)
    {
        ArgumentNullException.ThrowIfNull(svgDocument);

        LoadStyles(svgDocument);
        var svgTags = svgDocument.GetElementsByTagName("svg");
        if (svgTags.Count == 0)
        {
            return;
        }

        var svgTag = svgTags[0];
        if (svgTag is not null)
        {
            elements = Parse(svgTag);
        }
    }

    private void Load(XmlNode svgTag)
    {
        ArgumentNullException.ThrowIfNull(svgTag);

        LoadStyles(svgTag);
        elements = Parse(svgTag);
    }

    [SuppressMessage("Performance", "MA0078:Use 'Cast' instead of 'Select' to cast", Justification = "OK.")]
    private void LoadStyles(XmlNode doc)
    {
        if (ExternalFileLoader is null)
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
            var url = Regex.Match(
                node.Data,
                "href=\"(?<url>.*?)\"",
                RegexOptions.None,
                TimeSpan.FromSeconds(5)).Groups["url"].Value;
            var stream = ExternalFileLoader.LoadFile(
                url,
                Filename);
            if (stream is null)
            {
                continue;
            }

            using var streamTmp = stream;
            using var reader = new StreamReader(
                stream,
                Encoding.UTF8);
            var style = reader.ReadToEnd();
            CssStyleParser.ParseStyle(
                this,
                style);
        }
    }

    private List<Shape> Parse(XmlNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        var vb = node.Attributes?.GetNamedItem("viewBox");
        if (vb?.Value is not null)
        {
            var cord = vb.Value.Split(' ');
            var cult = CultureInfo.InvariantCulture;
            ViewBox = new Rect(
                x: double.Parse(cord[0], cult),
                y: double.Parse(cord[1], cult),
                width: double.Parse(cord[2], cult),
                height: double.Parse(cord[3], cult));
        }

        Size = new Size(
            width: SvgXmlUtil.AttrValue(node, "width", 300),
            height: SvgXmlUtil.AttrValue(node, "height", 150));

        var fill = node.Attributes?.GetNamedItem("fill");
        if (fill?.Value is not null)
        {
            Fill = SolidColorBrushHelper.GetBrushFromString(
                fill.Value,
                GlobalizationConstants.EnglishCultureInfo);
        }

        var stroke = node.Attributes?.GetNamedItem("stroke");
        if (stroke?.Value is not null)
        {
            Stroke = SolidColorBrushHelper.GetBrushFromString(
                stroke.Value,
                GlobalizationConstants.EnglishCultureInfo);
        }

        var lstElements = new List<Shape>();
        if (node.Name != SvgTagConstants.Svg && node.Name != SvgTagConstants.Pattern)
        {
            throw new FormatException("Not a valid SVG node");
        }

        foreach (XmlNode childNode in node.ChildNodes)
        {
            W3cSvg.Shapes.Group.AddToList(
                this,
                lstElements,
                childNode,
                parent: null);
        }

        return lstElements;
    }
}