# 🔽 DataGridFilter

Excel-style column filters, a search box, a column chooser and layout persistence for the stock WPF `DataGrid`, switched on with attached properties.

## 🔍 Overview

`DataGridFilter` is a set of opt-in attached behaviours, not a new grid: an existing `DataGrid` gains filtering by setting one property, and keeps its own columns, styles and bindings.

- **Column filter drop-down**: `DataGridFilter.IsEnabled="True"` adds a funnel button to each column header. It opens a pop-up with a `CheckListBox` of the column's **distinct values** (with filter box, tri-state **Select all** and one **(Blanks)** entry), plus OK, Cancel and **Clear filter**.
  - The values are formatted as the cells show them (the binding's `Converter`, `StringFormat` and the grid's `Language`) and sorted by their raw value, so numbers and dates sort naturally. Blanks (`null` or empty) come last.
  - The values reflect the rows that pass the **other** columns' filters, the search and your own view filter (Excel behaviour).
  - **Text filter** mode: *Contains*, *Equals* or *Starts with* (case-insensitive) on the formatted text.
  - **Range** mode for number and date columns: an inclusive minimum/maximum (dates compare whole days).
  - A column with an active filter shows a filled accent funnel; `DataGridFilter.IsColumnFilterActive` is set on the column for your own header styles.
- **Search**: `DataGridFilter.SearchText` searches all visible columns (contains, case-insensitive), debounced by `SearchDelay`, and ANDs with the column filters. Bind it to any `TextBox`.
- **Column chooser**: `DataGridFilter.ShowColumnChooser="True"` shows a menu of the columns with check boxes when a column header is right-clicked. `DataGridFilter.OpenColumnChooser(grid)` opens it from a toolbar button. A column with `CanUserHideColumn="False"` cannot be hidden, nor can the last shown column.
- **Layout persistence**: `SaveLayout(grid)` / `LoadLayout(grid, json)` save and restore the column order, widths, visibility, sort descriptions and active filters as versioned JSON.
- **Events and API**: the `FilterChanged` attached routed event, `GetColumnFilters`, `SetColumnFilter`, `ClearColumnFilter`, `ClearAllFilters`, `OpenColumnFilter`.

Filters combine with **AND** across columns. They are applied to the default collection view of `ItemsSource` (`CollectionViewSource.GetDefaultView`) with **one** `Refresh` per change. A filter you already set on that view is kept and ANDed, not overwritten.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.DataGrids;
```

In XAML the `atc:` prefix (`https://github.com/atc-net/atc-wpf/tree/main/schemas`) includes the namespace.

## 🚀 Usage

### Column filters, search and column chooser

```xml
<TextBox
    x:Name="SearchBox"
    atc:TextBoxHelper.Watermark="Search all columns" />

<DataGrid
    atc:DataGridFilter.IsEnabled="True"
    atc:DataGridFilter.SearchText="{Binding ElementName=SearchBox, Path=Text}"
    atc:DataGridFilter.ShowColumnChooser="True"
    AutoGenerateColumns="False"
    ItemsSource="{Binding Path=Orders}">
    <DataGrid.Columns>
        <DataGridTextColumn
            atc:DataGridFilter.CanUserHideColumn="False"
            atc:DataGridFilter.ColumnKey="id"
            atc:DataGridFilter.IsColumnFilterEnabled="False"
            Binding="{Binding Id}"
            Header="Id" />
        <DataGridTextColumn Binding="{Binding Customer}" Header="Customer" />
        <DataGridTextColumn Binding="{Binding Status}" Header="Status" />
        <DataGridTextColumn Binding="{Binding Amount, StringFormat=N2}" Header="Amount" />
        <DataGridTextColumn Binding="{Binding Date, StringFormat=yyyy-MM-dd}" Header="Date" />
    </DataGrid.Columns>
</DataGrid>
```

### Filters from code

```csharp
DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForValues("Status", ["Open", "Pending"]));
DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForNumberRange("Amount", minimum: 100, maximum: null));
DataGridFilter.SetColumnFilter(grid, DataGridColumnFilter.ForText("Customer", DataGridTextFilterOperator.StartsWith, "con"));

IReadOnlyList<DataGridColumnFilter> active = DataGridFilter.GetColumnFilters(grid);
DataGridFilter.ClearAllFilters(grid);
```

A values filter lists the **formatted** texts that pass; an empty string stands for blank cells.

### Save and restore the layout

```csharp
string json = DataGridFilter.SaveLayout(grid);   // store it in your settings
DataGridFilter.LoadLayout(grid, json);           // returns false for unreadable JSON
```

```json
{
  "version": 1,
  "columns": [ { "key": "Status", "displayIndex": 0, "width": 110, "widthUnit": "Pixel", "isVisible": true } ],
  "sorts": [ { "propertyName": "Amount", "direction": "Descending" } ],
  "filters": [ { "columnKey": "Status", "kind": "Values", "values": [ "Open", "Pending" ] } ]
}
```

Columns and filters for keys the grid does not have are ignored; columns the layout does not list keep their state. `GetLayout` / `ApplyLayout` work with the `DataGridLayout` object instead of JSON.

### FilterChanged

```xml
<DataGrid atc:DataGridFilter.FilterChanged="OnFilterChanged" ... />
```

