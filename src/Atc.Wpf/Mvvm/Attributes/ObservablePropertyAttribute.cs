// ReSharper disable RedundantAttributeUsageProperty
// ReSharper disable CheckNamespace
namespace Atc.Wpf.Mvvm;

/// <summary>
/// Specifies a property in the ViewModel that should be generated for a field.
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

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservablePropertyAttribute"/> class.
    /// </summary>
    /// <param name="propertyName">The name of the property to generate</param>
    /// <param name="dependentProperties">The name of the dependent properties to generate</param>
    public ObservablePropertyAttribute(
        string propertyName,
        params string[] dependentProperties)
    {
        PropertyName = propertyName;
        DependentProperties = dependentProperties;
    }

    /// <summary>
    /// Gets the name of property to generate.
    /// </summary>
    public string? PropertyName { get; }

    /// <summary>
    /// Gets or sets the dependent property names.
    /// </summary>
    public string[]? DependentProperties { get; set; }

    /// <summary>
    /// Gets or sets the method(s) or expressions to execute before property changes.
    /// Example A: 'DoStuffA();'.
    /// Example B: nameof(DoStuffA).
    /// </summary>
    public string? BeforeChangedCallback { get; set; }

    /// <summary>
    /// Gets or sets the method(s) or expressions to execute after property changes.
    /// Example A: 'EntrySelected?.Invoke(this, selectedEntry); DoStuffB();'.
    /// Example b: nameof(DoStuffB).
    /// </summary>
    public string? AfterChangedCallback { get; set; }
}