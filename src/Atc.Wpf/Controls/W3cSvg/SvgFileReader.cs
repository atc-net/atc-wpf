// ReSharper disable once GCSuppressFinalizeForTypeWithoutDestructor
namespace Atc.Wpf.Controls.W3cSvg;

internal sealed class SvgFileReader : IDisposable
{
    public const string GZipSignature = "H4sI"; // (the Base64 encoded version "1F 8B 08")

    public SvgFileReader(Color? overrideColor)
    {
        OverrideColor = overrideColor;
    }

    private Color? OverrideColor { get; }

    public DrawingGroup Read(Uri fileUri)
    {
        if (fileUri is null)
        {
            return new DrawingGroup();
        }

        var svgRender = new SvgRender(new FileSystemLoader())
        {
            OverrideColor = OverrideColor,
        };

        return svgRender.LoadDrawing(fileUri);
    }

    public DrawingGroup? Read(Stream? stream)
    {
        if (stream is null)
        {
            return new DrawingGroup();
        }

        var svgRender = new SvgRender(new FileSystemLoader())
        {
            OverrideColor = OverrideColor,
        };

        return svgRender.LoadDrawing(stream);
    }

    public static string RemoveWhitespace(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return string.Empty;
        }

        var len = str.Length;
        var src = str.ToCharArray();
        var dstIdx = 0;

        for (var i = 0; i < len; i++)
        {
            var ch = src[i];

            switch (ch)
            {
                case '\u0020':
                case '\u00A0':
                case '\u1680':
                case '\u2000':
                case '\u2001':
                case '\u2002':
                case '\u2003':
                case '\u2004':
                case '\u2005':
                case '\u2006':
                case '\u2007':
                case '\u2008':
                case '\u2009':
                case '\u200A':
                case '\u202F':
                case '\u205F':
                case '\u3000':
                case '\u2028':
                case '\u2029':
                case '\u0009':
                case '\u000A':
                case '\u000B':
                case '\u000C':
                case '\u000D':
                case '\u0085':
                    continue;
                default:
                    src[dstIdx++] = ch;
                    break;
            }
        }

        return new string(src, 0, dstIdx);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}