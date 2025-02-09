// ReSharper disable CheckNamespace
namespace Atc.Wpf.SourceGenerators;

internal sealed class RelayCommandToGenerate(
    string commandName,
    string methodName,
    string? parameterType,
    string? canExecuteMethodName,
    bool isAsync)
{
    public string CommandName { get; } = commandName;

    public string MethodName { get; } = methodName;

    public string? ParameterType { get; } = parameterType;

    public string? CanExecuteMethodName { get; } = canExecuteMethodName;

    public bool IsAsync { get; } = isAsync;
}