namespace Atc.Wpf.SourceGenerators.Models.FrameworkElement;

internal abstract class BasePropertyToGenerate(
    string ownerType,
    string name,
    string type,
    object? defaultValue,
    string? propertyChangedCallback,
    string? coerceValueCallback,
    string? validateValueCallback,
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

    public string? ValidateValueCallback { get; } = validateValueCallback;

    public string? Flags { get; } = flags;

    public string? DefaultUpdateSourceTrigger { get; } = defaultUpdateSourceTrigger;

    public bool? IsAnimationProhibited { get; } = isAnimationProhibited;

    public string? Category { get; } = category;

    public string? Description { get; } = description;

    public bool HasAnyMetadata
        => DefaultValue is not null ||
           PropertyChangedCallback is not null ||
           CoerceValueCallback is not null ||
           Flags is not null ||
           DefaultUpdateSourceTrigger is not null ||
           IsAnimationProhibited is not null;

    public bool HasAnyValidateValueCallback
        => ValidateValueCallback is not null;

    public override string ToString()
        => $"{nameof(OwnerType)}: {OwnerType}, {nameof(Name)}: {Name}, {nameof(Type)}: {Type}, {nameof(DefaultValue)}: {DefaultValue}, {nameof(PropertyChangedCallback)}: {PropertyChangedCallback}, {nameof(CoerceValueCallback)}: {CoerceValueCallback}, {nameof(ValidateValueCallback)}: {ValidateValueCallback}, {nameof(Flags)}: {Flags}, {nameof(DefaultUpdateSourceTrigger)}: {DefaultUpdateSourceTrigger}, {nameof(IsAnimationProhibited)}: {IsAnimationProhibited}, {nameof(Category)}: {Category}, {nameof(Description)}: {Description}";
}