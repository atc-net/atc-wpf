using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using Atc.Data.Models;
using Atc.Wpf.Controls.W3cSvg.Shapes.Filters;

// ReSharper disable VirtualMemberCallInConstructor
// ReSharper disable VirtualMemberNeverOverridden.Global
#pragma warning disable 8618
namespace Atc.Wpf.Controls.W3cSvg.Shapes
{
    [SuppressMessage("Critical Code Smell", "S1699:Constructors should only call non-overridable methods", Justification = "OK.")]
    [SuppressMessage("Usage", "CA2214:Do not call overridable methods in constructors", Justification = "OK.")]
    [SuppressMessage("Design", "MA0056:Do not call overridable members in constructor", Justification = "OK.")]
    internal class Shape
    {
        private Fill? fill;
        private Stroke? stroke;
        private TextStyle? textStyle;
        private string? localStyle;
        private double opacity;

        public Shape(Svg svg, XmlNode? node = null, Shape? parent = null)
        {
            this.Id = node == null
                ? "<null>"
                : SvgXmlUtil.AttrValue(node, "id");

            this.Opacity = 1;
            this.Parent = parent;
            this.ParseAtStart(svg, node);
            if (node?.Attributes != null)
            {
                foreach (XmlAttribute? attr in node.Attributes)
                {
                    if (attr is not null)
                    {
                        this.Parse(svg, attr.Name, attr.Value);
                    }
                }
            }

            this.ParseLocalStyle(svg);
        }

        public Shape(Svg svg, IReadOnlyCollection<KeyValueItem>? attrs, Shape parent)
        {
            this.Id = "<null>";

            this.Opacity = 1;
            this.Parent = parent;
            if (attrs != null)
            {
                foreach (KeyValueItem attr in attrs)
                {
                    this.Parse(svg, attr.Key, attr.Value);
                }
            }

            this.ParseLocalStyle(svg);
        }

        internal Clip? Clip { get; set; }

        internal Geometry? GeometryElement { get; set; }

        internal virtual Filter? Filter { get; private set; }

        internal Shape? RealParent { get; set; }

        public string Id { get; }

        public string? PaintServerKey { get; set; }

        public string? RequiredExtensions { get; set; }

        public string? RequiredFeatures { get; set; }

        public VisibilityType Visibility { get; set; }

        public bool Display { get; set; } = true;

        public virtual Stroke? Stroke
        {
            get
            {
                if (this.stroke != null)
                {
                    return this.stroke;
                }

                var parent = this.Parent;
                while (parent != null)
                {
                    if (parent.Stroke != null)
                    {
                        return parent.Stroke;
                    }

                    parent = parent.Parent;
                }

                return null;
            }
        }

        public virtual Fill? Fill
        {
            get
            {
                if (this.fill != null)
                {
                    return this.fill;
                }

                var parent = this.Parent;
                while (parent != null)
                {
                    if (parent.Fill != null)
                    {
                        return parent.Fill;
                    }

                    parent = parent.Parent;
                }

                return null;
            }
        }

        public virtual TextStyle? TextStyle
        {
            get
            {
                if (this.textStyle != null)
                {
                    return this.textStyle;
                }

                var parent = this.Parent;
                while (parent != null)
                {
                    if (parent.textStyle != null)
                    {
                        return parent.textStyle;
                    }

                    parent = parent.Parent;
                }

                return null;
            }
        }

        public double Opacity
        {
            get => Visibility == VisibilityType.Visible ? opacity : 0;
            set => opacity = value;
        }

        public virtual Transform? Transform { get; private set; }

        public Shape? Parent { get; set; }

