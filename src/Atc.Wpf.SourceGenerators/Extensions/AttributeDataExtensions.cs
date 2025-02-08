// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable InvertIf
namespace Atc.Wpf.SourceGenerators.Extensions;

internal static class AttributeDataExtensions
{
    public static string ExtractPropertyName(
        this AttributeData attributeData,
        string backingFieldName)
    {
        string? propertyName = null;

        foreach (var arg in attributeData.ConstructorArguments)
        {
            if (arg.Value is null)
            {
                continue;
            }

            propertyName = arg.Value.ToString();
            break;
        }

        foreach (var arg in attributeData.NamedArguments)
        {
            if (arg.Key != NameConstants.PropertyName)
            {
                continue;
            }

            propertyName = arg.Value.Value?.ToString();
            break;
        }

        if (propertyName is null)
        {
            propertyName = backingFieldName;
            if (propertyName.StartsWith("_", StringComparison.Ordinal))
            {
                propertyName = propertyName.Substring(1);
            }
            else if (propertyName.StartsWith("m_", StringComparison.Ordinal))
            {
                propertyName = propertyName.Substring(2);
            }
        }

        return propertyName.EnsureFirstCharacterToUpper();
    }

    public static string[]? ExtractConstructorArgumentValues(
        this AttributeData attributeData)
    {
        List<string>? result = null;

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

                    result ??= [];
                    result.Add(typedConstant.Value.ToString());
                }
            }
            else if (arg.Value is not null)
            {
                result ??= [];
                result.Add(arg.Value.ToString());
            }
        }

        return result?.ToArray();
    }

    public static string[]? ExtractConstructorArgumentValues(
        this List<AttributeData> attributeDataList)
    {
        List<string>? result = null;

        if (attributeDataList.Count > 0)
        {
            foreach (var attribute in attributeDataList)
            {
                var dependentProperties = attribute.ExtractConstructorArgumentValues();
                if (dependentProperties is null)
                {
                    continue;
                }

                result ??= [];
                result.AddRange(dependentProperties);
            }
        }

        return result?.ToArray();
    }

    public static IList<string>? ExtractPropertyNamesToInvalidate(
        this ImmutableArray<AttributeData> attributeData)
    {
        var observablePropertyAttributes = attributeData
            .FirstOrDefault(x => x.AttributeClass is not null &&
                                 x.AttributeClass.Name == NameConstants.ObservablePropertyAttribute);

        List<string>? result = null;

        var observablePropertyNamesToInvalidate = observablePropertyAttributes?.ExtractConstructorArgumentValues();
        if (observablePropertyNamesToInvalidate is not null)
        {
            result = [];
            result.AddRange(observablePropertyNamesToInvalidate);
        }

        var alsoNotifyPropertyAttributes = attributeData
            .Where(x => x.AttributeClass?.Name
                is NameConstants.AlsoNotifyPropertyAttribute
                or NameConstants.AlsoNotifyProperty)
            .ToList();

        var alsoNotifyPropertyNamesToInvalidate = alsoNotifyPropertyAttributes.ExtractConstructorArgumentValues();
        if (alsoNotifyPropertyNamesToInvalidate is not null)
        {
            result ??= [];
            result.AddRange(alsoNotifyPropertyNamesToInvalidate);
        }

        return result;
    }
}