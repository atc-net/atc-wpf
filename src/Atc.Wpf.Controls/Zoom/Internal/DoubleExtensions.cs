namespace Atc.Wpf.Controls.Zoom.Internal;

internal static class DoubleExtensions
{
    public static bool IsWithinOnePercent(
        this double value,
        double testValue)
        => System.Math.Abs(value - testValue) < .01 * testValue;

    public static double ToRealNumber(
        this double value,
        double defaultValue = 0)
        => double.IsInfinity(value) ||
           double.IsNaN(value)
            ? defaultValue
            : value;
}