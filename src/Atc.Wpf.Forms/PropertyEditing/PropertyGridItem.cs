namespace Atc.Wpf.Forms.PropertyEditing;

/// <summary>
/// Represents a property item in the PropertyGrid control.
/// </summary>
public sealed class PropertyGridItem : INotifyPropertyChanged
{
    private object? value;
    private bool isExpanded;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyGridItem"/> class.
    /// </summary>
    /// <param name="propertyInfo">The property info for this item.</param>
    /// <param name="parentObject">The parent object that contains the property.</param>
    /// <param name="attributes">The attributes applied to the property.</param>
    public PropertyGridItem(
        PropertyInfo propertyInfo,
        object parentObject,
        IReadOnlyList<Attribute> attributes)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);
        ArgumentNullException.ThrowIfNull(parentObject);
        ArgumentNullException.ThrowIfNull(attributes);

        PropertyInfo = propertyInfo;
        ParentObject = parentObject;
        Attributes = attributes;

        PropertyName = propertyInfo.Name;
        PropertyType = propertyInfo.PropertyType;
        DisplayName = GetDisplayName(propertyInfo, attributes);
        Description = GetDescription(attributes);
        Category = GetCategory(attributes);
        IsReadOnly = DetermineIsReadOnly(propertyInfo, attributes);
        value = propertyInfo.GetValue(parentObject);
        Children = new ObservableCollection<PropertyGridItem>();
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the property name.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// Gets the display name for the property (from DisplayNameAttribute or formatted from property name).
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// Gets the description of the property (from DescriptionAttribute).
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the category of the property (from CategoryAttribute).
    /// </summary>
    public string Category { get; }

    /// <summary>
    /// Gets the type of the property.
    /// </summary>
    public Type PropertyType { get; }

    /// <summary>
    /// Gets or sets the current value of the property.
    /// </summary>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK")]
    public object? Value
    {
        get => value;
        set
        {
            if (Equals(this.value, value))
            {
                return;
            }

            var oldValue = this.value;
            this.value = value;

            // Update the property on the parent object
            if (!IsReadOnly && PropertyInfo.CanWrite)
            {
                try
                {
                    PropertyInfo.SetValue(ParentObject, value);
                }
                catch (Exception ex)
                {
                    // Revert to old value if setting fails
                    this.value = oldValue;
                    Trace.TraceError($"Failed to set property {PropertyName}: {ex.Message}");
                    return;
                }
            }

            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets a value indicating whether the property is read-only.
    /// </summary>
    public bool IsReadOnly { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the nested object is expanded.
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
    /// Gets a value indicating whether this property has nested children.
    /// </summary>
    public bool HasChildren => Children.Count > 0;

    /// <summary>
    /// Gets the child property items for complex/nested objects.
    /// </summary>
    public ObservableCollection<PropertyGridItem> Children { get; }

    /// <summary>
    /// Gets the attributes applied to this property.
    /// </summary>
    public IReadOnlyList<Attribute> Attributes { get; }

    /// <summary>
    /// Gets the PropertyInfo for this property.
    /// </summary>
    internal PropertyInfo PropertyInfo { get; }

    /// <summary>
    /// Gets the parent object that contains this property.
    /// </summary>
    internal object ParentObject { get; }

    /// <summary>
    /// Refreshes the value from the parent object.
    /// </summary>
    public void RefreshValue()
    {
        var currentValue = PropertyInfo.GetValue(ParentObject);
        if (!Equals(value, currentValue))
        {
            value = currentValue;
            OnPropertyChanged(nameof(Value));
        }

        // Refresh children as well
        foreach (var child in Children)
        {
            child.RefreshValue();
        }
    }

    /// <summary>
    /// Gets an attribute of the specified type from this property.
    /// </summary>
    /// <typeparam name="T">The type of attribute to get.</typeparam>
    /// <returns>The attribute if found; otherwise, null.</returns>
    public T? GetAttribute<T>()
        where T : Attribute
        => Attributes
            .OfType<T>()
            .FirstOrDefault();

    private static string GetDisplayName(
        PropertyInfo propertyInfo,
        IReadOnlyList<Attribute> attributes)
    {
        var displayNameAttr = attributes.OfType<DisplayNameAttribute>().FirstOrDefault();
        if (displayNameAttr is not null && !string.IsNullOrEmpty(displayNameAttr.DisplayName))
        {
            return displayNameAttr.DisplayName;
        }

        // Fallback: normalize Pascal case
        return propertyInfo.Name.NormalizePascalCase();
    }

    private static string GetDescription(IReadOnlyList<Attribute> attributes)
    {
        var descriptionAttr = attributes.OfType<DescriptionAttribute>().FirstOrDefault();
        return descriptionAttr?.Description ?? string.Empty;
    }

    private static string GetCategory(IReadOnlyList<Attribute> attributes)
    {
        var categoryAttr = attributes.OfType<CategoryAttribute>().FirstOrDefault();
        return categoryAttr?.Category ?? "Misc";
    }

    private static bool DetermineIsReadOnly(
        PropertyInfo propertyInfo,
        IReadOnlyList<Attribute> attributes)
    {
        // Check if property has a setter
        if (!propertyInfo.CanWrite)
        {
            return true;
        }

        // Check for ReadOnlyAttribute
        var readOnlyAttr = attributes.OfType<ReadOnlyAttribute>().FirstOrDefault();
        return readOnlyAttr is { IsReadOnly: true };
    }

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}