namespace Atc.Wpf.SourceGenerators.Inspectors.Attributes;

internal static class RelayCommandInspector
{
    public static List<RelayCommandToGenerate> Inspect(
        INamedTypeSymbol classSymbol)
    {
        var result = new List<RelayCommandToGenerate>();

        var memberSymbols = classSymbol.GetMembers();

        foreach (var memberSymbol in memberSymbols)
        {
            if (memberSymbol is not IMethodSymbol methodSymbol)
            {
                continue;
            }

            var relayCommandAttributes = methodSymbol
                .GetAttributes()
                .Where(attr => attr.AttributeClass?.Name
                    is NameConstants.RelayCommandAttribute
                    or NameConstants.RelayCommand)
                .ToList();

            if (relayCommandAttributes.Count == 0)
            {
                continue;
            }

            foreach (var relayCommandAttribute in relayCommandAttributes)
            {
                AppendRelayCommandToGenerate(
                    methodSymbol,
                    memberSymbols,
                    relayCommandAttribute,
                    result);
            }
        }

        return result;
    }

    private static void AppendRelayCommandToGenerate(
        IMethodSymbol methodSymbol,
        ImmutableArray<ISymbol> memberSymbols,
        AttributeData relayCommandAttribute,
        List<RelayCommandToGenerate> relayCommandsToGenerate)
    {
        var commandName = relayCommandAttribute.ExtractRelayCommandName(methodSymbol.Name);
        var canExecuteName = relayCommandAttribute.ExtractRelayCommandCanExecuteName();
        var usePropertyForCanExecute = false;
        if (canExecuteName is not null)
        {
            usePropertyForCanExecute = memberSymbols.HasPropertyName(canExecuteName) ||
                                       memberSymbols.HasObservableFieldName(canExecuteName);
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
}