// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Wpf.SourceGenerators.Inspectors.Attributes;

internal static class ObservablePropertyInspector
{
    public static List<ObservablePropertyToGenerate> Inspect(
        INamedTypeSymbol classSymbol)
    {
        var result = new List<ObservablePropertyToGenerate>();

        var memberSymbols = classSymbol.GetMembers();

        foreach (var memberSymbol in memberSymbols)
        {
            if (memberSymbol is not IFieldSymbol fieldSymbol)
            {
                continue;
            }

            var fieldSymbolAttributes = fieldSymbol.GetAttributes();

            var observablePropertyAttribute = fieldSymbolAttributes
                .FirstOrDefault(x => x.AttributeClass?.Name
                    is NameConstants.ObservablePropertyAttribute
                    or NameConstants.ObservableProperty);

            if (observablePropertyAttribute is null)
            {
                continue;
            }

            AppendPropertiesToGenerate(
                fieldSymbol,
                fieldSymbolAttributes,
                observablePropertyAttribute,
                result);
        }

        return result;
    }

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
    private static void AppendPropertiesToGenerate(
        IFieldSymbol fieldSymbol,
        ImmutableArray<AttributeData> fieldSymbolAttributes,
        AttributeData observablePropertyAttribute,
        List<ObservablePropertyToGenerate> propertiesToGenerate)
    {
        var backingFieldName = fieldSymbol.Name;
        var propertyType = fieldSymbol.Type.ToString();

        var observableArgumentValues = observablePropertyAttribute.ExtractConstructorArgumentValues();

        var propertyName = observableArgumentValues.TryGetValue(NameConstants.Name, out var nameValue)
            ? nameValue!.EnsureFirstCharacterToUpper()
            : backingFieldName.StripPrefixFromField().EnsureFirstCharacterToUpper();

        List<string>? propertyNamesToInvalidate = null;
        if (observableArgumentValues.TryGetValue(NameConstants.DependentProperties, out var dependentPropertiesValue))
        {
            propertyNamesToInvalidate = [];

            propertyNamesToInvalidate.AddRange(
                dependentPropertiesValue!
                    .Split(',')
                    .Select(x => x.Trim().ExtractInnerContent()));
        }
        else
        {
            foreach (var argumentValue in observableArgumentValues)
            {
                if (argumentValue.Key
                    is NameConstants.Name
                    or NameConstants.BeforeChangedCallback
                    or NameConstants.AfterChangedCallback)
                {
                    continue;
                }

                propertyNamesToInvalidate ??= [];
                propertyNamesToInvalidate.Add(argumentValue.Value!.ExtractInnerContent());
            }
        }

        string? beforeChangedCallback = null;
        if (observableArgumentValues.TryGetValue(NameConstants.BeforeChangedCallback, out var beforeChangedCallbackValue))
        {
            beforeChangedCallback = beforeChangedCallbackValue;
        }

        string? afterChangedCallback = null;
        if (observableArgumentValues.TryGetValue(NameConstants.AfterChangedCallback, out var afterChangedCallbackValue))
        {
            afterChangedCallback = afterChangedCallbackValue;
        }

        var notifyPropertyChangedForAttributes = fieldSymbolAttributes
            .Where(x => x.AttributeClass?.Name
                is NameConstants.NotifyPropertyChangedForAttribute
                or NameConstants.NotifyPropertyChangedFor)
            .ToList();

        if (notifyPropertyChangedForAttributes.Count > 0)
        {
            propertyNamesToInvalidate ??= [];

            foreach (var notifyPropertyChangedForAttribute in notifyPropertyChangedForAttributes)
            {
                var argumentValues = notifyPropertyChangedForAttribute.ExtractConstructorArgumentValues();
                foreach (var argumentValue in argumentValues
                             .Where(parameter => !propertyNamesToInvalidate.Contains(parameter.Value!, StringComparer.Ordinal)))
                {
                    propertyNamesToInvalidate.Add(argumentValue.Value!);
                }
            }
        }

        propertiesToGenerate.Add(
            new ObservablePropertyToGenerate(
                propertyName,
                propertyType,
                backingFieldName)
            {
                PropertyNamesToInvalidate = propertyNamesToInvalidate,
                BeforeChangedCallback = beforeChangedCallback,
                AfterChangedCallback = afterChangedCallback,
            });
    }
}