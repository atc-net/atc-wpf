// ReSharper disable RedundantAttributeUsageProperty
// ReSharper disable CheckNamespace
namespace Atc.Wpf.Mvvm;

/// <summary>
/// Specifies that a property in the ViewModel should be generated for a field.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class RelayCommandAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RelayCommandAttribute"/> class.
    /// </summary>
    public RelayCommandAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RelayCommandAttribute"/> class.
    /// </summary>
    /// <param name="commandName">The name of the relay command to generate</param>
    public RelayCommandAttribute(
        string commandName)
    {
        CommandName = commandName;
    }

    /// <summary>
    /// Gets or sets the name of command to generate.
    /// </summary>
    public string? CommandName { get; }

    public string? CanExecute { get; set; }
}