namespace Atc.Wpf.SourceGenerators.Inspectors;

internal static class ViewModelInspector
{
    internal static ViewModelInspectorResult Inspect(
        INamedTypeSymbol viewModelClassSymbol)
    {
        var relayCommandsToGenerate = new List<RelayCommandToGenerate>();
        var propertiesToGenerate = new List<PropertyToGenerate>();

        var viewModelMemberSymbols = viewModelClassSymbol.GetMembers();

        foreach (var memberSymbol in viewModelMemberSymbols)
        {
            switch (memberSymbol)
            {
                case IMethodSymbol methodSymbol:
                    AppendRelayCommandsToGenerate(methodSymbol, viewModelMemberSymbols, relayCommandsToGenerate);
                    break;
                case IFieldSymbol fieldSymbol:
                    AppendPropertiesToGenerate(fieldSymbol, propertiesToGenerate);
                    break;
            }
        }

        return new ViewModelInspectorResult(
            relayCommandsToGenerate,
            propertiesToGenerate);
    }

    private static void AppendRelayCommandsToGenerate(
        IMethodSymbol methodSymbol,
        ImmutableArray<ISymbol> viewModelMemberSymbols,
        List<RelayCommandToGenerate> relayCommandsToGenerate)
    {
        var relayCommandAttributes = methodSymbol.GetAttributes()
            .Where(attr => attr.AttributeClass?.Name
                is NameConstants.RelayCommandAttribute
                or NameConstants.RelayCommand)
            .ToList();

        if (relayCommandAttributes.Count == 0)
        {
            return;
        }

        foreach (var relayCommandAttribute in relayCommandAttributes)
        {
            AppendRelayCommandToGenerate(
                methodSymbol,
                viewModelMemberSymbols,
                relayCommandAttribute,
                relayCommandsToGenerate);
        }
    }

    private static void AppendRelayCommandToGenerate(
        IMethodSymbol methodSymbol,
        ImmutableArray<ISymbol> viewModelMemberSymbols,
        AttributeData relayCommandAttribute,
        List<RelayCommandToGenerate> relayCommandsToGenerate)
    {
        var commandName = relayCommandAttribute.ExtractRelayCommandName(methodSymbol.Name);
        var canExecuteName = relayCommandAttribute.ExtractRelayCommandCanExecuteName();
        var usePropertyForCanExecute = false;
        if (canExecuteName is not null)
        {
            usePropertyForCanExecute = viewModelMemberSymbols.HasPropertyName(canExecuteName) ||
                                       viewModelMemberSymbols.HasObservableFieldName(canExecuteName);
        }

        var parameterValues = relayCommandAttribute.ExtractRelayCommandParameterValues();

        List<string>? parameterTypes = null;
        if (methodSymbol.Parameters.Length > 0)
        {
            parameterTypes = methodSymbol.Parameters
                .Select(parameterSymbol => parameterSymbol.Type.ToDisplayString())
                .ToList();
        }

        var isAsync = methodSymbol.ReturnType.Name
            is NameConstants.Task
            or NameConstants.ValueTask;

        relayCommandsToGenerate.Add(
            new RelayCommandToGenerate(
                commandName,
                methodSymbol.Name,
                parameterTypes?.ToArray(),
                parameterValues,
                canExecuteName,
                usePropertyForCanExecute,
                isAsync));
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