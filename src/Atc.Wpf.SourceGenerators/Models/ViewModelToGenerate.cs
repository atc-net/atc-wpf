// ReSharper disable CheckNamespace
namespace Atc.Wpf.SourceGenerators;

internal sealed class ViewModelToGenerate(
    string namespaceName,
    string className,
    string accessModifier)
{
    public string NamespaceName { get; } = namespaceName;

    public string ClassName { get; } = className;

    public string? ClassAccessModifier { get; } = accessModifier;

    public IList<RelayCommandToGenerate>? RelayCommandsToGenerate { get; set; }

    public IList<PropertyToGenerate>? PropertiesToGenerate { get; set; }

    public string GeneratedFileName => $"{NamespaceName}.{ClassName}.g.cs";

    public override string ToString()
        => $"{nameof(NamespaceName)}: {NamespaceName}, {nameof(ClassName)}: {ClassName}, {nameof(ClassAccessModifier)}: {ClassAccessModifier}, {nameof(RelayCommandsToGenerate)}.Count: {RelayCommandsToGenerate?.Count}, {nameof(PropertiesToGenerate)}.Count: {PropertiesToGenerate?.Count}, {nameof(GeneratedFileName)}: {GeneratedFileName}";
}