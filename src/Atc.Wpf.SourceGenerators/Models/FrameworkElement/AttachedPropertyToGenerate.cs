namespace Atc.Wpf.SourceGenerators.Models.FrameworkElement;

internal sealed class AttachedPropertyToGenerate(
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
    : BasePropertyToGenerate(
        ownerType,
        name,
        type,
        defaultValue,
        propertyChangedCallback,
        coerceValueCallback,
        validateValueCallback,
        flags,
        defaultUpdateSourceTrigger,
        isAnimationProhibited,
        category,
        description);