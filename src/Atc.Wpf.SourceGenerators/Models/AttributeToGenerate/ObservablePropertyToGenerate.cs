// ReSharper disable CheckNamespace
namespace Atc.Wpf.SourceGenerators.Models.ToGenerate;

internal sealed class ObservablePropertyToGenerate(
    string name,
    string type,
    string backingFieldName)
{
    public string Name { get; } = name;

    public string Type { get; } = type;

    public string BackingFieldName { get; } = backingFieldName;

    public ICollection<string>? PropertyNamesToInvalidate { get; set; }

    public string? BeforeChangedCallback { get; set; }

    public string? AfterChangedCallback { get; set; }

    public bool BroadcastOnChange { get; set; }

    public override string ToString()
        => $"{nameof(Name)}: {Name}, {nameof(Type)}: {Type}, {nameof(BackingFieldName)}: {BackingFieldName}, {nameof(PropertyNamesToInvalidate)}.Count: {PropertyNamesToInvalidate?.Count}, {nameof(BeforeChangedCallback)}: {BeforeChangedCallback}, {nameof(AfterChangedCallback)}: {AfterChangedCallback}, {nameof(BroadcastOnChange)}: {BroadcastOnChange}";
}