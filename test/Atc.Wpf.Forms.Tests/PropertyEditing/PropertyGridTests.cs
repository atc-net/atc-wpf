namespace Atc.Wpf.Forms.Tests.PropertyEditing;

public sealed class PropertyGridTests
{
    [StaFact]
    public void SelectedObject_DiscoversBrowsableReadableProperties()
    {
        // Arrange
        var sut = new PropertyGrid();

        // Act
        sut.SelectedObject = new Device();

        // Assert
        var names = AllItems(sut).Select(i => i.PropertyName).Order(StringComparer.Ordinal);
        Assert.Equal(
            [nameof(Device.Firmware), nameof(Device.Name), nameof(Device.Notes), nameof(Device.Port), nameof(Device.SerialNumber)],
            names,
            StringComparer.Ordinal);
    }

    [StaFact]
    public void SelectedObject_ReadsDisplayNameDescriptionAndValue()
    {
        // Arrange
        var sut = new PropertyGrid();

        // Act
        sut.SelectedObject = new Device { Port = 8080 };

        // Assert
        var port = Item(sut, nameof(Device.Port));
        Assert.Equal("TCP port", port.DisplayName);
        Assert.Equal("The port the device listens on.", port.Description);
        Assert.Equal(8080, port.Value);
        Assert.Equal(typeof(int), port.PropertyType);
    }

    [StaFact]
    public void SelectedObject_GroupsByCategory_SortedByName_WithMiscForUncategorized()
    {
        // Arrange
        var sut = new PropertyGrid();

        // Act
        sut.SelectedObject = new Device();

        // Assert
        Assert.Equal(["Connection", "Identity", "Misc"], sut.Categories.Select(c => c.Name), StringComparer.Ordinal);
        Assert.Equal(
            [nameof(Device.Port)],
            sut.Categories.Single(c => c.Name == "Connection").Properties.Select(p => p.PropertyName),
            StringComparer.Ordinal);
        Assert.Equal(
            [nameof(Device.Notes)],
            sut.Categories.Single(c => c.Name == "Misc").Properties.Select(p => p.PropertyName),
            StringComparer.Ordinal);
    }

    [StaFact]
    public void ShowCategoriesFalse_PutsEveryPropertyInTheDefaultCategory()
    {
        // Arrange
        var sut = new PropertyGrid
        {
            ShowCategories = false,
            DefaultCategoryName = "All",
        };

        // Act
        sut.SelectedObject = new Device();

        // Assert
        var category = Assert.Single(sut.Categories);
        Assert.Equal("All", category.Name);
        Assert.Equal(5, category.PropertyCount);
    }

    [StaFact]
    public void SortModeAlphabetical_OrdersPropertiesByDisplayName()
    {
        // Arrange
        var sut = new PropertyGrid
        {
            ShowCategories = false,
            SortMode = PropertySortMode.Alphabetical,
        };

        // Act
        sut.SelectedObject = new Device();

        // Assert
        var displayNames = sut.Categories.Single().Properties.Select(p => p.DisplayName).ToList();
        Assert.Equal(displayNames.Order(StringComparer.Ordinal), displayNames, StringComparer.Ordinal);
    }

    [StaFact]
    public void GetterOnlyAndReadOnlyAttributeProperties_AreReadOnly()
    {
        // Arrange
        var sut = new PropertyGrid();

        // Act
        sut.SelectedObject = new Device();

        // Assert
        Assert.True(Item(sut, nameof(Device.SerialNumber)).IsReadOnly);
        Assert.True(Item(sut, nameof(Device.Firmware)).IsReadOnly);
        Assert.False(Item(sut, nameof(Device.Name)).IsReadOnly);
    }

    [StaFact]
    public void ReadOnlyProperty_SettingValue_DoesNotWriteToTheObject()
    {
        // Arrange
        var device = new Device { Firmware = "1.0" };
        var sut = new PropertyGrid { SelectedObject = device };

        // Act
        Item(sut, nameof(Device.Firmware)).Value = "2.0";

        // Assert
        Assert.Equal("1.0", device.Firmware);
    }

