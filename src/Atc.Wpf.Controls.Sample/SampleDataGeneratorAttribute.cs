namespace Atc.Wpf.Controls.Sample;

/// <summary>
/// Links a DemoViewModel to its <see cref="ISampleDataGenerator"/> implementation.
/// The <see cref="SampleDataController"/> reads this attribute to discover
/// which generator to instantiate for data manipulation buttons.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class SampleDataGeneratorAttribute : Attribute
{
    public Type GeneratorType { get; }

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