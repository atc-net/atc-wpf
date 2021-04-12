using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Media;
using System.Xml;
using Atc.Wpf.Media;

// ReSharper disable MemberCanBeMadeStatic.Global
namespace Atc.Wpf.Controls.W3cSvg.PaintServers
{
    internal class PaintServerManager
    {
        private readonly Dictionary<string, PaintServer> paintServers = new Dictionary<string, PaintServer>(StringComparer.Ordinal);

        public void Create(Svg svg, XmlNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            switch (node.Name)
            {
                case SvgTagConstants.LinearGradient:
                {
                    var id = SvgXmlUtil.AttrValue(node, "id");
                    if (id is not null && !this.paintServers.ContainsKey(id))
                    {
                        this.paintServers[id] = new LinearGradientColorPaintServer(this, node);
                    }

                    return;
                }

                case SvgTagConstants.RadialGradient:
                {
                    var id = SvgXmlUtil.AttrValue(node, "id");
                    if (id is not null && !this.paintServers.ContainsKey(id))
                    {
                        this.paintServers[id] = new RadialGradientColorPaintServer(this, node);
                    }

                    return;
                }

                case SvgTagConstants.Pattern:
                {
                    var id = SvgXmlUtil.AttrValue(node, "id");
                    if (id is not null && !this.paintServers.ContainsKey(id))
                    {
                        this.paintServers[id] = new PatternPaintServer(this, svg, node);
                    }

                    return;
                }

                default:
                    return;
            }
        }

        public void AddServer(string key, PaintServer server)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            this.paintServers[key] = server;
        }

        public PaintServer? GetServer(string serverKey)
        {
            if (string.IsNullOrWhiteSpace(serverKey))
            {
                return null;
            }

            return this.paintServers.TryGetValue(serverKey, out var paintServer)
                ? paintServer
                : null;
        }

        public Dictionary<string, PaintServer> GetServers()
        {
            return this.paintServers;
        }

        public void CreateServerFromBrush(string key, Brush customBrush)
        {
            this.paintServers[key] = customBrush switch
            {
                SolidColorBrush => new SolidColorPaintServer(this, customBrush),
                LinearGradientBrush => new LinearGradientColorPaintServer(this, customBrush),
                RadialGradientBrush => new RadialGradientColorPaintServer(this, customBrush),
                DrawingBrush => new PatternPaintServer(this, customBrush),
                _ => new PaintServer(this, customBrush)
            };
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
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

                if (this.paintServers.TryGetValue(value, out _))
                {
                    return value;
                }

                switch (value)
                {
                    case SvgTagConstants.Inherit:
                        this.paintServers[value] = new InheritPaintServer(this);
                        return value;
                    case SvgTagConstants.CurrentColor:
                        this.paintServers[value] = new CurrentColorPaintServer(this);
                        return value;
                }

                if (value[0] == '#')
                {
                    return ParseSolidColor(value);
                }

                if (value.StartsWith("url", StringComparison.Ordinal))
                {
                    string id = ShapeUtil.ExtractBetween(value, '(', ')');
                    if (id.Length > 0 && id[0] == '#')
                    {
                        id = id.Substring(1);
                    }

                    this.paintServers.TryGetValue(id, out _);
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
            return ColorUtil.KnownColors.ContainsKey(value)
                ? ColorUtil.KnownColors[value]
                : Colors.Black;
        }

        private string ParseSolidColor(string value)
        {
            var id = value;
            if (this.paintServers.TryGetValue(id, out _))
            {
                return id;
            }

            var paintServer = new SolidColorPaintServer(this, ColorUtil.GetColorFromHex(value));
            this.paintServers[id] = paintServer;
            return id;
        }

        private string? ParseSolidRgbColor(string value)
        {
            value = value
                .Replace("\r", string.Empty, StringComparison.Ordinal)
                .Replace("\n", string.Empty, StringComparison.Ordinal)
                .Replace("\t", string.Empty, StringComparison.Ordinal)
                .Replace(" ", string.Empty, StringComparison.Ordinal);
            if (value.StartsWith("rgb(", StringComparison.Ordinal))
            {
                var newVal = value.Substring(4, value.Length - 5).Split(',');
                return ParseSolidColor("#" + ParseColorNumber(newVal[0]).ToString("x", GlobalizationConstants.EnglishCultureInfo) + ParseColorNumber(newVal[1]).ToString("x", GlobalizationConstants.EnglishCultureInfo) + ParseColorNumber(newVal[2]).ToString("x", GlobalizationConstants.EnglishCultureInfo));
            }

            if (value.StartsWith("rgba(", StringComparison.Ordinal))
            {
                var newVal = value.Substring(5, value.Length - 6).Split(',');
                return ParseSolidColor("#" + ParseColorNumber(newVal[0]).ToString("x", GlobalizationConstants.EnglishCultureInfo) + ParseColorNumber(newVal[1]).ToString("x", GlobalizationConstants.EnglishCultureInfo) + ParseColorNumber(newVal[2]).ToString("x", GlobalizationConstants.EnglishCultureInfo) + ParseColorNumber(newVal[3]).ToString("x", GlobalizationConstants.EnglishCultureInfo));
            }

            return null;
        }

        private static int ParseColorNumber(string value)
        {
            if (!value.EndsWith('%'))
            {
                return int.Parse(value, GlobalizationConstants.EnglishCultureInfo);
            }

            var nr = int.Parse(value.Substring(0, value.Length - 1), GlobalizationConstants.EnglishCultureInfo);
            if (nr < 0)
            {
                nr = 255 - nr;
            }

            return nr * 255 / 100;
        }

        private string? ParseKnownColor(string value)
        {
            if (this.paintServers.TryGetValue(value, out _))
            {
                return value;
            }

            if (ColorUtil.KnownColors.TryGetValue(value, out var color))
            {
                var paintServer = new SolidColorPaintServer(this, color);
                this.paintServers[value] = paintServer;
                return value;
            }

            return null;
        }
    }
}