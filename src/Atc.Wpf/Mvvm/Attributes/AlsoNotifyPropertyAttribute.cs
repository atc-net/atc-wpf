// ReSharper disable CheckNamespace
namespace Atc.Wpf.Mvvm;

/// <summary>
/// Indicates that when the property generated for the decorated field changes,
/// notifications should also be raised for the specified dependent properties.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public sealed class AlsoNotifyPropertyAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AlsoNotifyPropertyAttribute"/> class.
    /// </summary>
    /// <param name="dependentProperties">
    /// The names of the dependent properties that should also raise a property change notification.
    /// </param>
    public AlsoNotifyPropertyAttribute(params string[] dependentProperties)
    {
        DependentProperties = dependentProperties;
    }

    /// <summary>
    /// Gets the dependent property names.
    /// </summary>
    public string[] DependentProperties { get; }
}