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
}