```csharp
private void OnFilterChanged(object sender, DataGridFilterChangedEventArgs e)
    => Status = $"{e.ItemCount} rows, {e.Filters.Count} column filters, search '{e.SearchText}'";
```

## ⚙️ Properties

### On the DataGrid

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IsEnabled` | `bool` | `false` | Adds a filter button to each column header |
| `SearchText` | `string` | `""` | Text searched for in all visible columns (contains, case-insensitive) |
| `SearchDelay` | `TimeSpan` | `300 ms` | Wait after the last `SearchText` change; zero searches at once |
| `ShowColumnChooser` | `bool` | `false` | Right-clicking a column header shows the column chooser |

### On a column

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IsColumnFilterEnabled` | `bool` | `true` | `false` removes the column's filter button |
| `ColumnKey` | `string?` | `null` | Stable key for filters and saved layouts |
| `CanUserHideColumn` | `bool` | `true` | `false` keeps the column chooser from hiding the column |
| `IsColumnFilterActive` | `bool` (read-only) | `false` | `true` while the column has an active filter |

The column key is the `ColumnKey`, else the `SortMemberPath`, else the binding path, else the header text.

## 📡 Events

| Event | Type | Description |
|-------|------|-------------|
| `FilterChanged` | attached routed event (bubbling), `DataGridFilterChangedEventArgs` | Raised once after a column filter, the search or a loaded layout was applied. Carries `Filters`, `SearchText` and `ItemCount` (rows that pass) |

## 🧰 Methods

| Method | Description |
|--------|-------------|
| `GetColumnFilters(grid)` | The active column filters |
| `SetColumnFilter(grid, filter)` | Sets (or, for a filter that restricts nothing, removes) a column's filter |
| `ClearColumnFilter(grid, column)` / `ClearAllFilters(grid)` | Removes one or all column filters (the search and your own view filter stay) |
| `OpenColumnFilter(grid, column)` | Opens a column's filter pop-up |
| `OpenColumnChooser(grid)` | Opens the column chooser at the grid's top-left corner |
| `SaveLayout(grid)` / `LoadLayout(grid, json)` | Saves and restores the layout as JSON |
| `GetLayout(grid)` / `ApplyLayout(grid, layout)` | The same with a `DataGridLayout` object |
| `ResolveColumnKey(column)` | The key a column is saved and filtered under |

## ⌨️ Keyboard and UI Automation

| Key | Action |
|-----|--------|
| `Alt+Down` on a filter button | Opens that column's filter |
| `Alt+Down` on a cell | Opens the filter of the current column (stock column headers cannot take the keyboard focus). An editing cell's own control (e.g. a combo box) keeps `Alt+Down` |
| `Enter` in the pop-up | OK |
| `Esc` in the pop-up | Cancel |

- The filter button is a UI Automation **button** (Invoke) named "Filter {column}", with class name `DataGridFilterButton`.
- While its pop-up is open, the pop-up content (mode buttons, the value rows as check boxes with `Toggle`, Clear filter, OK, Cancel; AutomationIds `FilterValues`, `ClearFilter`, `Ok`, `Cancel`) is a child of the button, because the pop-up is a separate window.
- Closing the pop-up gives the focus back to the filter button.

## 📝 Notes

- **Shared view**: grids that share one `ItemsSource` share its default view, so their filters combine (AND) with each other and with the view's own filter. Give each grid its own `CollectionViewSource` (or `ListCollectionView`) when they must filter independently.
- **Your own filter**: a `Filter` already set on the default view is kept and ANDed. If you replace the view's `Filter` after a column filter was applied, your new filter is adopted (and ANDed) on the next filter change. Switching `IsEnabled` off gives the view your filter back.
- **ItemsSource replacement**: the active filters are applied to the new source; the old view gets its own filter back.
- **Header templates**: the button is added by wrapping the column's `HeaderTemplate` (kept and restored when `IsEnabled` is switched off). Set your own `HeaderTemplate` before switching `IsEnabled` on. The header's content is stretched so the button sits at the right edge.
- **Columns without a path**: template columns need a `SortMemberPath` to be filterable; otherwise they get no button. Indexer paths and `DataTable` sources (`BindingListCollectionView`, which cannot filter with a predicate) are not supported.
- **Values filters store the formatted text**, so a saved values filter depends on the column's `StringFormat` and the grid's `Language`. Range and text filters do not.
- **Performance** (100,000 rows, Release build): the distinct values of a column take about 20-40 ms; the pop-up opens in about 0.1 s for a column with 5 values and about 0.3 s for one with 100,000 distinct values; OK applies the filter in about 0.1 s including the grid's re-layout (one `Refresh`); the search over six columns takes about 50 ms. Row and column virtualization stay on.
- **Leaks**: the predicate on the (possibly long-lived) view holds the grid weakly; a dropped grid leaves only your own filter on the view.
- Theme brushes only: the button and pop-up follow the Light and Dark themes. The funnel takes the header's foreground (accent when active). The pop-up is a separate window, so its brushes come from the application resources, where the Atc theme is merged (`App.xaml`). Texts are translated (English, Danish, German).

## 🔗 Related Controls

- **CheckListBox** - the value list of the filter pop-up
- **MultiColumnComboBox** - a drop-down with a filterable, sortable grid of rows to pick from
- **DualListSelector** - move items between an available and a selected list

## 🎮 Sample Application

See the DataGridFilter sample in the sample application under **Wpf.Controls > DataGrids > DataGridFilter**.
