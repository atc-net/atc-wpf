namespace Atc.Wpf.SourceGenerators.Models.FrameworkElement;

internal sealed class AttachedPropertyToGenerate(
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
    : DependencyPropertyToGenerate(
        ownerType,
        name,
        type,
        defaultValue,
        propertyChangedCallback,
        coerceValueCallback,
        flags,
        defaultUpdateSourceTrigger,
        isAnimationProhibited,
        category,
        description);