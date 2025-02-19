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

    private static void AppendPropertiesToGenerate(
        IFieldSymbol fieldSymbol,
        ImmutableArray<AttributeData> fieldSymbolAttributes,
        AttributeData observablePropertyAttribute,
        List<ObservablePropertyToGenerate> propertiesToGenerate)
    {
        var backingFieldName = fieldSymbol.Name;
        var propertyType = fieldSymbol.Type.ToString();
        var propertyName = observablePropertyAttribute.ExtractPropertyName(backingFieldName);
        string? beforeChangedCallback = null;
        string? afterChangedCallback = null;

        var argumentValues = observablePropertyAttribute.ExtractConstructorArgumentValues();
        if (argumentValues is not null)
        {
            foreach (var argumentValue in argumentValues)
            {
                if (argumentValue.TryExtractCallbackContent(NameConstants.BeforeChangedCallback, out var beforeCallback))
                {
                    beforeChangedCallback = beforeCallback;
                }
                else if (argumentValue.TryExtractCallbackContent(NameConstants.AfterChangedCallback, out var afterCallback))
                {
                    afterChangedCallback = afterCallback;
                }
            }
        }

        var propertyNamesToInvalidate = fieldSymbolAttributes
            .ExtractPropertyNamesToInvalidate()
            .RemoveIsExist(propertyName);

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