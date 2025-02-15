// ReSharper disable CheckNamespace
namespace Atc.Wpf.SourceGenerators.Models.ViewModel;

internal sealed class RelayCommandToGenerate(
    string commandName,
    string methodName,
    string[]? parameterTypes,
    string[]? parameterValues,
    string? canExecuteMethodName,
    bool isAsync)
{
    public string CommandName { get; } = commandName;

    public string MethodName { get; } = methodName;

    public string[]? ParameterTypes { get; } = parameterTypes;

    public string[]? ParameterValues { get; } = parameterValues;

    public string? CanExecuteMethodName { get; } = canExecuteMethodName;

    public bool IsAsync { get; } = isAsync;

    public override string ToString()
        => $"{nameof(CommandName)}: {CommandName}, {nameof(MethodName)}: {MethodName}, {nameof(ParameterTypes)}.Count: {ParameterTypes?.Length}, {nameof(ParameterValues)}.Count: {ParameterValues?.Length}, {nameof(CanExecuteMethodName)}: {CanExecuteMethodName}, {nameof(IsAsync)}: {IsAsync}";
}