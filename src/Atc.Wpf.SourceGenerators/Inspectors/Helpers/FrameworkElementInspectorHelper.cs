namespace Atc.Wpf.SourceGenerators.Inspectors.Helpers;

[SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
internal static class FrameworkElementInspectorHelper
{
    public static List<T> InspectPropertyAttributes<T>(
        INamedTypeSymbol classSymbol,
        IEnumerable<AttributeData> propertyAttributes)
        where T : BaseFrameworkElementPropertyToGenerate
    {
        var propertiesToGenerate = new List<T>();

        var ownerType = classSymbol.Name;

        foreach (var propertyAttribute in propertyAttributes)
        {
            var propertyName = propertyAttribute.ExtractPropertyName(string.Empty);
            object? defaultValue = null;
            var type = propertyAttribute.ExtractClassFirstArgumentType(ref defaultValue);
            string? propertyChangedCallback = null;
            string? coerceValueCallback = null;
            string? validateValueCallback = null;
            string? flags = null;
            string? defaultUpdateSourceTrigger = null;
            bool? isAnimationProhibited = null;
            string? category = null;
            string? description = null;

            var parameters = propertyAttribute.ApplicationSyntaxReference?
                .GetSyntax()
                .ToFullString()
                .ExtractAttributeParameters();

            if (propertyAttribute.NamedArguments.Length == 0 && parameters is not null)
            {
                // Syntax-based extraction
                foreach (var parameter in parameters)
                {
                    ExtractsAttributeParametersFromTheSyntaxCheck(
                        parameter,
                        ref defaultValue,
                        ref propertyChangedCallback,
                        ref coerceValueCallback,
                        ref validateValueCallback,
                        ref flags,
                        ref defaultUpdateSourceTrigger,
                        ref isAnimationProhibited,
                        ref category,
                        ref description);
                }
            }
            else
            {
                // Runtime-based extraction
                foreach (var arg in propertyAttribute.NamedArguments)
                {
                    ExtractsNamedArgumentValuesAtRuntime(
                        arg.Key,
                        arg.Value.Value,
                        ref defaultValue,
                        ref propertyChangedCallback,
                        ref coerceValueCallback,
                        ref validateValueCallback,
                        ref flags,
                        ref defaultUpdateSourceTrigger,
                        ref isAnimationProhibited,
                        ref category,
                        ref description,
                        parameters);
                }
            }

            defaultValue = defaultValue is null && type.IsSimpleType()
                ? SimpleTypeFactory.CreateDefaultValueAsStrForType(type)
                : defaultValue?.TransformDefaultValueIfNeeded(type);

            propertiesToGenerate.Add(
                (T)Activator.CreateInstance(
                    typeof(T),
                    ownerType,
                    propertyName,
                    type,
                    defaultValue,
                    propertyChangedCallback,
                    coerceValueCallback,
                    validateValueCallback,
                    flags,
                    defaultUpdateSourceTrigger,
                    isAnimationProhibited,
                    category,
                    description)!);
        }

        return propertiesToGenerate;
    }

    private static void ExtractsAttributeParametersFromTheSyntaxCheck(
        string parameter,
        ref object? defaultValue,
        ref string? propertyChangedCallback,
        ref string? coerceValueCallback,
        ref string? validateValueCallback,
        ref string? flags,
        ref string? defaultUpdateSourceTrigger,
        ref bool? isAnimationProhibited,
        ref string? category,
        ref string? description)
    {
        var parameterTrim = parameter.Replace(" ", string.Empty);

        if (parameterTrim.StartsWith(NameConstants.DefaultValue + "=", StringComparison.CurrentCulture))
        {
            defaultValue = parameter.ExtractParameterValue();
        }
        else if (parameterTrim.StartsWith(NameConstants.PropertyChangedCallback + "=", StringComparison.CurrentCulture))
        {
            propertyChangedCallback = parameter.ExtractParameterValue();
        }
        else if (parameterTrim.StartsWith(NameConstants.CoerceValueCallback + "=", StringComparison.CurrentCulture))
        {
            coerceValueCallback = parameter.ExtractParameterValue();
        }
        else if (parameterTrim.StartsWith(NameConstants.ValidateValueCallback + "=", StringComparison.CurrentCulture))
        {
            validateValueCallback = parameter.ExtractParameterValue();
        }
        else if (parameterTrim.StartsWith(NameConstants.Flags + "=", StringComparison.CurrentCulture))
        {
            flags = parameter.ExtractParameterValue();
        }
        else if (parameterTrim.StartsWith(NameConstants.DefaultUpdateSourceTrigger + "=", StringComparison.CurrentCulture))
        {
            defaultUpdateSourceTrigger = parameter.ExtractParameterValue();
        }
        else if (parameterTrim.StartsWith(NameConstants.IsAnimationProhibited + "=", StringComparison.CurrentCulture))
        {
            isAnimationProhibited = "true".Equals(parameter.ExtractParameterValue(), StringComparison.OrdinalIgnoreCase);
        }
        else if (parameterTrim.StartsWith(NameConstants.Category + "=", StringComparison.CurrentCulture))
        {
            category = parameter.ExtractParameterValue();
        }
        else if (parameterTrim.StartsWith(NameConstants.Description + "=", StringComparison.CurrentCulture))
        {
            description = parameter.ExtractParameterValue();
        }
    }

    private static void ExtractsNamedArgumentValuesAtRuntime(
        string key,
        object? value,
        ref object? defaultValue,
        ref string? propertyChangedCallback,
        ref string? coerceValueCallback,
        ref string? validateValueCallback,
        ref string? flags,
        ref string? defaultUpdateSourceTrigger,
        ref bool? isAnimationProhibited,
        ref string? category,
        ref string? description,
        IEnumerable<string>? parameters)
    {
        switch (key)
        {
            case NameConstants.DefaultValue:
                defaultValue = value;
                break;
            case NameConstants.PropertyChangedCallback:
                propertyChangedCallback = value?.ToString();
                break;
            case NameConstants.CoerceValueCallback:
                coerceValueCallback = value?.ToString();
                break;
            case NameConstants.ValidateValueCallback:
                validateValueCallback = value?.ToString();
                break;
            case NameConstants.Flags:
                flags = parameters.ExtractParameterValueFromList(NameConstants.Flags);
                break;
            case NameConstants.DefaultUpdateSourceTrigger:
                defaultUpdateSourceTrigger = parameters.ExtractParameterValueFromList(NameConstants.DefaultUpdateSourceTrigger);
                break;
            case NameConstants.IsAnimationProhibited:
                isAnimationProhibited = "true".Equals(value?.ToString(), StringComparison.OrdinalIgnoreCase);
                break;
            case NameConstants.Category:
                category = value?.ToString();
                break;
            case NameConstants.Description:
                description = value?.ToString();
                break;
        }
    }
}