namespace Atc.Wpf.SourceGenerators.Extensions;

internal static class StringExtensions
{
    public static string EnsureFirstCharacterToUpper(
        this string value)
        => value.Length switch
        {
            0 => value,
            1 => value.ToUpperInvariant(),
            _ => char.ToUpperInvariant(value[0]) + value.Substring(1),
        };

    public static string[] ExtractAttributeParameters(
        this string value)
    {
        var start = value.IndexOf('(');
        var end = value.LastIndexOf(')');
        if (start == -1 || end == -1)
        {
            return [value];
        }

        var parameterString = value.Substring(start + 1, end - start - 1);
        var parameterList = parameterString.Split(',');

        var parameters = new List<string>();
        foreach (var parameter in parameterList)
        {
            var trimmedParameter = parameter
                .Replace("\"", string.Empty)
                .Trim();

            if (trimmedParameter.Length <= 0)
            {
                continue;
            }

            if (value.StartsWith("nameof(", StringComparison.Ordinal))
            {
                var valueStart = value.IndexOf('(') + 1;
                var valueEnd = value.LastIndexOf(')') - valueStart;
                trimmedParameter = value.Substring(valueStart, valueEnd).Trim();
            }

            parameters.Add(trimmedParameter);
        }

        return parameters.ToArray();
    }

    public static string StripPrefixFromField(
        this string fieldName)
    {
        if (fieldName.StartsWith("m_", StringComparison.Ordinal))
        {
            fieldName = fieldName.Substring(2);
        }
        else if (fieldName.StartsWith("_", StringComparison.Ordinal))
        {
            fieldName = fieldName.Substring(1);
        }

        return fieldName;
    }
}