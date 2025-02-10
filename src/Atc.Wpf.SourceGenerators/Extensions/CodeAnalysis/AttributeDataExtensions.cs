// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable InvertIf
namespace Atc.Wpf.SourceGenerators.Extensions.CodeAnalysis;

internal static class AttributeDataExtensions
{
    public static string ExtractRelayCommandName(
        this AttributeData attributeData,
        string defaultName)
    {
        string? relayCommandName = null;

        if (attributeData.ConstructorArguments.Length == 0 &&
            attributeData.NamedArguments.Length == 0)
        {
            // Syntax check
            var str = attributeData.ApplicationSyntaxReference?.GetSyntax().ToFullString();
            if (str is not null && str.Contains('('))
            {
                var parameters = str.ExtractAttributeParameters();
                if (parameters.Length > 0 && !parameters[0].Contains('='))
                {
                    relayCommandName = parameters[0];
                }
            }
        }
        else
        {
            // Runtime check
            foreach (var arg in attributeData.ConstructorArguments)
            {
                if (arg.Value is null)
                {
                    continue;
                }

                relayCommandName = arg.Value.ToString();
                break;
            }

            foreach (var arg in attributeData.NamedArguments)
            {
                if (arg.Key != NameConstants.CommandName)
                {
                    continue;
                }

                relayCommandName = arg.Value.Value?.ToString();
                break;
            }
        }

        relayCommandName ??= defaultName;

        if (relayCommandName.EndsWith(NameConstants.Handler, StringComparison.Ordinal))
        {
            relayCommandName = relayCommandName.Substring(0, relayCommandName.Length - NameConstants.Handler.Length);
        }

        return relayCommandName.EnsureFirstCharacterToUpper() + NameConstants.Command;
    }

    public static string? ExtractRelayCommandCanExecuteName(
        this AttributeData attributeData)
    {
        string? relayCommandCanExecuteName = null;

        if (attributeData.ConstructorArguments.Length == 0 &&
            attributeData.NamedArguments.Length == 0)
        {
            // Syntax check
            var str = attributeData.ApplicationSyntaxReference?.GetSyntax().ToFullString();
            if (str is not null && str.Contains('('))
            {
                var parameters = str.ExtractAttributeParameters();
                if (parameters.Length > 0)
                {
                    foreach (var parameter in parameters)
                    {
                        if (parameter.StartsWith(NameConstants.CanExecute, StringComparison.Ordinal))
                        {
                            relayCommandCanExecuteName = parameter.ExtractAttributeParameters()[0];
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            // Runtime check
            foreach (var arg in attributeData.NamedArguments)
            {
                if (arg.Key != NameConstants.CanExecute)
                {
                    continue;
                }

                relayCommandCanExecuteName = arg.Value.Value?.ToString();
                break;
            }
        }

        return relayCommandCanExecuteName?.EnsureFirstCharacterToUpper();
    }

    public static string ExtractPropertyName(
        this AttributeData attributeData,
        string backingFieldName)
    {
        string? propertyName = null;

        if (attributeData.ConstructorArguments.Length == 0 &&
            attributeData.NamedArguments.Length == 0)
        {
            // Syntax check
            var str = attributeData.ApplicationSyntaxReference?.GetSyntax().ToFullString();
            if (str is not null && str.Contains('('))
            {
                var parameters = str.ExtractAttributeParameters();
                if (parameters.Length > 0 && !parameters[0].Contains('='))
                {
                    propertyName = parameters[0];
                }
            }
        }
        else
        {
            // Runtime check
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
        }

        propertyName ??= backingFieldName.StripPrefixFromField();

        return propertyName.EnsureFirstCharacterToUpper();
    }

    public static string[]? ExtractConstructorArgumentValues(
        this AttributeData attributeData)
    {
        List<string>? result = null;

        if (attributeData.ConstructorArguments.Length == 0)
        {
            // Syntax check
            var str = attributeData.ApplicationSyntaxReference?.GetSyntax().ToFullString();
            if (str is not null && str.Contains('('))
            {
                var parameters = str.ExtractAttributeParameters();
                if (parameters.Length > 0)
                {
                    result = [];
                    result.AddRange(parameters.Select(x => x.ExtractAttributeParameters()[0]));
                }
            }
        }
        else
        {
            // Runtime check
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
            .FirstOrDefault(x => x.AttributeClass?.Name
                is NameConstants.ObservablePropertyAttribute
                or NameConstants.ObservableProperty);

        List<string>? result = null;

        var observablePropertyNamesToInvalidate = observablePropertyAttributes?.ExtractConstructorArgumentValues();
        if (observablePropertyNamesToInvalidate is not null)
        {
            result = [];
            result.AddRange(observablePropertyNamesToInvalidate);
        }

        var notifyPropertyChangedForAttributes = attributeData
            .Where(x => x.AttributeClass?.Name
                is NameConstants.NotifyPropertyChangedForAttribute
                or NameConstants.NotifyPropertyChangedFor)
            .ToList();

        var notifyPropertyChangedForNamesToInvalidate = notifyPropertyChangedForAttributes.ExtractConstructorArgumentValues();
        if (notifyPropertyChangedForNamesToInvalidate is not null)
        {
            result ??= [];
            result.AddRange(notifyPropertyChangedForNamesToInvalidate);
        }

        return result;
    }
}