// ReSharper disable once CheckNamespace
namespace System.Windows;

public static class CornerRadiusExtensions
{
    /// <summary>
    /// Verifies if this CornerRadius contains only valid values
    /// The set of validity checks is passed as parameters.
    /// </summary>
    /// <param name='cornerRadius'>CornerRadius value</param>
    /// <param name='allowNegative'>allows negative values</param>
    /// <param name='allowNaN'>allows Double.NaN</param>
    /// <param name='allowPositiveInfinity'>allows Double.PositiveInfinity</param>
    /// <param name='allowNegativeInfinity'>allows Double.NegativeInfinity</param>
    /// <returns>Whether or not the CornerRadius complies to the range specified</returns>
    public static bool IsValid(
        this CornerRadius cornerRadius,
        bool allowNegative,
        bool allowNaN,
        bool allowPositiveInfinity,
        bool allowNegativeInfinity)
    {
        if (!allowNegative && (cornerRadius.TopLeft < 0d || cornerRadius.TopRight < 0d || cornerRadius.BottomLeft < 0d || cornerRadius.BottomRight < 0d))
        {
            return false;
        }

        if (!allowNaN && (IsNaN(cornerRadius.TopLeft) ||
                          IsNaN(cornerRadius.TopRight) ||
                          IsNaN(cornerRadius.BottomLeft) ||
                          IsNaN(cornerRadius.BottomRight)))
        {
            return false;
        }

        if (!allowPositiveInfinity && (double.IsPositiveInfinity(cornerRadius.TopLeft) ||
                                       double.IsPositiveInfinity(cornerRadius.TopRight) ||
                                       double.IsPositiveInfinity(cornerRadius.BottomLeft) ||
                                       double.IsPositiveInfinity(cornerRadius.BottomRight)))
        {
            return false;
        }

        if (!allowNegativeInfinity && (double.IsNegativeInfinity(cornerRadius.TopLeft) ||
                                       double.IsNegativeInfinity(cornerRadius.TopRight) ||
                                       double.IsNegativeInfinity(cornerRadius.BottomLeft) ||
                                       double.IsNegativeInfinity(cornerRadius.BottomRight)))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Verifies if the CornerRadius contains only zero values
    /// </summary>
    /// <param name="cornerRadius">CornerRadius</param>
    /// <returns>Size</returns>
    public static bool IsZero(this CornerRadius cornerRadius)
        => cornerRadius.TopLeft.IsZero() &&
           cornerRadius.TopRight.IsZero() &&
           cornerRadius.BottomRight.IsZero() &&
           cornerRadius.BottomLeft.IsZero();

    /// <summary>
    /// Verifies if the CornerRadius contains same values
    /// </summary>
    /// <param name="corner">CornerRadius</param>
    /// <returns>true if yes, otherwise false</returns>
    public static bool IsUniform(this CornerRadius corner)
    {
        var topLeft = corner.TopLeft;
        return topLeft.AreClose(corner.TopRight) &&
               topLeft.AreClose(corner.BottomLeft) &&
               topLeft.AreClose(corner.BottomRight);
    }

    private static bool IsNaN(double value)
    {
        var t = new NanUnion
        {
            DoubleValue = value,
        };

        var exp = t.UintValue & 0xfff0000000000000;
        var man = t.UintValue & 0x000fffffffffffff;

        return exp is 0x7ff0000000000000 or 0xfff0000000000000 && (man != 0);
    }
}