    [StaFact]
    public void EditingAValue_WritesItBackToTheObject()
    {
        // Arrange
        var device = new Device { Name = "Old" };
        var sut = new PropertyGrid { SelectedObject = device };
        var item = Item(sut, nameof(Device.Name));
        var raised = new List<string?>();
        item.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

        // Act
        item.Value = "New";

        // Assert
        Assert.Equal("New", device.Name);
        Assert.Equal([nameof(PropertyGridItem.Value)], raised, StringComparer.Ordinal);
    }

    [StaFact]
    public void EditingAValue_SetterThrows_RevertsTheItem()
    {
        // Arrange
        var device = new Device { Port = 80 };
        var sut = new PropertyGrid { SelectedObject = device };
        var item = Item(sut, nameof(Device.Port));

        // Act
        item.Value = -1;

        // Assert
        Assert.Equal(80, device.Port);
        Assert.Equal(80, item.Value);
    }

    [StaFact]
    public void ObjectRaisesPropertyChanged_RefreshesTheItem()
    {
        // Arrange
        var device = new Device { Name = "Old" };
        var sut = new PropertyGrid { SelectedObject = device };

        // Act
        device.Name = "Changed outside";

        // Assert
        Assert.Equal("Changed outside", Item(sut, nameof(Device.Name)).Value);
    }

    [StaFact]
    public void ReplacingTheSelectedObject_StopsListeningToTheOldOne()
    {
        // Arrange
        var oldDevice = new Device { Name = "Old" };
        var sut = new PropertyGrid { SelectedObject = oldDevice };

        // Act
        sut.SelectedObject = new Device { Name = "New" };
        oldDevice.Name = "Changed outside";

        // Assert
        Assert.Equal("New", Item(sut, nameof(Device.Name)).Value);
    }

    [StaFact]
    public void SelectedObjectNull_ShowsNothing()
    {
        // Arrange
        var sut = new PropertyGrid();

        // Act
        sut.SelectedObject = null;
        sut.Refresh();

        // Assert
        Assert.Empty(sut.Categories);
    }

    [StaFact]
    public void SelectedObjectSetBackToNull_ClearsTheGrid()
    {
        // Arrange
        var sut = new PropertyGrid { SelectedObject = new Device() };

        // Act
        sut.SelectedObject = null;

        // Assert
        Assert.Empty(sut.Categories);
    }

    [StaFact]
    public void IsReadOnly_DisablesEveryEditor()
    {
        // Arrange
        var sut = new PropertyGrid();

        // Act
        sut.IsReadOnly = true;
        sut.SelectedObject = new Device();

        // Assert
        var editors = FindDescendants<Control>(sut).Where(c => c is TextBox or ComboBox or CheckBox).ToList();
        Assert.NotEmpty(editors);
        Assert.All(editors, e => Assert.False(e.IsEnabled));
    }

    [StaFact]
    public void NestedObject_IsExpandedIntoChildren()
    {
        // Arrange
        var sut = new PropertyGrid();

        // Act
        sut.SelectedObject = new Rack { Device = new Device { Name = "Child" } };

        // Assert
        var device = Item(sut, nameof(Rack.Device));
        Assert.True(device.HasChildren);
        Assert.Equal("Child", device.Children.Single(c => c.PropertyName == nameof(Device.Name)).Value);
    }

    [StaFact]
    public void MaxNestedDepthZero_DoesNotExpandNestedObjects()
    {
        // Arrange
        var sut = new PropertyGrid { MaxNestedDepth = 0 };

        // Act
        sut.SelectedObject = new Rack { Device = new Device() };

        // Assert
        Assert.False(Item(sut, nameof(Rack.Device)).HasChildren);
    }

    [StaFact]
    public void ObjectWithAnIndexer_SkipsTheIndexer()
    {
        // Arrange
        var sut = new PropertyGrid();

        // Act
        sut.SelectedObject = new Lookup();

        // Assert
        var item = Assert.Single(AllItems(sut));
        Assert.Equal(nameof(Lookup.Title), item.PropertyName);
    }

