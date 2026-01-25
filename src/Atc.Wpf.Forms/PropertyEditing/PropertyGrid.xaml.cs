namespace Atc.Wpf.Forms.PropertyEditing;

/// <summary>
/// A control for displaying and editing properties of an object.
/// </summary>
public partial class PropertyGrid
{
    private readonly Factories.PropertyGridEditorFactory editorFactory = new();
    private INotifyPropertyChanged? subscribedObject;

    [DependencyProperty(PropertyChangedCallback = nameof(OnSelectedObjectChanged))]
    private object? selectedObject;

    [DependencyProperty(
        DefaultValue = true,
        PropertyChangedCallback = nameof(OnShowCategoriesChanged))]
    private bool showCategories;

    [DependencyProperty(
        DefaultValue = true,
        PropertyChangedCallback = nameof(OnShowDescriptionsChanged))]
    private bool showDescriptions;

    [DependencyProperty(
        DefaultValue = false,
        PropertyChangedCallback = nameof(OnIsReadOnlyChanged))]
    private bool isReadOnly;

    [DependencyProperty(
        DefaultValue = PropertySortMode.Categorized,
        PropertyChangedCallback = nameof(OnSortModeChanged))]
    private PropertySortMode sortMode;

    [DependencyProperty(DefaultValue = "Misc")]
    private string defaultCategoryName;

    [DependencyProperty(DefaultValue = 3)]
    private int maxNestedDepth;

    /// <summary>
    /// Gets the categories collection.
    /// </summary>
    public ObservableCollection<PropertyGridCategory> Categories { get; } = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyGrid"/> class.
    /// </summary>
    public PropertyGrid()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Refreshes the property grid from the selected object.
    /// Call this method when properties on the selected object have changed
    /// but the object does not implement INotifyPropertyChanged.
    /// </summary>
    public void Refresh()
    {
        if (SelectedObject is null)
        {
            return;
        }

        foreach (var category in Categories)
        {
            foreach (var item in category.Properties)
            {
                item.RefreshValue();
            }
        }
    }

    /// <summary>
    /// Registers a custom editor for specific types.
    /// </summary>
    /// <param name="editor">The editor to register.</param>
    public void RegisterEditor(Abstractions.IPropertyGridEditor editor)
    {
        editorFactory.RegisterEditor(editor);
    }

