// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// Internal helper that determines whether an enum value matches a converter parameter
/// that may be: a single <see cref="Enum"/>, a single string, a comma-separated string,
/// or an <see cref="IEnumerable"/> of <see cref="Enum"/>/string items.
/// </summary>
internal static class EnumParameterMatcher
{
    public static bool Matches(
        Enum enumValue,
        object parameter)
    {
        if (parameter is Enum parameterEnum)
        {
            return Equals(enumValue, parameterEnum);
        }

        var enumString = enumValue.ToString();

        if (parameter is string parameterString)
        {
            return MatchesString(enumString, parameterString);
        }

        if (parameter is IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                if (item is null)
                {
                    continue;
                }

                if (item is Enum itemEnum && Equals(enumValue, itemEnum))
                {
                    return true;
                }

                if (string.Equals(enumString, item.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        return string.Equals(enumString, parameter.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    private static bool MatchesString(
        string enumString,
        string parameterString)
    {
        if (parameterString.Length == 0)
        {
            return false;
        }

        if (!parameterString.Contains(',', StringComparison.Ordinal))
        {
            return string.Equals(enumString, parameterString, StringComparison.OrdinalIgnoreCase);
        }

        foreach (var part in parameterString.Split(','))
        {
            if (string.Equals(enumString, part.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}