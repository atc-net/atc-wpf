// ReSharper disable once CheckNamespace
namespace System.Windows;

public static class ThicknessExtensions
{
    /// <summary>
    /// Verifies if this Thickness contains only valid values
    /// The set of validity checks is passed as parameters.
    /// </summary>
    /// <param name='thickness'>Thickness value</param>
    /// <param name='allowNegative'>allows negative values</param>
    /// <param name='allowNaN'>allows Double.NaN</param>
    /// <param name='allowPositiveInfinity'>allows Double.PositiveInfinity</param>
    /// <param name='allowNegativeInfinity'>allows Double.NegativeInfinity</param>
    /// <returns>Whether or not the thickness complies to the range specified</returns>
    public static bool IsValid(
        this Thickness thickness,
        bool allowNegative,
        bool allowNaN,
        bool allowPositiveInfinity,
        bool allowNegativeInfinity)
    {
        if (!allowNegative && (thickness.Left < 0d ||
                               thickness.Right < 0d ||
                               thickness.Top < 0d ||
                               thickness.Bottom < 0d))
        {
            return false;
        }

        if (!allowNaN && (IsNaN(thickness.Left) ||
                          IsNaN(thickness.Right) ||
                          IsNaN(thickness.Top) ||
                          IsNaN(thickness.Bottom)))
        {
            return false;
        }

        if (!allowPositiveInfinity && (double.IsPositiveInfinity(thickness.Left) ||
                                       double.IsPositiveInfinity(thickness.Right) ||
                                       double.IsPositiveInfinity(thickness.Top) ||
                                       double.IsPositiveInfinity(thickness.Bottom)))
        {
            return false;
        }

        if (!allowNegativeInfinity && (double.IsNegativeInfinity(thickness.Left) ||
                                       double.IsNegativeInfinity(thickness.Right) ||
                                       double.IsNegativeInfinity(thickness.Top) ||
                                       double.IsNegativeInfinity(thickness.Bottom)))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Method to add up the left and right size as width, as well as the top and bottom size as height
    /// </summary>
    /// <param name="thickness">Thickness</param>
    /// <returns>Size</returns>
    public static Size CollapseThickness(
        this Thickness thickness)
        => new(thickness.Left + thickness.Right, thickness.Top + thickness.Bottom);

    /// <summary>
    /// Verifies if the Thickness contains only zero values
    /// </summary>
    /// <param name="thickness">Thickness</param>
    /// <returns>Size</returns>
    public static bool IsZero(
        this Thickness thickness)
        => thickness.Left.IsZero() &&
           thickness.Top.IsZero() &&
           thickness.Right.IsZero() &&
           thickness.Bottom.IsZero();

    /// <summary>
    /// Verifies if all the values in Thickness are same
    /// </summary>
    /// <param name="thickness">Thickness</param>
    /// <returns>true if yes, otherwise false</returns>
    public static bool IsUniform(
        this Thickness thickness)
        => thickness.Left.AreClose(thickness.Top) &&
           thickness.Left.AreClose(thickness.Right) &&
           thickness.Left.AreClose(thickness.Bottom);

    private static bool IsNaN(
        double value)
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