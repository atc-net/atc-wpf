// ReSharper disable CheckNamespace
namespace Atc.Wpf.SourceGenerators.Models.ViewModel;

internal sealed class RelayCommandToGenerate(
    string commandName,
    string methodName,
    string? parameterType,
    string? canExecuteMethodName,
    string[]? parameterValues,
    bool isAsync)
{
    public string CommandName { get; } = commandName;

    public string MethodName { get; } = methodName;

    public string? ParameterType { get; } = parameterType;

    public string? CanExecuteMethodName { get; } = canExecuteMethodName;

    public string[]? ParameterValues { get; } = parameterValues;

    public bool IsAsync { get; } = isAsync;

    public override string ToString()
        => $"{nameof(CommandName)}: {CommandName}, {nameof(MethodName)}: {MethodName}, {nameof(ParameterType)}: {ParameterType}, {nameof(CanExecuteMethodName)}: {CanExecuteMethodName}, {nameof(ParameterValues)}.Count: {ParameterValues?.Length}, {nameof(IsAsync)}: {IsAsync}";
}