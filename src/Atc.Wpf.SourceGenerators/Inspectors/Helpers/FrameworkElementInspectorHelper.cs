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
            var argumentValues = propertyAttribute.ExtractConstructorArgumentValues();

            string? propertyName = null;
            if (argumentValues.TryGetValue(NameConstants.Name, out var nameValue))
            {
                propertyName = nameValue!.EnsureFirstCharacterToUpper();
            }

            if (propertyName is null)
            {
                continue;
            }

            object? defaultValue = null;
            var type = propertyAttribute.ExtractClassFirstArgumentType(ref defaultValue);

            if (argumentValues.TryGetValue(NameConstants.DefaultValue, out var defaultValueValue))
            {
                defaultValue = defaultValueValue;
            }

            string? propertyChangedCallback = null;
            if (argumentValues.TryGetValue(NameConstants.PropertyChangedCallback, out var propertyChangedCallbackValue))
            {
                propertyChangedCallback = propertyChangedCallbackValue!.ExtractInnerContent();
            }

            string? coerceValueCallback = null;
            if (argumentValues.TryGetValue(NameConstants.CoerceValueCallback, out var coerceValueCallbackValue))
            {
                coerceValueCallback = coerceValueCallbackValue!.ExtractInnerContent();
            }

            string? validateValueCallback = null;
            if (argumentValues.TryGetValue(NameConstants.ValidateValueCallback, out var validateValueCallbackValue))
            {
                validateValueCallback = validateValueCallbackValue!.ExtractInnerContent();
            }

            string? flags = null;
            if (argumentValues.TryGetValue(NameConstants.Flags, out var flagsValue))
            {
                flags = flagsValue;
            }

            string? defaultUpdateSourceTrigger = null;
            if (argumentValues.TryGetValue(NameConstants.DefaultUpdateSourceTrigger, out var defaultUpdateSourceTriggerValue))
            {
                defaultUpdateSourceTrigger = defaultUpdateSourceTriggerValue;
            }

            bool? isAnimationProhibited = null;
            if (argumentValues.TryGetValue(NameConstants.IsAnimationProhibited, out var isAnimationProhibitedValue))
            {
                isAnimationProhibited = isAnimationProhibitedValue!.Equals("true", StringComparison.OrdinalIgnoreCase);
            }

            string? category = null;
            if (argumentValues.TryGetValue(NameConstants.Category, out var categoryValue))
            {
                category = categoryValue;
            }

            string? description = null;
            if (argumentValues.TryGetValue(NameConstants.Description, out var descriptionValue))
            {
                description = descriptionValue;
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
}