// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
namespace Atc.Wpf.Controls.Layouts.Grid.Internal;

internal sealed class ColLayoutConverter : TypeConverter
{
    public override bool CanConvertFrom(
        ITypeDescriptorContext? context,
        Type sourceType)
    {
        ArgumentNullException.ThrowIfNull(context);

        return Type.GetTypeCode(sourceType) switch
        {
            TypeCode.Int16 => true,
            TypeCode.UInt16 => true,
            TypeCode.Int32 => true,
            TypeCode.UInt32 => true,
            TypeCode.Int64 => true,
            TypeCode.UInt64 => true,
            TypeCode.Single => true,
            TypeCode.Double => true,
            TypeCode.Decimal => true,
            TypeCode.String => true,
            _ => false,
        };
    }

    public override bool CanConvertTo(
        ITypeDescriptorContext? context,
        Type? destinationType)
        => destinationType == typeof(InstanceDescriptor) || destinationType == typeof(string);

    public override object ConvertFrom(
        ITypeDescriptorContext? context,
        CultureInfo? culture,
        object value)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(culture);

        if (value is null)
        {
            throw GetConvertFromException(value: null);
        }

        return value switch
        {
            string s => FromString(s, culture),
            double d => new ColLayout((int)d),
            _ => new ColLayout(Convert.ToInt32(value, culture)),
        };
    }

    [SuppressMessage("Usage", "MA0015:Specify the parameter name in ArgumentException", Justification = "OK")]
    [SecurityCritical]
    public override object ConvertTo(
        ITypeDescriptorContext? context,
        CultureInfo? culture,
        object? value,
        Type destinationType)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(culture);
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(destinationType);

        if (value is not ColLayout th)
        {
            throw new ArgumentException("UnexpectedParameterType");
        }

        if (destinationType == typeof(string))
        {
            return ToString(th, culture);
        }

        if (destinationType == typeof(InstanceDescriptor))
        {
            var ci = typeof(ColLayout).GetConstructor(
                new[]
                {
                    typeof(int),
                    typeof(int),
                    typeof(int),
                    typeof(int),
                    typeof(int),
                    typeof(int),
                });

            return new InstanceDescriptor(
                ci,
                new object[]
                {
                    th.Xs,
                    th.Sm,
                    th.Md,
                    th.Lg,
                    th.Xl,
                    th.Xxl,
                });
        }

        throw new ArgumentException("CannotConvertType");
    }

    private static string ToString(ColLayout th, CultureInfo cultureInfo)
    {
        var listSeparator = TokenizerHelper.GetNumericListSeparator(cultureInfo);

        var sb = new StringBuilder();
        sb.Append(th.Xs.ToString(cultureInfo));
        sb.Append(listSeparator);
        sb.Append(th.Sm.ToString(cultureInfo));
        sb.Append(listSeparator);
        sb.Append(th.Md.ToString(cultureInfo));
        sb.Append(listSeparator);
        sb.Append(th.Lg.ToString(cultureInfo));
        sb.Append(listSeparator);
        sb.Append(th.Xl.ToString(cultureInfo));
        sb.Append(listSeparator);
        sb.Append(th.Xxl.ToString(cultureInfo));
        return th.ToString();
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK - errorHandler will handle it")]
    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
    private static ColLayout FromString(
        string value,
        IFormatProvider cultureInfo)
    {
        var th = new TokenizerHelper(value, cultureInfo);
        var lengths = new int[6];
        var i = 0;

        while (th.NextToken())
        {
            if (i >= 6)
            {
                i = 7;
                break;
            }

            var currentToken = th.GetCurrentToken();
            if (currentToken is null)
            {
                lengths[i] = 0;
            }
            else
            {
                try
                {
                    lengths[i] = (int)TypeDescriptor.GetConverter(typeof(int)).ConvertFromString(currentToken)!;
                }
                catch
                {
                    lengths[i] = 0;
                }
            }

            i++;
        }

        return i switch
        {
            1 => new ColLayout(lengths[0]),
            2 => new ColLayout
            {
                Xs = lengths[0],
                Sm = lengths[1],
            },
            3 => new ColLayout
            {
                Xs = lengths[0],
                Sm = lengths[1],
                Md = lengths[2],
            },
            4 => new ColLayout
            {
                Xs = lengths[0],
                Sm = lengths[1],
                Md = lengths[2],
                Lg = lengths[3],
            },
            5 => new ColLayout
            {
                Xs = lengths[0],
                Sm = lengths[1],
                Md = lengths[2],
                Lg = lengths[3],
                Xl = lengths[4],
            },
            6 => new ColLayout
            {
                Xs = lengths[0],
                Sm = lengths[1],
                Md = lengths[2],
                Lg = lengths[3],
                Xl = lengths[4],
                Xxl = lengths[5],
            },
            _ => throw new FormatException("InvalidStringColLayout"),
        };
    }
}