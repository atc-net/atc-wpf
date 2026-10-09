namespace Atc.Wpf.Controls.Sample;

/// <summary>
/// Links a DemoViewModel to its <see cref="ISampleDataGenerator"/> implementation.
/// The <see cref="SampleDataController"/> reads this attribute to discover
/// which generator to instantiate for data manipulation buttons.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class SampleDataGeneratorAttribute : Attribute
{
    /// <summary>
    /// Gets the type of the <see cref="ISampleDataGenerator"/> implementation to instantiate.
    /// </summary>
    public Type GeneratorType { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SampleDataGeneratorAttribute"/> class.
    /// </summary>
    /// <param name="generatorType">The generator type; must implement <see cref="ISampleDataGenerator"/>.</param>
    public SampleDataGeneratorAttribute(Type generatorType)
    {
        ArgumentNullException.ThrowIfNull(generatorType);

        if (!typeof(ISampleDataGenerator).IsAssignableFrom(generatorType))
        {
            throw new ArgumentException(
                $"Type '{generatorType.Name}' does not implement '{nameof(ISampleDataGenerator)}'.",
                nameof(generatorType));
        }

        GeneratorType = generatorType;
    }
}