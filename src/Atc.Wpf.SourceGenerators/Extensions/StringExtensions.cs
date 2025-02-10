namespace Atc.Wpf.SourceGenerators.Extensions;

public static class StringExtensions
{
    public static string EnsureFirstCharacterToUpper(
        this string value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return value.Length switch
        {
            0 => value,
            1 => value.ToUpperInvariant(),
            _ => char.ToUpperInvariant(value[0]) + value.Substring(1),
        };
    }

    public static string[] ExtractAttributeParameters(
        this string value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var start = value.IndexOf('(');
        if (start == -1)
        {
            return [value];
        }

        var end = value.LastIndexOf(')');
        if (end == -1)
        {
            return [value];
        }

        var parameterString = value.Substring(start + 1, end - start - 1);
        var parameters = new List<string>();

        var currentParam = new StringBuilder();
        var insideBrackets = false;

        foreach (var c in parameterString)
        {
            insideBrackets = c switch
            {
                '[' => true,
                ']' => false,
                _ => insideBrackets,
            };

            if (c == ',' && !insideBrackets)
            {
                var trimmedParam = currentParam.ToString().Replace("\"", string.Empty).Trim();

                if (trimmedParam.Length > 0)
                {
                    parameters.Add(trimmedParam);
                }

                currentParam.Clear();
            }
            else
            {
                currentParam.Append(c);
            }
        }

        var lastParam = currentParam.ToString().Replace("\"", string.Empty).Trim();

        if (lastParam.Length > 0)
        {
            parameters.Add(lastParam);
        }

        return parameters.ToArray();
    }

    public static string ExtractParameterValue(
        this string value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var indexOfEqual = value.IndexOf('=');
        if (indexOfEqual != -1)
        {
            value = value.Substring(indexOfEqual);
        }

        return value
            .Replace("=", string.Empty)
            .Replace("[", string.Empty)
            .Replace("]", string.Empty)
            .Trim();
    }

    public static string StripPrefixFromField(
        this string fieldName)
    {
        if (fieldName is null)
        {
            throw new ArgumentNullException(nameof(fieldName));
        }

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