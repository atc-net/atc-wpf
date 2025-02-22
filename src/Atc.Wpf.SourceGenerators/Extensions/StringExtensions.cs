namespace Atc.Wpf.SourceGenerators.Extensions;

[SuppressMessage("Design", "CA1308:Teplace the call to 'ToLowerInvariant' with 'ToUpperInvariant'", Justification = "OK.")]
public static class StringExtensions
{
    private static readonly Regex NameofRegex = new(
        @"^nameof\((?<name>\w+)\)$",
        RegexOptions.Compiled | RegexOptions.ExplicitCapture,
        TimeSpan.FromMilliseconds(100));

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

    public static Dictionary<string, string?> ExtractAttributeConstructorParameters(
        this string value)
    {
        var result = new Dictionary<string, string?>(StringComparer.Ordinal);

        if (string.IsNullOrEmpty(value))
        {
            return result;
        }

        var content = RemoveOuterBrackets(value);
        var parameters = ExtractParameterString(content);

        if (string.IsNullOrEmpty(parameters))
        {
            return result;
        }

        var arguments = SplitArguments(parameters);

        ProcessArguments(arguments, result);

        return result;
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
        if (argumentValue is null)
        {
            throw new ArgumentNullException(nameof(argumentValue));
        }

        if (prefix is null)
        {
            throw new ArgumentNullException(nameof(prefix));
        }

        callback = null;
        if (!argumentValue.StartsWith(prefix, StringComparison.Ordinal))
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

    public static bool TryExtractArrayParameters(
        this string argumentValue,
        string prefix,
        out string[]? parameters)
    {
        if (argumentValue is null)
        {
            throw new ArgumentNullException(nameof(argumentValue));
        }

        if (prefix is null)
        {
            throw new ArgumentNullException(nameof(prefix));
        }

        parameters = null;
        if (!argumentValue.StartsWith(prefix, StringComparison.Ordinal))
        {
            return false;
        }

        var extractedValue = argumentValue
            .Substring(prefix.Length)
            .Trim();

        if (extractedValue.Length > 0)
        {
            extractedValue = extractedValue
                .Replace("nameof(", string.Empty)
                .Replace(")", string.Empty)
                .Replace(" ", string.Empty)
                .Replace("=", string.Empty)
                .Replace("[", string.Empty)
                .Replace("]", string.Empty)
                .Trim();
        }

        parameters = extractedValue.Split(',');
        return true;
    }

    private static string RemoveOuterBrackets(
        string value)
    {
        if (value.StartsWith("[", StringComparison.Ordinal) &&
            value.EndsWith("]", StringComparison.Ordinal))
        {
            return value.Substring(1, value.Length - 2);
        }

        return value;
    }

    private static string ExtractParameterString(
        string value)
    {
        var startIndex = value.IndexOf('(');
        if (startIndex < 0)
        {
            return string.Empty;
        }

        var count = 0;
        var endIndex = -1;
        for (var i = startIndex; i < value.Length; i++)
        {
            if (value[i] == '(')
            {
                count++;
            }
            else if (value[i] == ')')
            {
                count--;
                if (count != 0)
                {
                    continue;
                }

                endIndex = i;
                break;
            }
        }

        return endIndex < 0
            ? string.Empty
            : value.Substring(startIndex + 1, endIndex - startIndex - 1);
    }

    private static void ProcessArguments(
        IEnumerable<string> arguments,
        Dictionary<string, string?> result)
    {
        var unnamedIndex = 0;
        foreach (var arg in arguments)
        {
            var trimmedArg = arg.Trim();
            var eqIndex = trimmedArg.IndexOf('=');
            if (eqIndex > 0)
            {
                var key = trimmedArg.Substring(0, eqIndex).Trim();
                var value = trimmedArg.Substring(eqIndex + 1).Trim();

                value = StripQuotes(value);

                if (value.StartsWith("[", StringComparison.Ordinal) &&
                    value.EndsWith("]", StringComparison.Ordinal))
                {
                    value = value
                        .Substring(1, value.Length - 2)
                        .Trim();
                }

                result[key] = string.IsNullOrEmpty(value) ? string.Empty : value;
            }
            else
            {
                var key = unnamedIndex == 0 ? "Name" : unnamedIndex.ToString(CultureInfo.InvariantCulture);
                var value = StripQuotes(trimmedArg);

                var match = NameofRegex.Match(value);
                if (match.Success)
                {
                    value = match.Groups["name"].Value;
                }

                result[key] = string.IsNullOrEmpty(value)
                    ? string.Empty
                    : value;

                unnamedIndex++;
            }
        }
    }

    private static List<string> SplitArguments(
        string value)
    {
        var args = new List<string>();
        var currentArg = new StringBuilder();
        var bracketCount = 0;
        var inQuotes = false;
        var quoteChar = '\0';

        foreach (var c in value)
        {
            if (inQuotes)
            {
                currentArg.Append(c);
                if (c == quoteChar)
                {
                    inQuotes = false;
                }

                continue;
            }

            if (c is '"' or '\'')
            {
                inQuotes = true;
                quoteChar = c;
                currentArg.Append(c);
                continue;
            }

            switch (c)
            {
                case '[':
                    bracketCount++;
                    break;
                case ']':
                    bracketCount--;
                    break;
            }

            if (c == ',' && bracketCount == 0 && !inQuotes)
            {
                args.Add(currentArg.ToString().Trim());
                currentArg.Clear();
            }
            else
            {
                currentArg.Append(c);
            }
        }

        if (currentArg.Length > 0)
        {
            args.Add(currentArg.ToString().Trim());
        }

        return args;
    }

    private static string StripQuotes(
        string value)
    {
        if ((value.StartsWith("\"", StringComparison.Ordinal) && value.EndsWith("\"", StringComparison.Ordinal)) ||
            (value.StartsWith("'", StringComparison.Ordinal) && value.EndsWith("'", StringComparison.Ordinal)))
        {
            return value.Substring(1, value.Length - 2);
        }

        return value;
    }
}