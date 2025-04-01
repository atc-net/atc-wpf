namespace Atc.Wpf.Controls.ValueConverters;

public sealed class MethodToValueConverter : IValueConverter
{
    public static readonly MethodToValueConverter Instance = new();

    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value == null ||
            parameter is not string methodName)
        {
            return null;
        }

        var methodInfo = value.GetType().GetMethod(methodName, Type.EmptyTypes);
        if (methodInfo == null)
        {
            return null;
        }

        var returnValue = methodInfo.Invoke(value, []);
        return returnValue;
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException(GetType().Name + " can only be used for one way conversion.");
}