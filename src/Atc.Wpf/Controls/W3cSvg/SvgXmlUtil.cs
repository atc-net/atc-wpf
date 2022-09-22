namespace Atc.Wpf.Controls.W3cSvg;

internal static class SvgXmlUtil
{
    public static bool GetValueRespectingUnits(string input, out double value, double percentageMaximum)
    {
        ArgumentNullException.ThrowIfNull(input);

        value = 0;
        var units = string.Empty;
        var index = input.LastIndexOfAny(new[] { '.', '-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
        if (index < 0)
        {
            return false;
        }

        var tmp = input.Substring(0, index + 1);
        if (index + 1 < input.Length)
        {
            units = input.Substring(index + 1);
        }

        try
        {
            value = XmlConvert.ToDouble(tmp);

            switch (units)
            {
                case "pt":
                    value *= 1.25;
                    break;
                case "mm":
                    value *= 3.54;
                    break;
                case "pc":
                    value *= 15;
                    break;
                case "cm":
                    value *= 35.43;
                    break;
                case "in":
                    value *= 90;
                    break;
                case "%":
                    value = value * percentageMaximum / 100;
                    break;
            }

            return true;
        }
        catch (FormatException)
        {
            // Dummy
        }

        return false;
    }

    public static double GetDoubleValue(string value, double percentageMaximum = 1)
    {
        ArgumentNullException.ThrowIfNull(value);

        return GetValueRespectingUnits(value, out var result, percentageMaximum)
            ? result
            : 0;
    }

    public static double AttrValue(KeyValueItem attr)
    {
        ArgumentNullException.ThrowIfNull(attr);

        return GetValueRespectingUnits(attr.Value, out var result, 1)
            ? result
            : 0;
    }

    public static double AttrValue(XmlNode node, string id, double defaultValue, double percentageMaximum = 1)
    {
        ArgumentNullException.ThrowIfNull(node);

        var attr = node.Attributes?[id];
        if (attr is null)
        {
            return defaultValue;
        }

        return GetValueRespectingUnits(attr.Value, out var result, percentageMaximum)
            ? result
            : defaultValue;
    }

    public static string? AttrValue(XmlNode node, string id, string? defaultValue)
    {
        ArgumentNullException.ThrowIfNull(node);

        if (node.Attributes is null)
        {
            return defaultValue;
        }

        var attr = node.Attributes[id];
        return attr is not null
            ? attr.Value
            : defaultValue;
    }

    public static string? AttrValue(XmlNode node, string id)
    {
        return AttrValue(node, id, string.Empty);
    }

    public static double ParseDouble(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return GetValueRespectingUnits(value, out var result, 1)
            ? result
            : 0.1;
    }

    public static IEnumerable<KeyValueItem> SplitStyle(string fullStyle)
    {
        ArgumentNullException.ThrowIfNull(fullStyle);

        var list = new List<KeyValueItem>();
        if (fullStyle.Length == 0)
        {
            return list;
        }

        var attrs = fullStyle.Split(';');
        list.AddRange(
            from attr
                in attrs
            select attr.Split(':')
            into s
            where s.Length == 2
            select new KeyValueItem(s[0].Trim(), s[1].Trim()));

        return list;
    }
}