// ReSharper disable RedundantAttributeUsageProperty
// ReSharper disable CheckNamespace
namespace Atc.Wpf.Mvvm;

/// <summary>
/// Specifies that a property in the ViewModel should be generated for a field.
/// The class need to inherits from <see cref="IViewModelBase"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public sealed class ObservablePropertyAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservablePropertyAttribute"/> class.
    /// </summary>
    public ObservablePropertyAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservablePropertyAttribute"/> class.
    /// </summary>
    /// <param name="propertyName">The name of the property to generate</param>
    public ObservablePropertyAttribute(
        string propertyName)
    {
        PropertyName = propertyName;
    }

    public ObservablePropertyAttribute(
        string propertyName,
        params string[] dependentProperties)
    {
        PropertyName = propertyName;
        DependentProperties = dependentProperties;
    }

    /// <summary>
    /// Gets or sets the name of property to generate.
    /// </summary>
    public string? PropertyName { get; }

    /// <summary>
    /// Gets the dependent property names.
    /// </summary>
    public string[]? DependentProperties { get; }
}