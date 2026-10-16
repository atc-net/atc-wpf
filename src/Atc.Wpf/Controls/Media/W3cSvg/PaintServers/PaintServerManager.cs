// ReSharper disable MemberCanBeMadeStatic.Global
namespace Atc.Wpf.Controls.Media.W3cSvg.PaintServers;

internal sealed class PaintServerManager
{
    private readonly Dictionary<string, PaintServer> paintServers = new(StringComparer.Ordinal);

    public void Create(
        Svg svg,
        XmlNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        switch (node.Name)
        {
            case SvgTagConstants.LinearGradient:
            {
                var id = SvgXmlUtil.AttrValue(
                    node,
                    "id");
                if (id is not null && !paintServers.ContainsKey(id))
                {
                    paintServers[id] = new LinearGradientColorPaintServer(
                        this,
                        node);
                }

                return;
            }

            case SvgTagConstants.RadialGradient:
            {
                var id = SvgXmlUtil.AttrValue(
                    node,
                    "id");
                if (id is not null && !paintServers.ContainsKey(id))
                {
                    paintServers[id] = new RadialGradientColorPaintServer(
                        this,
                        node);
                }

                return;
            }

            case SvgTagConstants.Pattern:
            {
                var id = SvgXmlUtil.AttrValue(
                    node,
                    "id");
                if (id is not null && !paintServers.ContainsKey(id))
                {
                    paintServers[id] = new PatternPaintServer(
                        this,
                        svg,
                        node);
                }

                return;
            }

            default:
                return;
        }
    }

    public void AddServer(
        string key,
        PaintServer server)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return;
        }

        paintServers[key] = server;
    }

    public PaintServer? GetServer(string serverKey)
    {
        if (string.IsNullOrWhiteSpace(serverKey))
        {
            return null;
        }

        return paintServers.TryGetValue(
                serverKey,
                out var paintServer)
            ? paintServer
            : null;
    }

    public Dictionary<string, PaintServer> GetServers()
        => paintServers;

    public void CreateServerFromBrush(
        string key,
        Brush customBrush)
    {
        paintServers[key] = customBrush switch
        {
            SolidColorBrush => new SolidColorPaintServer(
                this,
                customBrush),
            LinearGradientBrush => new LinearGradientColorPaintServer(
                this,
                customBrush),
            RadialGradientBrush => new RadialGradientColorPaintServer(
                this,
                customBrush),
            DrawingBrush => new PatternPaintServer(
                this,
                customBrush),
            _ => new PaintServer(
                this,
                customBrush),
        };
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
    public string? Parse(string value)
    {
        try
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            if (value == SvgTagConstants.None)
            {
                return null;
            }

            if (paintServers.TryGetValue(
                    value,
                    out _))
            {
                return value;
            }

            switch (value)
            {
                case SvgTagConstants.Inherit:
                    paintServers[value] = new InheritPaintServer(this);
                    return value;
                case SvgTagConstants.CurrentColor:
                    paintServers[value] = new CurrentColorPaintServer(this);
                    return value;
            }

            if (value[0] == '#')
            {
                return ParseSolidColor(value);
            }

            if (value.StartsWith("url", StringComparison.Ordinal))
            {
                var id = ShapeUtil.ExtractBetween(
                    value,
                    '(',
                    ')');
                if (id.Length > 0 && id[0] == '#')
                {
                    id = id.Substring(1);
                }

                paintServers.TryGetValue(
                    id,
                    out _);
                return id;
            }

            return value.StartsWith("rgb", StringComparison.Ordinal)
                ? ParseSolidRgbColor(value)
                : ParseKnownColor(value.ToLower(GlobalizationConstants.EnglishCultureInfo));
        }
        catch
        {
            // Dummy
        }

        return null;
    }

    public static Color KnownColor(string value)
    {
        var color = ColorHelper.GetColorFromString(
            value,
            GlobalizationConstants.EnglishCultureInfo);
        return color ?? Colors.Black;
    }

    private string ParseSolidColor(string value)
    {
        var id = value;
        if (paintServers.TryGetValue(
                id,
                out _))
        {
            return id;
        }

        var color = ColorHelper.GetColorFromHex(value);
        if (color is null)
        {
            return id;
        }

        var paintServer = new SolidColorPaintServer(
            this,
            color.Value);
        paintServers[id] = paintServer;
        return id;
    }

    private string? ParseSolidRgbColor(string value)
    {
        value = string.Concat(value.Where(c => !char.IsWhiteSpace(c)));
        if (paintServers.ContainsKey(value))
        {
            return value;
        }

        if (!value.EndsWith(')'))
        {
            return null;
        }

        string[] components;
        bool hasAlpha;
        if (value.StartsWith("rgb(", StringComparison.Ordinal))
        {
            components = value[4..^1].Split(',');
            hasAlpha = false;
        }
        else if (value.StartsWith("rgba(", StringComparison.Ordinal))
        {
            components = value[5..^1].Split(',');
            hasAlpha = true;
        }
        else
        {
            return null;
        }

        if (components.Length != (hasAlpha ? 4 : 3))
        {
            return null;
        }

        var r = ParseColorComponent(components[0]);
        var g = ParseColorComponent(components[1]);
        var b = ParseColorComponent(components[2]);
        var color = hasAlpha
            ? Color.FromArgb(ParseAlphaComponent(components[3]), r, g, b)
            : Color.FromRgb(r, g, b);

        paintServers[value] = new SolidColorPaintServer(
            this,
            color);
        return value;
    }

    private static byte ParseColorComponent(string value)
        => (byte)System.Math.Clamp(ParseColorNumber(value), 0, 255);

    /// <summary>The alpha of rgba() is 0..1 (or a percentage), not 0..255.</summary>
    private static byte ParseAlphaComponent(string value)
    {
        var alpha = value.EndsWith('%')
            ? double.Parse(value.AsSpan(0, value.Length - 1), CultureInfo.InvariantCulture) / 100
            : double.Parse(value, CultureInfo.InvariantCulture);

        return (byte)System.Math.Round(System.Math.Clamp(alpha, 0, 1) * 255);
    }

    private static int ParseColorNumber(string value)
    {
        if (!value.EndsWith('%'))
        {
            return NumberHelper.ParseToInt(value);
        }

        var nr = int.Parse(
            value.AsSpan(
                0,
                value.Length - 1),
            GlobalizationConstants.EnglishCultureInfo);
        if (nr < 0)
        {
            nr = 255 - nr;
        }

        return nr * 255 / 100;
    }

    private string? ParseKnownColor(string value)
    {
        if (paintServers.TryGetValue(
                value,
                out _))
        {
            return value;
        }

        var color = ColorHelper.GetColorFromString(
            value,
            GlobalizationConstants.EnglishCultureInfo);
        if (!color.HasValue)
        {
            return null;
        }

        var paintServer = new SolidColorPaintServer(
            this,
            color.Value);
        paintServers[value] = paintServer;
        return value;
    }
}