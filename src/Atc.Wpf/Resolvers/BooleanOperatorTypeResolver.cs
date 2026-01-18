namespace Atc.Wpf.Resolvers;

internal static class BooleanOperatorTypeResolver
{
    public static BooleanOperatorType Resolve(
        object? parameter,
        BooleanOperatorType defaultValue)
    {
        if (parameter is BooleanOperatorType operatorType)
        {
            return operatorType;
        }

        if (parameter is string str && Enum.TryParse<BooleanOperatorType>(str, ignoreCase: true, out var parsed))
        {
            return parsed;
        }

        return defaultValue;
    }
}