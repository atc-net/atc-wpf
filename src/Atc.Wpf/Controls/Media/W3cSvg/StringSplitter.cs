namespace Atc.Wpf.Controls.Media.W3cSvg;

internal sealed class StringSplitter
{
    private string val;
    private int curPos;

    public StringSplitter(string value)
    {
        val = value;
        curPos = 0;
    }

    public void SetString(string value, int startPos)
    {
        val = value;
        curPos = startPos;
    }

    public bool More => curPos >= 0 && curPos < val.Length;

    public double ReadNextValue()
    {
        var startPos = curPos;
        if (startPos < 0)
        {
            startPos = 0;
        }

        if (startPos >= val.Length)
        {
            return float.NaN;
        }

        var numbers = "0123456789-.eE";
        while (startPos < val.Length && !numbers.Contains(val[startPos], StringComparison.Ordinal))
        {
            startPos++;
        }

        var endPos = startPos;
        while (endPos < val.Length && numbers.Contains(val[endPos], StringComparison.Ordinal))
        {
            // '-' if number is followed by '-' then it indicates the end of the value
            if (endPos != startPos &&
                val[endPos] == '-' &&
                val[endPos - 1] != 'e' &&
                val[endPos - 1] != 'E')
            {
                break;
            }

            endPos++;
        }

        var len = endPos - startPos;
        if (len <= 0)
        {
            return float.NaN;
        }

        curPos = endPos;
        var s = val.Substring(startPos, len);

        startPos = endPos;
        while (startPos < val.Length && !numbers.Contains(val[startPos], StringComparison.Ordinal))
        {
            startPos++;
        }

        if (startPos >= val.Length)
        {
            endPos = startPos;
        }

        curPos = endPos;
        return XmlConvert.ToDouble(s);
    }

    public Point ReadNextPoint()
    {
        var x = ReadNextValue();
        var y = ReadNextValue();
        return new Point(x, y);
    }
}