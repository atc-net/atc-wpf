namespace Atc.Wpf.ValueConverters;

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