    private static void OnSelectedObjectChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var propertyGrid = (PropertyGrid)d;
        propertyGrid.UnsubscribeFromPropertyChanges();
        propertyGrid.BuildPropertyGrid();
        propertyGrid.SubscribeToPropertyChanges();
    }

    private static void OnShowCategoriesChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var propertyGrid = (PropertyGrid)d;
        propertyGrid.BuildPropertyGrid();
    }

    private static void OnShowDescriptionsChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var propertyGrid = (PropertyGrid)d;
        propertyGrid.DescriptionPanel.Visibility = (bool)e.NewValue
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private static void OnIsReadOnlyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var propertyGrid = (PropertyGrid)d;
        propertyGrid.BuildPropertyGrid();
    }

    private static void OnSortModeChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var propertyGrid = (PropertyGrid)d;
        propertyGrid.BuildPropertyGrid();
    }

    private void SubscribeToPropertyChanges()
    {
        if (SelectedObject is INotifyPropertyChanged notifyPropertyChanged)
        {
            subscribedObject = notifyPropertyChanged;
            subscribedObject.PropertyChanged += OnSelectedObjectPropertyChanged;
        }
    }

    private void UnsubscribeFromPropertyChanges()
    {
        if (subscribedObject is not null)
        {
            subscribedObject.PropertyChanged -= OnSelectedObjectPropertyChanged;
            subscribedObject = null;
        }
    }

    private void OnSelectedObjectPropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        // Find and refresh the specific property item
        foreach (var category in Categories)
        {
            var item = category.Properties.FirstOrDefault(p => p.PropertyName == e.PropertyName);
            if (item is not null)
            {
                item.RefreshValue();
                break;
            }
        }
    }

    private void BuildPropertyGrid()
    {
        Categories.Clear();
        ContentPanel.Children.Clear();

        if (SelectedObject is null)
        {
            return;
        }

        var items = ExtractPropertyItems(SelectedObject, 0);
        var groupedItems = GroupByCategory(items);

        foreach (var category in groupedItems)
        {
            Categories.Add(category);
            BuildCategoryUI(category);
        }
    }

    private void BuildCategoryUI(PropertyGridCategory category)
    {
        var expander = new Expander
        {
            Header = category.DisplayName,
            IsExpanded = category.IsExpanded,
            Margin = new Thickness(0, 0, 0, 5),
            BorderBrush = (Brush)FindResource("AtcApps.Brushes.Gray7"),
            BorderThickness = new Thickness(0, 0, 0, 1),
        };

        expander.Expanded += (_, _) => category.IsExpanded = true;
        expander.Collapsed += (_, _) => category.IsExpanded = false;

        var propertiesPanel = new StackPanel
        {
            Margin = new Thickness(10, 0, 0, 0),
        };

        foreach (var item in category.Properties)
        {
            var propertyRow = BuildPropertyRow(item);
            propertiesPanel.Children.Add(propertyRow);
        }

        if (ShowCategories)
        {
            expander.Content = propertiesPanel;
            ContentPanel.Children.Add(expander);
        }
        else
        {
            // Without categories, just add properties directly
            ContentPanel.Children.Add(propertiesPanel);
        }
    }

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
    private Grid BuildPropertyRow(PropertyGridItem item)
    {
        var grid = new Grid
        {
            Margin = new Thickness(0, 2, 0, 2),
        };

        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star), MinWidth = 80 });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star), MinWidth = 100 });

        // Label
        var label = new TextBlock
        {
            Text = item.DisplayName,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(5, 2, 5, 2),
            TextTrimming = TextTrimming.CharacterEllipsis,
            ToolTip = !string.IsNullOrEmpty(item.Description) ? item.Description : null,
        };

        Grid.SetColumn(label, 0);
        grid.Children.Add(label);

        // Editor
        var editor = editorFactory.CreateEditorControl(item);
        if (editor is not null)
        {
            editor.Margin = new Thickness(5, 2, 5, 2);
            editor.VerticalAlignment = VerticalAlignment.Center;

            if (IsReadOnly && editor is Control control)
            {
                control.IsEnabled = false;
            }

            // Hook up focus for description panel
            editor.GotFocus += (_, _) => UpdateDescription(item);

            Grid.SetColumn(editor, 1);
            grid.Children.Add(editor);
        }
        else
        {
            // Fallback: display value as text
            var valueText = new TextBlock
            {
                Text = item.Value?.ToString() ?? "(null)",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5, 2, 5, 2),
                Foreground = SystemColors.GrayTextBrush,
                FontStyle = FontStyles.Italic,
            };

            Grid.SetColumn(valueText, 1);
            grid.Children.Add(valueText);
        }

        // Build nested children if any
        if (item.HasChildren)
        {
            var outerPanel = new StackPanel();
            outerPanel.Children.Add(grid);

            var nestedExpander = new Expander
            {
                Header = new TextBlock
                {
                    Text = $"{item.DisplayName} (expand)",
                    FontStyle = FontStyles.Italic,
                },
                IsExpanded = item.IsExpanded,
                Margin = new Thickness(20, 0, 0, 0),
            };

            nestedExpander.Expanded += (_, _) => item.IsExpanded = true;
            nestedExpander.Collapsed += (_, _) => item.IsExpanded = false;

            var nestedPanel = new StackPanel();
            foreach (var child in item.Children)
            {
                var childRow = BuildPropertyRow(child);
                nestedPanel.Children.Add(childRow);
            }

            nestedExpander.Content = nestedPanel;
            outerPanel.Children.Add(nestedExpander);

            return new Grid { Children = { outerPanel } };
        }

        return grid;
    }

    private List<PropertyGridItem> ExtractPropertyItems(
        object obj,
        int depth)
    {
        var items = new List<PropertyGridItem>();
        var type = obj.GetType();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var propertyInfo in properties)
        {
            // Skip non-readable properties
            if (!propertyInfo.CanRead)
            {
                continue;
            }

            var attributes = propertyInfo.GetCustomAttributes(inherit: true)
                .OfType<Attribute>()
                .ToList();

            // Check for Browsable(false)
            var browsableAttr = attributes.OfType<BrowsableAttribute>().FirstOrDefault();
            if (browsableAttr is { Browsable: false })
            {
                continue;
            }

            // Check for IgnoreDisplay attribute
            if (propertyInfo.HasIgnoreDisplayAttribute())
            {
                continue;
            }

            var item = new PropertyGridItem(propertyInfo, obj, attributes);

            // Extract nested properties if applicable
            if (ShouldExpandNestedObject(item, depth))
            {
                var nestedValue = item.Value;
                if (nestedValue is not null)
                {
                    var nestedItems = ExtractPropertyItems(nestedValue, depth + 1);
                    foreach (var nestedItem in nestedItems)
                    {
                        item.Children.Add(nestedItem);
                    }
                }
            }

            items.Add(item);
        }

        return SortItems(items);
    }

    private bool ShouldExpandNestedObject(
        PropertyGridItem item,
        int depth)
    {
        if (depth >= MaxNestedDepth)
        {
            return false;
        }

        var propertyType = item.PropertyType;

        // Skip primitives, enums, and common types
        if (propertyType.IsPrimitive ||
            propertyType.IsEnum ||
            propertyType == typeof(string) ||
            propertyType == typeof(decimal) ||
            propertyType == typeof(DateTime) ||
            propertyType == typeof(DateOnly) ||
            propertyType == typeof(TimeOnly) ||
            propertyType == typeof(TimeSpan) ||
            propertyType == typeof(Guid) ||
            propertyType == typeof(Color) ||
            propertyType == typeof(Thickness) ||
            typeof(Brush).IsAssignableFrom(propertyType) ||
            typeof(FileSystemInfo).IsAssignableFrom(propertyType))
        {
            return false;
        }

        // Skip collections
        if (propertyType.IsArray ||
            (typeof(System.Collections.IEnumerable).IsAssignableFrom(propertyType) &&
             propertyType != typeof(string)))
        {
            return false;
        }

        // Skip nullable value types of the above
        var underlyingType = Nullable.GetUnderlyingType(propertyType);
        if (underlyingType is not null &&
            (underlyingType.IsPrimitive ||
             underlyingType.IsEnum ||
             underlyingType == typeof(decimal) ||
             underlyingType == typeof(DateTime) ||
             underlyingType == typeof(DateOnly) ||
             underlyingType == typeof(TimeOnly) ||
             underlyingType == typeof(TimeSpan) ||
             underlyingType == typeof(Guid)))
        {
            // Create a temporary check with the underlying type
            return false;
        }

        return true;
    }

    private List<PropertyGridItem> SortItems(List<PropertyGridItem> items)
        => SortMode switch
        {
            PropertySortMode.Alphabetical => items.OrderBy(i => i.DisplayName, StringComparer.Ordinal).ToList(),
            PropertySortMode.NoSort => items,
            _ => items.OrderBy(i => i.DisplayName, StringComparer.Ordinal).ToList(),
        };

    private List<PropertyGridCategory> GroupByCategory(
        List<PropertyGridItem> items)
    {
        var categories = new Dictionary<string, PropertyGridCategory>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in items)
        {
            var categoryName = ShowCategories ? item.Category : DefaultCategoryName;

            if (!categories.TryGetValue(categoryName, out var category))
            {
                category = new PropertyGridCategory(categoryName);
                categories[categoryName] = category;
            }

            category.Properties.Add(item);
        }

        return SortMode == PropertySortMode.Categorized
            ? categories.Values.OrderBy(c => c.Name, StringComparer.Ordinal).ToList()
            : categories.Values.ToList();
    }

    private void UpdateDescription(PropertyGridItem item)
    {
        if (!ShowDescriptions)
        {
            return;
        }

        SelectedPropertyName.Text = item.DisplayName;
        SelectedPropertyDescription.Text = item.Description;
        DescriptionPanel.Visibility = !string.IsNullOrEmpty(item.Description)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }
}