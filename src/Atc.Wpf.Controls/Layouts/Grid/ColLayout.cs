namespace Atc.Wpf.Controls.Layouts.Grid;

/// <summary>Describes the number of cells a <see cref="Col"/> spans at each responsive breakpoint.</summary>
[TypeConverter(typeof(ColLayoutConverter))]
public sealed class ColLayout : MarkupExtension
{
    /// <summary>The number of cells in a full row.</summary>
    public static readonly int ColMaxCellCount = 24;

    /// <summary>The number of cells in half a row.</summary>
    public static readonly int HalfColMaxCellCount = 12;

    /// <summary>The exclusive upper width bound of the <see cref="ColLayoutType.Xs"/> breakpoint.</summary>
    public static readonly int XsMaxWidth = 768;

    /// <summary>The exclusive upper width bound of the <see cref="ColLayoutType.Sm"/> breakpoint.</summary>
    public static readonly int SmMaxWidth = 992;

    /// <summary>The exclusive upper width bound of the <see cref="ColLayoutType.Md"/> breakpoint.</summary>
    public static readonly int MdMaxWidth = 1200;

    /// <summary>The exclusive upper width bound of the <see cref="ColLayoutType.Lg"/> breakpoint.</summary>
    public static readonly int LgMaxWidth = 1920;

    /// <summary>The exclusive upper width bound of the <see cref="ColLayoutType.Xl"/> breakpoint.</summary>
    public static readonly int XlMaxWidth = 2560;

    /// <summary>Initializes a new instance of the <see cref="ColLayout"/> class with the default spans.</summary>
    public ColLayout()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ColLayout"/> class that uses the same span at every breakpoint.</summary>
    public ColLayout(int uniformWidth)
    {
        Xs = uniformWidth;
        Sm = uniformWidth;
        Md = uniformWidth;
        Lg = uniformWidth;
        Xl = uniformWidth;
        Xxl = uniformWidth;
    }

    /// <summary>Initializes a new instance of the <see cref="ColLayout"/> class with a span for each breakpoint.</summary>
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

    /// <summary>Gets or sets the number of cells spanned at the <see cref="ColLayoutType.Xs"/> breakpoint.</summary>
    public int Xs { get; set; } = 24;

    /// <summary>Gets or sets the number of cells spanned at the <see cref="ColLayoutType.Sm"/> breakpoint.</summary>
    public int Sm { get; set; } = 12;

    /// <summary>Gets or sets the number of cells spanned at the <see cref="ColLayoutType.Md"/> breakpoint.</summary>
    public int Md { get; set; } = 8;

    /// <summary>Gets or sets the number of cells spanned at the <see cref="ColLayoutType.Lg"/> breakpoint.</summary>
    public int Lg { get; set; } = 6;

    /// <summary>Gets or sets the number of cells spanned at the <see cref="ColLayoutType.Xl"/> breakpoint.</summary>
    public int Xl { get; set; } = 4;

    /// <summary>Gets or sets the number of cells spanned at the <see cref="ColLayoutType.Xxl"/> breakpoint.</summary>
    public int Xxl { get; set; } = 2;

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider serviceProvider)
        => new ColLayout
        {
            Xs = Xs,
            Sm = Sm,
            Md = Md,
            Lg = Lg,
            Xl = Xl,
            Xxl = Xxl,
        };

    /// <summary>Gets the responsive breakpoint that applies to the specified width.</summary>
    public static ColLayoutType GetLayoutStatus(double width)
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

    /// <inheritdoc />
    public override string ToString()
    {
        var cultureInfo = CultureInfo.CurrentCulture;
        var listSeparator = TokenizerHelper.GetNumericListSeparator(cultureInfo);

        return new StringBuilder()
            .Append(Xs.ToString(cultureInfo))
            .Append(listSeparator)
            .Append(Sm.ToString(cultureInfo))
            .Append(listSeparator)
            .Append(Md.ToString(cultureInfo))
            .Append(listSeparator)
            .Append(Lg.ToString(cultureInfo))
            .Append(listSeparator)
            .Append(Xl.ToString(cultureInfo))
            .Append(listSeparator)
            .Append(Xxl.ToString(cultureInfo))
            .ToString();
    }
}