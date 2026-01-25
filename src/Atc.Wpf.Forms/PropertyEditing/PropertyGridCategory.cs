namespace Atc.Wpf.Forms.PropertyEditing;

/// <summary>
/// Represents a category of properties in the PropertyGrid control.
/// </summary>
public sealed class PropertyGridCategory : INotifyPropertyChanged
{
    private bool isExpanded = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyGridCategory"/> class.
    /// </summary>
    /// <param name="name">The category name.</param>
    public PropertyGridCategory(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        Name = name;
        DisplayName = name.NormalizePascalCase();
        Properties = [];
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the category name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the display name for the category.
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// Gets or sets a value indicating whether this category is expanded.
    /// </summary>
    public bool IsExpanded
    {
        get => isExpanded;
        set
        {
            if (isExpanded == value)
            {
                return;
            }

            isExpanded = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets the properties in this category.
    /// </summary>
    public ObservableCollection<PropertyGridItem> Properties { get; }

    /// <summary>
    /// Gets the number of properties in this category.
    /// </summary>
    public int PropertyCount => Properties.Count;

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}