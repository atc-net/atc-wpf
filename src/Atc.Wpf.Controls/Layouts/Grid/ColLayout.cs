namespace Atc.Wpf.Controls.Layouts.Grid;

[TypeConverter(typeof(ColLayoutConverter))]
public class ColLayout : MarkupExtension
{
    public static readonly int ColMaxCellCount = 24;
    public static readonly int HalfColMaxCellCount = 12;
    public static readonly int XsMaxWidth = 768;
    public static readonly int SmMaxWidth = 992;
    public static readonly int MdMaxWidth = 1200;
    public static readonly int LgMaxWidth = 1920;
    public static readonly int XlMaxWidth = 2560;

    public ColLayout()
    {
    }

    public ColLayout(
        int uniformWidth)
    {
        Xs = uniformWidth;
        Sm = uniformWidth;
        Md = uniformWidth;
        Lg = uniformWidth;
        Xl = uniformWidth;
        Xxl = uniformWidth;
    }

    public ColLayout(
        int xs,
        int sm,
        int md,
        int lg,
        int xl,
        int xxl)
    {
        Xs = xs;
        Sm = sm;
        Md = md;
        Lg = lg;
        Xl = xl;
        Xxl = xxl;
    }

    public int Xs { get; set; } = 24;

    public int Sm { get; set; } = 12;

    public int Md { get; set; } = 8;

    public int Lg { get; set; } = 6;

    public int Xl { get; set; } = 4;

    public int Xxl { get; set; } = 2;

    public override object ProvideValue(
        IServiceProvider serviceProvider)
    {
        return new ColLayout
        {
            Xs = Xs,
            Sm = Sm,
            Md = Md,
            Lg = Lg,
            Xl = Xl,
            Xxl = Xxl,
        };
    }

    public static ColLayoutType GetLayoutStatus(
        double width)
    {
        if (width < MdMaxWidth)
        {
            if (width < SmMaxWidth)
            {
                return width < XsMaxWidth
                    ? ColLayoutType.Xs
                    : ColLayoutType.Sm;
            }

            return ColLayoutType.Md;
        }

        if (width < XlMaxWidth)
        {
            return width < LgMaxWidth
                ? ColLayoutType.Lg
                : ColLayoutType.Xl;
        }

        return ColLayoutType.Xxl;
    }

    public override string ToString()
    {
        var cultureInfo = CultureInfo.CurrentCulture;
        var listSeparator = TokenizerHelper.GetNumericListSeparator(cultureInfo);

        var sb = new StringBuilder();
        sb.Append(Xs.ToString(cultureInfo));
        sb.Append(listSeparator);
        sb.Append(Sm.ToString(cultureInfo));
        sb.Append(listSeparator);
        sb.Append(Md.ToString(cultureInfo));
        sb.Append(listSeparator);
        sb.Append(Lg.ToString(cultureInfo));
        sb.Append(listSeparator);
        sb.Append(Xl.ToString(cultureInfo));
        sb.Append(listSeparator);
        sb.Append(Xxl.ToString(cultureInfo));
        return sb.ToString();
    }
}