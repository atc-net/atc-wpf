// ReSharper disable CheckNamespace
namespace Atc.Wpf;

[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "OK.")]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class DependencyPropertyAttribute<T>(string propertyName) : Attribute
{
    public string? PropertyName { get; } = propertyName;

    public Type Type { get; } = typeof(T);

    public object? DefaultValue { get; set; }

    public string? PropertyChangedCallback { get; set; }

    public string? CoerceValueCallback { get; set; }

    public FrameworkPropertyMetadataOptions Flags { get; set; }

    public UpdateSourceTrigger DefaultUpdateSourceTrigger { get; set; }

    public bool IsAnimationProhibited { get; set; }

    public string? Category { get; set; }

    public string? Description { get; set; }
}