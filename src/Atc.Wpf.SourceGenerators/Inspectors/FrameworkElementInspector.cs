namespace Atc.Wpf.SourceGenerators.Inspectors;

[SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
internal static class FrameworkElementInspector
{
    public static FrameworkElementInspectorResult Inspect(
        INamedTypeSymbol frameworkElementClassSymbol)
    {
        var dependencyPropertiesToGenerate = CollectDependencyPropertiesToGenerate(frameworkElementClassSymbol);
        var attachedPropertiesToGenerate = CollectAttachedPropertiesToGenerate(frameworkElementClassSymbol);

        return new FrameworkElementInspectorResult(
            dependencyPropertiesToGenerate,
            attachedPropertiesToGenerate);
    }

    private static List<DependencyPropertyToGenerate> CollectDependencyPropertiesToGenerate(
        INamedTypeSymbol frameworkElementClassSymbol)
    {
        var dependencyPropertiesToGenerate = new List<DependencyPropertyToGenerate>();

        var attributes = frameworkElementClassSymbol.GetAttributes();

        var dependencyPropertyAttributes = attributes
            .Where(x => x.AttributeClass?.Name
                is NameConstants.DependencyPropertyAttribute
                or NameConstants.DependencyProperty);

        var ownerType = frameworkElementClassSymbol.Name;

        foreach (var propertyAttribute in dependencyPropertyAttributes)
        {
            var propertyName = propertyAttribute.ExtractPropertyName(string.Empty);
            object? defaultValue = null;
            var type = propertyAttribute.ExtractClassFirstArgumentType(ref defaultValue);
            string? propertyChangedCallback = null;
            string? coerceValueCallback = null;
            string? flags = null;
            string? defaultUpdateSourceTrigger = null;
            bool? isAnimationProhibited = null;
            string? category = null;
            string? description = null;

            if (propertyAttribute.NamedArguments.Length == 0)
            {
                // Syntax check
                var str = propertyAttribute.ApplicationSyntaxReference?.GetSyntax().ToFullString();
                if (str is not null && str.Contains('('))
                {
                    var parameters = str.ExtractAttributeParameters();
                    foreach (var parameter in parameters)
                    {
                        if (parameter == propertyName)
                        {
                            continue;
                        }

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
                }
            }
            else
            {
                var parameters = propertyAttribute.ApplicationSyntaxReference?
                    .GetSyntax()
                    .ToFullString()
                    .ExtractAttributeParameters();

                // Runtime check
                foreach (var arg in propertyAttribute.NamedArguments)
                {
                    switch (arg.Key)
                    {
                        case NameConstants.DefaultValue:
                            defaultValue = arg.Value.Value;
                            break;
                        case NameConstants.PropertyChangedCallback:
                            propertyChangedCallback = arg.Value.Value?.ToString();
                            break;
                        case NameConstants.CoerceValueCallback:
                            coerceValueCallback = arg.Value.Value?.ToString();
                            break;
                        case NameConstants.Flags:
                            flags = parameters?
                                .FirstOrDefault(x => x.StartsWith(NameConstants.Flags, StringComparison.Ordinal))?
                                .Replace(NameConstants.Flags, string.Empty)
                                .Replace("=", string.Empty)
                                .Trim();
                            break;
                        case NameConstants.DefaultUpdateSourceTrigger:
                            defaultUpdateSourceTrigger = parameters?
                                .FirstOrDefault(x => x.StartsWith(NameConstants.DefaultUpdateSourceTrigger, StringComparison.Ordinal))?
                                .Replace(NameConstants.DefaultUpdateSourceTrigger, string.Empty)
                                .Replace("=", string.Empty)
                                .Trim();
                            break;
                        case NameConstants.IsAnimationProhibited:
                            isAnimationProhibited = "true".Equals(arg.Value.Value?.ToString(), StringComparison.OrdinalIgnoreCase);
                            break;
                        case NameConstants.Category:
                            category = arg.Value.Value?.ToString();
                            break;
                        case NameConstants.Description:
                            description = arg.Value.Value?.ToString();
                            break;
                    }
                }
            }

            defaultValue = defaultValue?.TransformDefaultValueIfNeeded();

            dependencyPropertiesToGenerate.Add(
                new DependencyPropertyToGenerate(
                    ownerType,
                    propertyName,
                    type,
                    defaultValue,
                    propertyChangedCallback,
                    coerceValueCallback,
                    flags,
                    defaultUpdateSourceTrigger,
                    isAnimationProhibited,
                    category,
                    description));
        }

        return dependencyPropertiesToGenerate;
    }

    private static List<AttachedPropertyToGenerate> CollectAttachedPropertiesToGenerate(
        INamedTypeSymbol frameworkElementClassSymbol)
    {
        var attachedPropertiesToGenerate = new List<AttachedPropertyToGenerate>();

        var attributes = frameworkElementClassSymbol.GetAttributes();

        var attachedPropertyAttributes = attributes
            .Where(x => x.AttributeClass?.Name
                is NameConstants.AttachedPropertyAttribute
                or NameConstants.AttachedProperty);

        var ownerType = frameworkElementClassSymbol.Name;

        foreach (var propertyAttribute in attachedPropertyAttributes)
        {
            var propertyName = propertyAttribute.ExtractPropertyName(string.Empty);
            object? defaultValue = null;
            var type = propertyAttribute.ExtractClassFirstArgumentType(ref defaultValue);
            string? propertyChangedCallback = null;
            string? coerceValueCallback = null;
            string? flags = null;
            string? defaultUpdateSourceTrigger = null;
            bool? isAnimationProhibited = null;
            string? category = null;
            string? description = null;

            if (propertyAttribute.NamedArguments.Length == 0)
            {
                // Syntax check
                var str = propertyAttribute.ApplicationSyntaxReference?.GetSyntax().ToFullString();
                if (str is not null && str.Contains('('))
                {
                    var parameters = str.ExtractAttributeParameters();
                    foreach (var parameter in parameters)
                    {
                        if (parameter == propertyName)
                        {
                            continue;
                        }

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
                }
            }
            else
            {
                var parameters = propertyAttribute.ApplicationSyntaxReference?
                    .GetSyntax()
                    .ToFullString()
                    .ExtractAttributeParameters();

                // Runtime check
                foreach (var arg in propertyAttribute.NamedArguments)
                {
                    switch (arg.Key)
                    {
                        case NameConstants.DefaultValue:
                            defaultValue = arg.Value.Value;
                            break;
                        case NameConstants.PropertyChangedCallback:
                            propertyChangedCallback = arg.Value.Value?.ToString();
                            break;
                        case NameConstants.CoerceValueCallback:
                            coerceValueCallback = arg.Value.Value?.ToString();
                            break;
                        case NameConstants.Flags:
                            flags = parameters?
                                .FirstOrDefault(x => x.StartsWith(NameConstants.Flags, StringComparison.Ordinal))?
                                .Replace(NameConstants.Flags, string.Empty)
                                .Replace("=", string.Empty)
                                .Trim();
                            break;
                        case NameConstants.DefaultUpdateSourceTrigger:
                            defaultUpdateSourceTrigger = parameters?
                                .FirstOrDefault(x => x.StartsWith(NameConstants.DefaultUpdateSourceTrigger, StringComparison.Ordinal))?
                                .Replace(NameConstants.DefaultUpdateSourceTrigger, string.Empty)
                                .Replace("=", string.Empty)
                                .Trim();
                            break;
                        case NameConstants.IsAnimationProhibited:
                            isAnimationProhibited = "true".Equals(arg.Value.Value?.ToString(), StringComparison.OrdinalIgnoreCase);
                            break;
                        case NameConstants.Category:
                            category = arg.Value.Value?.ToString();
                            break;
                        case NameConstants.Description:
                            description = arg.Value.Value?.ToString();
                            break;
                    }
                }
            }

            defaultValue = defaultValue?.TransformDefaultValueIfNeeded();

            attachedPropertiesToGenerate.Add(
                new AttachedPropertyToGenerate(
                    ownerType,
                    propertyName,
                    type,
                    defaultValue,
                    propertyChangedCallback,
                    coerceValueCallback,
                    flags,
                    defaultUpdateSourceTrigger,
                    isAnimationProhibited,
                    category,
                    description));
        }

        return attachedPropertiesToGenerate;
    }
}