namespace Atc.Wpf.SourceGenerators.Extensions;

[SuppressMessage("Design", "CA1308:Teplace the call to 'ToLowerInvariant' with 'ToUpperInvariant'", Justification = "OK.")]
public static class StringExtensions
{
    private static readonly Dictionary<string, string> TypeAliases = new(StringComparer.Ordinal)
    {
        { nameof(Boolean), "bool" },
        { nameof(Byte), "byte" },
        { nameof(SByte), "sbyte" },
        { nameof(Char), "char" },
        { nameof(Decimal), "decimal" },
        { nameof(Double), "double" },
        { nameof(Single), "float" },
        { nameof(Int32), "int" },
        { nameof(UInt32), "uint" },
        { nameof(Int16), "short" },
        { nameof(UInt16), "ushort" },
        { nameof(Int64), "long" },
        { nameof(UInt64), "ulong" },
        { nameof(Object), "object" },
        { nameof(String), "string" },
    };

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

    public static string EnsureFirstCharacterToLower(
        this string value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return value.Length switch
        {
            0 => value,
            1 => value.ToLowerInvariant(),
            _ => char.ToLowerInvariant(value[0]) + value.Substring(1),
        };
    }

    public static string ExtractInnerContent(
        this string value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var start = value.IndexOf('(');
        if (start == -1)
        {
            return value;
        }

        var end = value.LastIndexOf(')');
        return end == -1
            ? value
            : value.Substring(start + 1, end - start - 1);
    }

    public static string[] ExtractAttributeParameters(
        this string value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var parameterString = value.ExtractInnerContent();

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

        var lastParam = currentParam
            .ToString()
            .Replace("\"", string.Empty)
            .Trim();

        if (lastParam.Length <= 0)
        {
            return parameters.ToArray();
        }

        if (parameters.Count > 0 &&
            parameters[parameters.Count - 1].EndsWith("(this", StringComparison.Ordinal))
        {
            parameters[parameters.Count - 1] = parameters[parameters.Count - 1] + ", " + lastParam;
        }
        else
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

        value = value
            .Replace("=", string.Empty)
            .Replace("[", string.Empty)
            .Replace("]", string.Empty)
            .Replace("\"", string.Empty)
            .Trim();

        var start = value.IndexOf('(');
        if (start == -1)
        {
            return value;
        }

        var end = value.IndexOf(')');
        value = value.Substring(start + 1, end - start - 1);

        return value;
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

    public static string EnsureValidRelayCommandName(
        this string relayCommandName)
    {
        if (relayCommandName is null)
        {
            throw new ArgumentNullException(nameof(relayCommandName));
        }

        if (relayCommandName.EndsWith(NameConstants.Command + NameConstants.Command, StringComparison.Ordinal))
        {
            relayCommandName = relayCommandName.Substring(0, relayCommandName.Length - NameConstants.Command.Length);
        }

        if (relayCommandName.EndsWith(NameConstants.Command, StringComparison.Ordinal))
        {
            return relayCommandName.EnsureFirstCharacterToUpper() + "X";
        }

        if (relayCommandName.EndsWith(NameConstants.Handler, StringComparison.Ordinal))
        {
            relayCommandName = relayCommandName.Substring(0, relayCommandName.Length - NameConstants.Handler.Length);
        }

        return relayCommandName.EndsWith(NameConstants.Command, StringComparison.Ordinal)
            ? relayCommandName.EnsureFirstCharacterToUpper()
            : relayCommandName.EnsureFirstCharacterToUpper() + NameConstants.Command;
    }

    public static string EnsureCSharpAliasIfNeeded(
        this string typeName)
        => TypeAliases.TryGetValue(typeName, out var alias)
            ? alias
            : typeName;

    public static bool IsSimpleType(
        this string value)
        => value
            is "bool"
            or "decimal"
            or "double"
            or "float"
            or "int"
            or "long"
            or "string";

    public static bool TryExtractCallbackContent(
        this string argumentValue,
        string prefix,
        out string? callback)
    {
        callback = null;
        if (argumentValue is null ||
            prefix is null ||
            !argumentValue.StartsWith(prefix, StringComparison.Ordinal))
        {
            return false;
        }

        var extractedValue = argumentValue
            .Substring(prefix.Length)
            .Trim();

        if (extractedValue.Length > 0)
        {
            extractedValue = extractedValue
                .Substring(1)// Remove the first character '='
                .Trim();
        }

        if ((!extractedValue.StartsWith("nameof(", StringComparison.Ordinal) ||
             !extractedValue.EndsWith(")", StringComparison.Ordinal)) &&
            !extractedValue.EndsWith(";", StringComparison.Ordinal))
        {
            return false;
        }

        callback = extractedValue;
        return true;
    }
}