// ReSharper disable CheckNamespace
namespace Atc.Wpf.SourceGenerators;

internal sealed class PropertyToGenerate(
    string name,
    string type,
    string backingFieldName)
{
    public string Name { get; } = name;

    public string Type { get; } = type;

    public string BackingFieldName { get; } = backingFieldName;

    public ICollection<string>? PropertyNamesToInvalidate { get; set; }
}