    [StaTheory]
    [InlineData(nameof(Counters.Big), 5_000_000_000d, 5_000_000_000L)]
    [InlineData(nameof(Counters.Big), -5_000_000_000d, -5_000_000_000L)]
    [InlineData(nameof(Counters.Huge), 10_000_000_000_000_000_000d, 10_000_000_000_000_000_000UL)]
    [InlineData(nameof(Counters.Huge), -5d, 0UL)]
    [InlineData(nameof(Counters.Unsigned), 4_000_000_000d, 4_000_000_000U)]
    [InlineData(nameof(Counters.Unsigned), -5d, 0U)]
    [InlineData(nameof(Counters.Ranged), -5d, 0U)]
    [InlineData(nameof(Counters.Ranged), 70_000d, 50_000U)]
    [InlineData(nameof(Counters.Small), 300d, (byte)255)]
    public void IntegerPropertyEditor_WritesTheValueInTheRangeOfThePropertyType(
        string propertyName,
        double editorValue,
        object expected)
    {
        // Arrange
        var counters = new Counters();
        var propertyInfo = typeof(Counters).GetProperty(propertyName)!;
        var item = new PropertyGridItem(propertyInfo, counters, propertyInfo.GetCustomAttributes().ToList());
        var sut = new Atc.Wpf.Forms.PropertyEditing.Editors.IntegerPropertyEditor();
        var editor = (Atc.Wpf.Controls.Inputs.IntegerBox)sut.CreateEditor(item);
        sut.BindToProperty(editor, item);

        // Act
        editor.Value = editorValue;

        // Assert
        Assert.Equal(expected, item.Value);
    }

    private static IEnumerable<PropertyGridItem> AllItems(PropertyGrid grid)
        => grid.Categories.SelectMany(c => c.Properties);

    private static PropertyGridItem Item(
        PropertyGrid grid,
        string propertyName)
        => AllItems(grid).Single(i => i.PropertyName == propertyName);

    private static IEnumerable<T> FindDescendants<T>(DependencyObject root)
        where T : DependencyObject
    {
        foreach (var child in LogicalTreeHelper.GetChildren(root).OfType<DependencyObject>())
        {
            if (child is T match)
            {
                yield return match;
            }

            foreach (var descendant in FindDescendants<T>(child))
            {
                yield return descendant;
            }
        }
    }

    private sealed class Device : INotifyPropertyChanged
    {
        private string name = string.Empty;
        private int port;

        public event PropertyChangedEventHandler? PropertyChanged;

        [Category("Identity")]
        public string Name
        {
            get => name;
            set
            {
                name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        [Category("Connection")]
        [DisplayName("TCP port")]
        [Description("The port the device listens on.")]
        public int Port
        {
            get => port;
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegative(value);
                port = value;
            }
        }

        [Category("Identity")]
        public string SerialNumber => "SN-1";

        [Category("Identity")]
        [ReadOnly(true)]
        public string Firmware { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        [Browsable(false)]
        public string Hidden { get; set; } = string.Empty;

        [SuppressMessage("Major Code Smell", "S2376:Write-only properties should not be used", Justification = "Tests that write-only properties are skipped.")]
        [SuppressMessage("Design", "CA1044:Properties should not be write only", Justification = "Tests that write-only properties are skipped.")]
        public string Secret
        {
            set => _ = value;
        }
    }

    private sealed class Counters
    {
        public long Big { get; set; }

        public ulong Huge { get; set; }

        public uint Unsigned { get; set; }

        [Range(-10, 50_000)]
        public uint Ranged { get; set; }

        public byte Small { get; set; }
    }

    private sealed class Rack
    {
        public Device Device { get; set; } = new();
    }

    private sealed class Lookup
    {
        public string Title { get; set; } = "Lookup";

        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Indexers cannot be static.")]
        public string this[int index] => index.ToString(CultureInfo.InvariantCulture);
    }
}