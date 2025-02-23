namespace Atc.Wpf.SourceGenerators.Extensions.CodeAnalysis;

internal static class AttributeDataExtensions
{
    public static Dictionary<string, string?> ExtractConstructorArgumentValues(
        this AttributeData attributeData)
    {
        var result = new Dictionary<string, string?>(StringComparer.Ordinal);

        if (attributeData.ConstructorArguments.Length == 0 &&
            attributeData.NamedArguments.Length == 0)
        {
            // Syntax check
            if (attributeData.ApplicationSyntaxReference is not null)
            {
                result = attributeData
                    .ApplicationSyntaxReference
                    .GetSyntax()
                    .ToFullString()
                    .ExtractAttributeConstructorParameters();
            }
        }
        else
        {
            // Runtime check
            var arrayIndex = 0;
            foreach (var arg in attributeData.ConstructorArguments)
            {
                if (arg.Kind == TypedConstantKind.Array)
                {
                    foreach (var typedConstant in arg.Values)
                    {
                        if (typedConstant.Value is null)
                        {
                            continue;
                        }

                        arrayIndex++;
                        result.Add(
                            arrayIndex.ToString(CultureInfo.InvariantCulture),
                            typedConstant.Value.ToString());
                    }
                }
                else if (arg.Value is not null)
                {
                    result.Add(
                        NameConstants.Name,
                        arg.Value.ToString());
                }
            }

            foreach (var arg in attributeData.NamedArguments)
            {
                result.Add(
                    arg.Key,
                    arg.Value.Value?.ToString());
            }
        }

        return result;
    }

    public static string ExtractClassFirstArgumentType(
        this AttributeData propertyAttribute,
        ref object? defaultValue)
    {
        var type = "int";
        if (propertyAttribute.AttributeClass is not { TypeArguments.Length: 1 })
        {
            return type;
        }

        var typeSymbol = propertyAttribute.AttributeClass.TypeArguments[0];
        type = typeSymbol.ToDisplayString().EnsureCSharpAliasIfNeeded();

        if (type == "bool")
        {
            defaultValue = "false";
        }

        return type;
    }
}