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

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
    private static void AppendRelayCommandToGenerate(
        IMethodSymbol methodSymbol,
        ImmutableArray<ISymbol> memberSymbols,
        AttributeData relayCommandAttribute,
        List<RelayCommandToGenerate> relayCommandsToGenerate)
    {
        var relayCommandArgumentValues = relayCommandAttribute.ExtractConstructorArgumentValues();

        var commandName = relayCommandArgumentValues.TryGetValue(NameConstants.Name, out var nameValue)
            ? nameValue!.EnsureFirstCharacterToUpper()
            : methodSymbol.Name.EnsureFirstCharacterToUpper();

        if (commandName.EndsWith(NameConstants.Handler, StringComparison.Ordinal))
        {
            commandName = commandName.Substring(0, commandName.Length - NameConstants.Handler.Length);
        }

        if (!commandName.EndsWith(NameConstants.Command, StringComparison.Ordinal))
        {
            commandName += NameConstants.Command;
        }

        if (commandName == methodSymbol.Name)
        {
            commandName += "X";
        }

        string? canExecuteName = null;
        if (relayCommandArgumentValues.TryGetValue(NameConstants.CanExecute, out var canExecuteNameValue))
        {
            canExecuteName = canExecuteNameValue!.ExtractInnerContent();
        }

        var usePropertyForCanExecute = false;
        if (canExecuteName is not null)
        {
            usePropertyForCanExecute = memberSymbols.HasPropertyName(canExecuteName) ||
                                       memberSymbols.HasObservablePropertyOrFieldName(canExecuteName);
        }

        var parameterValues = new List<string>();
        if (relayCommandArgumentValues.TryGetValue(NameConstants.ParameterValue, out var parameterValueValue))
        {
            parameterValues.Add(parameterValueValue!);
        }
        else if (relayCommandArgumentValues.TryGetValue(NameConstants.ParameterValues, out var parameterValuesValue))
        {
            parameterValues.AddRange(parameterValuesValue!.Split(',').Select(x => x.Trim()));
        }

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
                parameterValues.Count == 0 ? null : parameterValues.ToArray(),
                canExecuteName,
                usePropertyForCanExecute,
                isAsync));
    }
}