// ReSharper disable CheckNamespace
namespace Atc.Wpf.SourceGenerators;

internal abstract class GenerateBase(
    string namespaceName,
    string className,
    string accessModifier)
{
    public string NamespaceName { get; } = namespaceName;

    public string ClassName { get; } = className;

    public string? ClassAccessModifier { get; } = accessModifier;

    public string GeneratedFileName => $"{NamespaceName}.{ClassName}.g.cs";
}