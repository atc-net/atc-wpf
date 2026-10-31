namespace Atc.Wpf.Forms.Internal.Helpers;

/// <summary>
/// Converts model values and <see cref="RangeAttribute"/> bounds to the <see cref="decimal"/> and <see cref="int"/>
/// of the number boxes without throwing: NaN becomes 0, and values outside the target range (including ±∞ and
/// idioms such as <c>[Range(0, double.MaxValue)]</c>) are clamped to its ends.
/// </summary>
internal static class NumberClampHelper
{
    private const double DecimalMaxValueAsDouble = (double)decimal.MaxValue;

    /// <summary>
    /// Converts a <see cref="double"/> to a <see cref="decimal"/>, mapping NaN to 0 and clamping to the decimal range.
    /// </summary>
    public static decimal ToDecimal(double value)
    {
        if (double.IsNaN(value))
        {
            return 0m;
        }

        if (value >= DecimalMaxValueAsDouble)
        {
            return decimal.MaxValue;
        }

        if (value <= -DecimalMaxValueAsDouble)
        {
            return decimal.MinValue;
        }

        return (decimal)value;
    }

    /// <summary>
    /// Converts a <see cref="RangeAttribute"/> bound (a number or an invariant-culture string) to a <see cref="decimal"/>,
    /// clamping to the decimal range.
    /// </summary>
    public static decimal ToDecimal(object bound)
    {
        switch (bound)
        {
            case double d:
                return ToDecimal(d);
            case float f:
                return ToDecimal(f);
        }

        try
        {
            return Convert.ToDecimal(bound, CultureInfo.InvariantCulture);
        }
        catch (OverflowException)
        {
            return ToDecimal(Convert.ToDouble(bound, CultureInfo.InvariantCulture));
        }
    }

    /// <summary>
    /// Converts a <see cref="double"/> to an <see cref="int"/>, truncating, mapping NaN to 0 and clamping to the int range.
    /// </summary>
    public static int ToInt32(double value)
    {
        if (double.IsNaN(value))
        {
            return 0;
        }

        if (value >= int.MaxValue)
        {
            return int.MaxValue;
        }

        return value <= int.MinValue
            ? int.MinValue
            : (int)value;
    }

    /// <summary>
    /// Converts a <see cref="RangeAttribute"/> bound (a number or an invariant-culture string) to an <see cref="int"/>,
    /// clamping to the int range.
    /// </summary>
    public static int ToInt32(object bound)
    {
        try
        {
            return Convert.ToInt32(bound, CultureInfo.InvariantCulture);
        }
        catch (OverflowException)
        {
            return ToInt32(Convert.ToDouble(bound, CultureInfo.InvariantCulture));
        }
    }
}