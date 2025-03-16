// ReSharper disable CheckNamespace
namespace Atc.Wpf.SourceGenerators.Models.ToGenerate;

internal sealed class RelayCommandToGenerate(
    string commandName,
    string methodName,
    string[]? parameterTypes,
    string[]? parameterValues,
    string? canExecuteName,
    bool invertCanExecute,
    bool usePropertyForCanExecute,
    bool isAsync)
{
    public string CommandName { get; } = commandName;

    public string MethodName { get; } = methodName;

    public string[]? ParameterTypes { get; } = parameterTypes;

    public string[]? ParameterValues { get; } = parameterValues;

    public string? CanExecuteName { get; } = canExecuteName;

    public bool InvertCanExecute { get; } = invertCanExecute;

    public bool UsePropertyForCanExecute { get; } = usePropertyForCanExecute;

    public bool IsAsync { get; } = isAsync;

    public override string ToString()
        => $"{nameof(CommandName)}: {CommandName}, {nameof(MethodName)}: {MethodName}, {nameof(ParameterTypes)}.Count: {ParameterTypes?.Length}, {nameof(ParameterValues)}.Count: {ParameterValues?.Length}, {nameof(CanExecuteName)}: {CanExecuteName}, {nameof(InvertCanExecute)}: {InvertCanExecute}, {nameof(UsePropertyForCanExecute)}: {UsePropertyForCanExecute}, {nameof(IsAsync)}: {IsAsync}";
}