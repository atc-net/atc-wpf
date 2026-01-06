namespace Atc.Wpf.Controls.Media.W3cSvg;

internal static class ShapeUtil
{
    public static Transform? ParseTransform(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var transforms = value.Split(')');
        if (transforms.Length == 2)
        {
            return ParseTransformInternal(value);
        }

        var tg = new TransformGroup();
        foreach (var transform in transforms
            .OrderBy(x => x.StartsWith(SvgTagConstants.Translate, StringComparison.Ordinal)))
        {
            if (string.IsNullOrEmpty(transform))
            {
                continue;
            }

            var transObj = ParseTransformInternal(transform + ")");
            if (transObj is not null)
            {
                tg.Children.Add(transObj);
            }
        }

        return tg;
    }

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
    private static Transform? ParseTransformInternal(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        value = value.Trim();
        var type = ExtractUntil(
                value,
                '(')
            .TrimStart(',');
        var v1 = ExtractBetween(
            value,
            '(',
            ')');

        var split = new StringSplitter(v1);
        var values = new List<double>();
        while (split.More)
        {
            values.Add(split.ReadNextValue());
        }

        if (type == SvgTagConstants.Translate)
        {
            return values.Count == 1
                ? new TranslateTransform(
                    values[0],
                    0)
                : new TranslateTransform(
                    values[0],
                    values[1]);
        }

        if (type == SvgTagConstants.Matrix)
        {
            return Transform.Parse(v1);
        }

        if (type == SvgTagConstants.Scale)
        {
            return values.Count == 1
                ? new ScaleTransform(
                    values[0],
                    values[0])
                : new ScaleTransform(
                    values[0],
                    values[1]);
        }

        if (type == SvgTagConstants.SkewX)
        {
            return new SkewTransform(
                values[0],
                0);
        }

        if (type == SvgTagConstants.SkewY)
        {
            return new SkewTransform(
                0,
                values[0]);
        }

        if (type == SvgTagConstants.Rotate)
        {
            return values.Count switch
            {
                1 => new RotateTransform(
                    values[0],
                    0,
                    0),
                2 => new RotateTransform(
                    values[0],
                    values[1],
                    0),
                _ => new RotateTransform(
                    values[0],
                    values[1],
                    values[2]),
            };
        }

        return null;
    }

    public static string ExtractUntil(
        string value,
        char ch)
    {
        ArgumentNullException.ThrowIfNull(value);

        var index = value.IndexOf(
            ch,
            StringComparison.Ordinal);
        return index <= 0
            ? value
            : value.Substring(
                0,
                index);
    }

    public static string ExtractBetween(
        string value,
        char startCh,
        char endCh)
    {
        ArgumentNullException.ThrowIfNull(value);

        var start = value.IndexOf(
            startCh,
            StringComparison.Ordinal);
        if (startCh < 0)
        {
            return value;
        }

        start++;
        var end = value.IndexOf(
            endCh,
            start);
        return endCh < 0
            ? value.Substring(start)
            : value.Substring(
                start,
                end - start);
    }

    public static double CalculateDRef(
        double width,
        double height)
        => System.Math.Sqrt((width * width) + (height * height)) /
            System.Math.Sqrt(2);

    [SuppressMessage("Usage", "CA2201:Do not raise reserved exception types", Justification = "OK.")]
    [SuppressMessage("Major Code Smell", "S112:General exceptions should never be thrown", Justification = "OK.")]
    public static KeyValueItem ReadNextAttr(
        string input,
        ref int startPos)
    {
        ArgumentNullException.ThrowIfNull(input);

        if (input[startPos] != ' ')
        {
            throw new Exception("input[startPos] must be a whitespace character");
        }

        while (input[startPos] == ' ')
        {
            startPos++;
        }

        var nameStart = startPos;
        var nameEnd = input.IndexOf(
            '=',
            startPos);
        if (nameEnd < nameStart)
        {
            throw new Exception("did not find xml attribute name");
        }

        var valueStart = input.IndexOf(
            '"',
            nameEnd);
        valueStart++;
        var valueEnd = input.IndexOf(
            '"',
            valueStart);
        if (valueEnd < 0 || valueEnd < valueStart)
        {
            throw new Exception("did not find xml attribute value");
        }

        var attrName = input
            .Substring(
                nameStart,
                nameEnd - nameStart)
            .Trim();
        var attrValue = input
            .Substring(
                valueStart,
                valueEnd - valueStart)
            .Trim();
        startPos = valueEnd + 1;
        return new KeyValueItem(
            attrName,
            attrValue);
    }
}