        protected virtual void ParseAtStart(Svg svg, XmlNode? node)
        {
            if (svg == null)
            {
                throw new ArgumentNullException(nameof(svg));
            }

            if (node == null)
            {
                return;
            }

            var name = node.Name;
            if (name.Contains(':', StringComparison.Ordinal))
            {
                name = name.Split(':')[1];
            }

            if (svg.StyleItems.TryGetValue(name, out var attributes))
            {
                foreach (var xmlAttribute in attributes)
                {
                    Parse(svg, xmlAttribute.Key, xmlAttribute.Value);
                }
            }

            if (!string.IsNullOrEmpty(this.Id) &&
                svg.StyleItems.TryGetValue("#" + this.Id, out attributes))
            {
                foreach (var xmlAttribute in attributes)
                {
                    Parse(svg, xmlAttribute.Key, xmlAttribute.Value);
                }
            }
        }

        protected virtual void ParseLocalStyle(Svg svg)
        {
            if (string.IsNullOrEmpty(this.localStyle))
            {
                return;
            }

            foreach (KeyValueItem item in SvgXmlUtil.SplitStyle(this.localStyle))
            {
                this.Parse(svg, item.Key, item.Value);
            }
        }

        [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK - for now.")]
        protected virtual void Parse(Svg svg, string name, string value)
        {
            if (svg == null)
            {
                throw new ArgumentNullException(nameof(svg));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (name.Contains(':', StringComparison.Ordinal))
            {
                name = name.Split(':')[1];
            }

            switch (name)
            {
                case SvgTagConstants.Display when value == "none":
                    this.Display = false;
                    break;
                case SvgTagConstants.Class:
                {
                    var classes = value.Split(' ');
                    foreach (var @class in classes)
                    {
                        if (!svg.StyleItems.TryGetValue("." + @class, out var attributes))
                        {
                            continue;
                        }

                        foreach (var xmlAttribute in attributes)
                        {
                            Parse(svg, xmlAttribute.Key, xmlAttribute.Value);
                        }
                    }

                    return;
                }
            }

            switch (name)
            {
                case SvgTagConstants.Transform:
                    this.Transform = ShapeUtil.ParseTransform(value.ToLower(GlobalizationConstants.EnglishCultureInfo));
                    return;
                case SvgTagConstants.Visibility:
                    this.Visibility = value == "hidden"
                        ? VisibilityType.Hidden
                        : VisibilityType.Visible;
                    break;
                case SvgTagConstants.Stroke:
                    this.GetStroke().PaintServerKey = svg.PaintServers.Parse(value);
                    return;
                case SvgTagConstants.StrokeWidth:
                    this.GetStroke().Width = SvgXmlUtil.ParseDouble(value);
                    return;
                case SvgTagConstants.StrokeOpacity:
                    this.GetStroke().Opacity = SvgXmlUtil.ParseDouble(value) * 100;
                    return;
                case SvgTagConstants.StrokeDashArray when value == "none":
                    this.GetStroke().StrokeArray = null;
                    return;
                case SvgTagConstants.StrokeDashArray:
                {
                    var stringSplitter = new StringSplitter(value);
                    var doubles = new List<double>();
                    while (stringSplitter.More)
                    {
                        doubles.Add(stringSplitter.ReadNextValue());
                    }

                    this.GetStroke().StrokeArray = doubles.ToArray();
                    return;
                }

                case SvgTagConstants.RequiredFeatures:
                    this.RequiredFeatures = value.Trim();
                    return;
                case SvgTagConstants.RequiredExtensions:
                    this.RequiredExtensions = value.Trim();
                    return;
                case SvgTagConstants.StrokeLineCap:
                    this.GetStroke().LineCap = Enum<StrokeLineCapType>.Parse(value);
                    return;
                case SvgTagConstants.StrokeLineJoin:
                    this.GetStroke().LineJoin = Enum<StrokeLineJoinType>.Parse(value);
                    return;
                case SvgTagConstants.FilterProperty when value.StartsWith("url", StringComparison.Ordinal):
                {
                    string id = ShapeUtil.ExtractBetween(value, '(', ')');
                    if (id.Length > 0 && id[0] == '#')
                    {
                        id = id.Substring(1);
                    }

                    svg.Shapes.TryGetValue(id, out var shape);
                    this.Filter = shape as Filter;
                    return;
                }

                case SvgTagConstants.FilterProperty:
                    return;
                case SvgTagConstants.ClipPathProperty when value.StartsWith("url", StringComparison.Ordinal):
                {
                    string id = ShapeUtil.ExtractBetween(value, '(', ')');
                    if (id.Length > 0 && id[0] == '#')
                    {
                        id = id.Substring(1);
                    }

                    svg.Shapes.TryGetValue(id, out var shape);
                    this.Clip = shape as Clip;
                    return;
                }

                case SvgTagConstants.ClipPathProperty:
                    return;
                case SvgTagConstants.Fill:
                    this.GetFill().PaintServerKey = svg.PaintServers.Parse(value);
                    return;
                case SvgTagConstants.Color:
                    this.PaintServerKey = svg.PaintServers.Parse(value);
                    return;
                case SvgTagConstants.FillOpacity:
                    this.GetFill().Opacity = SvgXmlUtil.ParseDouble(value) * 100;
                    return;
                case SvgTagConstants.FillRule:
                    this.GetFill().FillRule = Enum<FillRuleType>.Parse(value);
                    return;
                case SvgTagConstants.Opacity:
                    this.Opacity = SvgXmlUtil.ParseDouble(value);
                    return;
                case SvgTagConstants.Style:
                    localStyle = value;
                    break;
                case SvgTagConstants.FontFamily:
                    this.GetTextStyle().FontFamily = value;
                    return;
                case SvgTagConstants.FontSize:
                    this.GetTextStyle().FontSize = SvgXmlUtil.AttrValue(new KeyValueItem(name, value));
                    return;
                case SvgTagConstants.FontWeight:
                    this.GetTextStyle().FontWeight = (FontWeight)new FontWeightConverter().ConvertFromString(value);
                    return;
                case SvgTagConstants.FontStyle:
                    this.GetTextStyle().FontStyle = (FontStyle)new FontStyleConverter().ConvertFromString(value);
                    return;
                case SvgTagConstants.TextDecoration:
                {
                    TextDecoration t = new TextDecoration();
                    switch (value)
                    {
                        case "none":
                            return;
                        case "underline":
                            t.Location = TextDecorationLocation.Underline;
                            break;
                        case "overline":
                            t.Location = TextDecorationLocation.OverLine;
                            break;
                        case "line-through":
                            t.Location = TextDecorationLocation.Strikethrough;
                            break;
                    }

                    var tt = new TextDecorationCollection
                    {
                        t,
                    };

                    this.GetTextStyle().TextDecoration = tt;
                    return;
                }

                case SvgTagConstants.TextAnchor:
                {
                    this.GetTextStyle().TextAlignment = value switch
                    {
                        "start" => TextAlignment.Left,
                        "middle" => TextAlignment.Center,
                        "end" => TextAlignment.Right,
                        _ => this.GetTextStyle().TextAlignment
                    };

                    return;
                }

                case "word-spacing":
                    this.GetTextStyle().WordSpacing = SvgXmlUtil.AttrValue(new KeyValueItem(name, value));
                    return;
                case "letter-spacing":
                    this.GetTextStyle().LetterSpacing = SvgXmlUtil.AttrValue(new KeyValueItem(name, value));
                    return;
                case "baseline-shift":
                    this.GetTextStyle().BaseLineShift = value;
                    return;
            }
        }

        protected Fill GetFill() => this.fill ??= new Fill();

        protected TextStyle GetTextStyle() => this.textStyle ??= new TextStyle(this);

        public override string ToString()
        {
            return this.GetType().Name + " (" + Id + ")";
        }

        private Stroke GetStroke() => this.stroke ??= new Stroke();
    }
}
#pragma warning restore 8618