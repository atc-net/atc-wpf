namespace Atc.Wpf.Forms.Internal.Helpers;

/// <summary>
/// Bounds and conversions for the integral number types a form field can edit.
/// </summary>
internal static class IntegralTypeHelper
{
    /// <summary>
    /// Determines whether the type (or its nullable underlying type) is <see cref="uint"/>, <see cref="long"/> or
    /// <see cref="ulong"/>: an integral type whose values do not all fit in an <see cref="int"/>.
    /// </summary>
    public static bool IsWideIntegralType(Type? type)
    {
        var nonNullableType = GetNonNullableType(type);
        return nonNullableType == typeof(uint) ||
               nonNullableType == typeof(long) ||
               nonNullableType == typeof(ulong);
    }

    /// <summary>
    /// Gets the minimum and maximum value of an integral type (or its nullable underlying type).
    /// </summary>
    /// <returns><see langword="true"/> when the type is integral; otherwise <see langword="false"/>.</returns>
    public static bool TryGetBounds(
        Type? type,
        out decimal minimum,
        out decimal maximum)
    {
        (minimum, maximum) = GetNonNullableType(type) switch
        {
            { } t when t == typeof(byte) => (byte.MinValue, byte.MaxValue),
            { } t when t == typeof(sbyte) => (sbyte.MinValue, sbyte.MaxValue),
            { } t when t == typeof(short) => (short.MinValue, short.MaxValue),
            { } t when t == typeof(ushort) => (ushort.MinValue, ushort.MaxValue),
            { } t when t == typeof(int) => (int.MinValue, int.MaxValue),
            { } t when t == typeof(uint) => (uint.MinValue, uint.MaxValue),
            { } t when t == typeof(long) => (long.MinValue, long.MaxValue),
            { } t when t == typeof(ulong) => (ulong.MinValue, (decimal)ulong.MaxValue),
            _ => (0m, -1m),
        };

        return minimum <= maximum;
    }

    /// <summary>
    /// Gets a whole number as a value for an integral type (or its nullable underlying type): a <see cref="long"/>
    /// when it fits, otherwise a <see cref="decimal"/> without decimals (only for <see cref="ulong"/>).
    /// </summary>
    /// <returns>
    /// <see langword="true"/> when the type is integral and the value is a whole number within its bounds;
    /// otherwise <see langword="false"/>.
    /// </returns>
    public static bool TryGetWholeNumber(
        decimal value,
        Type? type,
        [NotNullWhen(true)] out object? result)
    {
        result = null;
        if (!TryGetBounds(type, out var minimum, out var maximum) ||
            value < minimum ||
            value > maximum ||
            decimal.Truncate(value) != value)
        {
            return false;
        }

        result = value <= long.MaxValue
            ? (object)(long)value
            : decimal.Truncate(value);
        return true;
    }

    private static Type? GetNonNullableType(Type? type)
        => type is null
            ? null
            : Nullable.GetUnderlyingType(type) ?? type;
}