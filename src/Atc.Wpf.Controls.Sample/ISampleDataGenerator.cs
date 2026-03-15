namespace Atc.Wpf.Controls.Sample;

/// <summary>
/// Interface for generating and manipulating sample data in demo views.
/// Implementations provide add/remove/reset/populate capabilities
/// that the <see cref="SampleDataController"/> exposes as buttons.
/// </summary>
public interface ISampleDataGenerator
{
    bool CanAddItems { get; }

    bool CanRemoveItems { get; }

    bool CanReset { get; }

    bool CanPopulateSampleData { get; }

    void AddItem(object viewModel);

    void RemoveItem(object viewModel);

    void Reset(object viewModel);

    void PopulateSampleData(object viewModel);
}