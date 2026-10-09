namespace Atc.Wpf.Controls.Sample;

/// <summary>
/// Interface for generating and manipulating sample data in demo views.
/// Implementations provide add/remove/reset/populate capabilities
/// that the <see cref="SampleDataController"/> exposes as buttons.
/// </summary>
public interface ISampleDataGenerator
{
    /// <summary>
    /// Gets a value indicating whether the generator supports adding items (shows the "Add Item" button).
    /// </summary>
    bool CanAddItems { get; }

    /// <summary>
    /// Gets a value indicating whether the generator supports removing items (shows the "Remove Item" button).
    /// </summary>
    bool CanRemoveItems { get; }

    /// <summary>
    /// Gets a value indicating whether the generator supports resetting the data (shows the "Reset" button).
    /// </summary>
    bool CanReset { get; }

    /// <summary>
    /// Gets a value indicating whether the generator supports populating sample data (shows the "Populate Sample Data" button).
    /// </summary>
    bool CanPopulateSampleData { get; }

    /// <summary>
    /// Adds an item to the data of the specified demo view model.
    /// </summary>
    void AddItem(object viewModel);

    /// <summary>
    /// Removes an item from the data of the specified demo view model.
    /// </summary>
    void RemoveItem(object viewModel);

    /// <summary>
    /// Resets the data of the specified demo view model.
    /// </summary>
    void Reset(object viewModel);

    /// <summary>
    /// Populates the specified demo view model with sample data.
    /// </summary>
    void PopulateSampleData(object viewModel);
}