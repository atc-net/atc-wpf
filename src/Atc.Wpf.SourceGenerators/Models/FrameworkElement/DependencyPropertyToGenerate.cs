namespace Atc.Wpf.SourceGenerators.Models.FrameworkElement;

internal sealed class DependencyPropertyToGenerate(
    string ownerType,
    string name,
    string type,
    object? defaultValue,
    string? propertyChangedCallback,
    string? coerceValueCallback,
    string? flags,
    string? defaultUpdateSourceTrigger,
    bool? isAnimationProhibited,
    string? category,
    string? description)
{
    public string OwnerType { get; } = ownerType;

    public string Name { get; } = name;

    public string Type { get; } = type;

    public object? DefaultValue { get; } = defaultValue;

    public string? PropertyChangedCallback { get; } = propertyChangedCallback;

    public string? CoerceValueCallback { get; } = coerceValueCallback;

    public string? Flags { get; } = flags;

    public string? DefaultUpdateSourceTrigger { get; } = defaultUpdateSourceTrigger;

    public bool? IsAnimationProhibited { get; } = isAnimationProhibited;

    public string? Category { get; } = category;

    public string? Description { get; } = description;

    public override string ToString()
        => $"{nameof(OwnerType)}: {OwnerType}, {nameof(Name)}: {Name}, {nameof(Type)}: {Type}, {nameof(DefaultValue)}: {DefaultValue}, {nameof(PropertyChangedCallback)}: {PropertyChangedCallback}, {nameof(CoerceValueCallback)}: {CoerceValueCallback}, {nameof(Flags)}: {Flags}, {nameof(DefaultUpdateSourceTrigger)}: {DefaultUpdateSourceTrigger}, {nameof(IsAnimationProhibited)}: {IsAnimationProhibited}, {nameof(Category)}: {Category}, {nameof(Description)}: {Description}";
}