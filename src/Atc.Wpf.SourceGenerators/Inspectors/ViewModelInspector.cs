namespace Atc.Wpf.SourceGenerators.Inspectors;

internal static class ViewModelInspector
{
    internal static ViewModelInspectorResult Inspect(
        INamedTypeSymbol viewModelClassSymbol)
    {
        var propertiesToGenerate = new List<PropertyToGenerate>();

        var viewModelMemberSymbols = viewModelClassSymbol.GetMembers();

        foreach (var memberSymbol in viewModelMemberSymbols)
        {
            switch (memberSymbol)
            {
                case IFieldSymbol fieldSymbol:
                    AppendPropertiesToGenerate(fieldSymbol, propertiesToGenerate);
                    break;
            }
        }

        return new ViewModelInspectorResult(
            propertiesToGenerate);
    }

    private static void AppendPropertiesToGenerate(
        IFieldSymbol fieldSymbol,
        List<PropertyToGenerate> propertiesToGenerate)
    {
        var attributes = fieldSymbol.GetAttributes();

        var observablePropertyAttribute = attributes
            .FirstOrDefault(x => x.AttributeClass?.Name
                is NameConstants.ObservablePropertyAttribute
                or NameConstants.ObservableProperty);

        if (observablePropertyAttribute is null)
        {
            return;
        }

        var backingFieldName = fieldSymbol.Name;
        var propertyType = fieldSymbol.Type.ToString();
        var propertyName = observablePropertyAttribute.ExtractPropertyName(backingFieldName);

        var propertyNamesToInvalidate = attributes
            .ExtractPropertyNamesToInvalidate()
            .RemoveIsExist(propertyName);

        propertiesToGenerate.Add(
            new PropertyToGenerate(
                propertyName,
                propertyType,
                backingFieldName)
            {
                PropertyNamesToInvalidate = propertyNamesToInvalidate,
            });
    }
}