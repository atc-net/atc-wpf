namespace Atc.Wpf.Controls.Media.W3cSvg;

internal static class CssStyleParser
{
    [SuppressMessage("Security", "MA0009:Add regex evaluation timeout", Justification = "OK.")]
    [SuppressMessage("Performance", "MA0023:Add RegexOptions.ExplicitCapture", Justification = "OK.")]
    private static readonly Regex RxStyle = new("([\\.,<>a-zA-Z0-9: \\-#]*){([^}]*)}", RegexOptions.Compiled | RegexOptions.Singleline);

    public static void ParseStyle(Svg svg, string style)
    {
        ArgumentNullException.ThrowIfNull(svg);

        var svgStyles = svg.Styles;

        var match = RxStyle.Match(style);
        while (match.Success)
        {
            var name = match.Groups[1].Value.Trim();
            var value = match.Groups[2].Value.Trim();
            foreach (var nm in name.Split(','))
            {
                if (!svgStyles.ContainsKey(nm))
                {
                    svgStyles.Add(nm, new List<KeyValueItem>());
                }

                foreach (var item in SvgXmlUtil.SplitStyle(value))
                {
                    svgStyles[nm].Add(new KeyValueItem(item.Key, item.Value));
                }
            }

            match = match.NextMatch();
        }
    }
}