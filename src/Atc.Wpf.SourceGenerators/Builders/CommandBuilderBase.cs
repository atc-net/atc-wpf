namespace Atc.Wpf.SourceGenerators.Builders;

[SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
internal abstract class CommandBuilderBase : BuilderBase
{
    public void GenerateRelayCommands(
        CommandBuilderBase builder,
        IEnumerable<RelayCommandToGenerate>? relayCommandsToGenerate)
    {
        if (relayCommandsToGenerate is null)
        {
            return;
        }

        foreach (var relayCommandToGenerate in relayCommandsToGenerate)
        {
            builder.AppendLineBeforeMember();

            var interfaceType = relayCommandToGenerate.IsAsync
                ? NameConstants.IRelayCommandAsync
                : NameConstants.IRelayCommand;

            var implementationType = relayCommandToGenerate.IsAsync
                ? NameConstants.RelayCommandAsync
                : NameConstants.RelayCommand;

            if (relayCommandToGenerate.ParameterValues is null)
            {
                GenerateRelayCommandWithOutParameterValues(builder, relayCommandToGenerate, interfaceType, implementationType);
            }
            else
            {
                GenerateRelayCommandWithParameterValues(builder, relayCommandToGenerate, interfaceType, implementationType);
            }
        }
    }

    private static void GenerateRelayCommandWithOutParameterValues(
        CommandBuilderBase builder,
        RelayCommandToGenerate rc,
        string interfaceType,
        string implementationType)
    {
        if (rc.ParameterTypes is null || rc.ParameterTypes.Length == 0)
        {
            var cmd = GenerateCommandLine(
                interfaceType,
                implementationType,
                rc.CommandName,
                rc.MethodName,
                rc.CanExecuteName,
                rc.InvertCanExecute,
                rc.UsePropertyForCanExecute);
            builder.AppendLine(cmd);
        }
        else if (rc.ParameterTypes.Length == 1)
        {
            var parameterType = rc.ParameterTypes[0];

            if (parameterType.EndsWith(nameof(CancellationToken), StringComparison.Ordinal))
            {
                var cmd = GenerateCommandLine(
                    interfaceType,
                    implementationType,
                    rc.CommandName,
                    $"{rc.MethodName}(CancellationToken.None)",
                    rc.CanExecuteName,
                    rc.InvertCanExecute,
                    isLambda: true);
                builder.AppendLine(cmd);
            }
            else
            {
                var generic = $"<{parameterType}>";
                var cmd = GenerateCommandLine(
                    $"{interfaceType}{generic}",
                    $"{implementationType}{generic}",
                    rc.CommandName,
                    rc.MethodName,
                    rc.CanExecuteName,
                    rc.InvertCanExecute,
                    rc.UsePropertyForCanExecute);
                builder.AppendLine(cmd);
            }
        }
        else
        {
            var (tupleGeneric, constructorParametersMulti, filteredConstructorParameters) = GetConstructorParametersWithParameterTypes(rc);
            if (rc.UsePropertyForCanExecute)
            {
                var cmd = GenerateCommandLine(
                    $"{interfaceType}{tupleGeneric}",
                    $"{implementationType}{tupleGeneric}",
                    rc.CommandName,
                    $"x => {rc.MethodName}({constructorParametersMulti})",
                    rc.CanExecuteName is null ? null : rc.InvertCanExecute ? $"x => !{rc.CanExecuteName}" : $"x => {rc.CanExecuteName}");
                builder.AppendLine(cmd);
            }
            else
            {
                var cmd = GenerateCommandLine(
                    $"{interfaceType}{tupleGeneric}",
                    $"{implementationType}{tupleGeneric}",
                    rc.CommandName,
                    $"x => {rc.MethodName}({constructorParametersMulti})",
                    rc.CanExecuteName is null ? null : rc.InvertCanExecute ? $"x => !{rc.CanExecuteName}({filteredConstructorParameters})" : $"x => {rc.CanExecuteName}({filteredConstructorParameters})");
                builder.AppendLine(cmd);
            }
        }
    }

    private static void GenerateRelayCommandWithParameterValues(
        CommandBuilderBase builder,
        RelayCommandToGenerate rc,
        string interfaceType,
        string implementationType)
    {
        if (rc.ParameterValues!.Length == 1)
        {
            var cmd = GenerateCommandLine(
                interfaceType,
                implementationType,
                rc.CommandName,
                $"() => {rc.MethodName}({rc.ParameterValues[0]})",
                rc.CanExecuteName);
            builder.AppendLine(cmd);
        }
        else
        {
            var constructorParameters = string.Join(", ", rc.ParameterValues!);
            if (rc.CanExecuteName is null)
            {
                var cmd = GenerateCommandLine(
                    interfaceType,
                    implementationType,
                    rc.CommandName,
                    $"() => {rc.MethodName}({constructorParameters})");
                builder.AppendLine(cmd);
            }
            else
            {
                if (rc.UsePropertyForCanExecute)
                {
                    var cmd = GenerateCommandLine(
                        interfaceType,
                        implementationType,
                        rc.CommandName,
                        $"() => {rc.MethodName}({constructorParameters})",
                        rc.InvertCanExecute ? $"!{rc.CanExecuteName}" : $"{rc.CanExecuteName}");
                    builder.AppendLine(cmd);
                }
                else
                {
                    var cmd = GenerateCommandLine(
                        interfaceType,
                        implementationType,
                        rc.CommandName,
                        $"() => {rc.MethodName}({constructorParameters})",
                        rc.InvertCanExecute ? $"!{rc.CanExecuteName}({constructorParameters})" : $"{rc.CanExecuteName}({constructorParameters})");
                    builder.AppendLine(cmd);
                }
            }
        }
    }

    private static (
        string Generic,
        string ConstructorParameters,
        string FilteredConstructorParameters) GetConstructorParametersWithParameterTypes(
        RelayCommandToGenerate rc)
    {
        var filteredParameterTypes = rc.ParameterTypes!.Where(x => !x.EndsWith(nameof(CancellationToken), StringComparison.Ordinal));
        var generic = $"<({string.Join(", ", filteredParameterTypes)})>";

        var constructorParametersList = new List<string>();
        var tupleItemNumber = 0;

        foreach (var parameterType in rc.ParameterTypes!)
        {
            if (parameterType.EndsWith(nameof(CancellationToken), StringComparison.Ordinal))
            {
                constructorParametersList.Add("CancellationToken.None");
            }
            else
            {
                tupleItemNumber++;
                constructorParametersList.Add($"x.Item{tupleItemNumber}");
            }
        }

        var constructorParameters = string.Join(", ", constructorParametersList);
        var filteredConstructorParameters = string.Join(", ", constructorParametersList.Where(x => !x.Contains(nameof(CancellationToken))));

        return (generic, constructorParameters, filteredConstructorParameters);
    }

    private static string GenerateCommandLine(
        string interfaceType,
        string implementationType,
        string commandName,
        string constructorParameters,
        string? canExecuteName = null,
        bool invertCanExecute = false,
        bool usePropertyForCanExecute = false,
        bool isLambda = false)
    {
        var lambdaPrefix = isLambda ? "() => " : string.Empty;
        var commandInstance = $"new {implementationType}({lambdaPrefix}{constructorParameters}";

        if (canExecuteName is not null)
        {
            if (usePropertyForCanExecute)
            {
                if (interfaceType.Contains('<'))
                {
                    commandInstance += invertCanExecute
                        ? $", _ => !{canExecuteName}"
                        : $", _ => {canExecuteName}";
                }
                else
                {
                    commandInstance += invertCanExecute
                        ? $", () => !{canExecuteName}"
                        : $", () => {canExecuteName}";
                }
            }
            else
            {
                commandInstance += invertCanExecute
                    ? $", !{canExecuteName}"
                    : $", {canExecuteName}";
            }
        }

        commandInstance += ");";

        return $"public {interfaceType} {commandName} => {commandInstance}";
    }
}