namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: invokes a parameterless instance method on the bound value via reflection.
/// </summary>
/// <remarks>
/// <c>ConverterParameter</c> is the method name to invoke (string). The method must be parameterless
/// and public on the bound value's type. Returns the method's return value, or <see langword="null"/>
/// if the input is null, the parameter is missing, or no matching method is found. Method lookups
/// are cached per <c>(Type, methodName)</c> for performance.
/// </remarks>
public sealed class MethodToValueConverter : IValueConverter
{
    public static readonly MethodToValueConverter Instance = new();

    private static readonly ConcurrentDictionary<(Type, string), MethodInfo?> MethodCache = new();

    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is null ||
            parameter is not string methodName)
        {
            return null;
        }

        var methodInfo = MethodCache.GetOrAdd(
            (value.GetType(), methodName),
            static key => key.Item1.GetMethod(key.Item2, Type.EmptyTypes));

        return methodInfo?.Invoke(value, []);
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException(GetType().Name + " can only be used for one way conversion.");
}