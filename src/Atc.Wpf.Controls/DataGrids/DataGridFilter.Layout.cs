namespace Atc.Wpf.Controls.DataGrids;

/// <summary>
/// Saving and restoring the column layout and filters of a <see cref="DataGrid"/>.
/// </summary>
public static partial class DataGridFilter
{
    private static readonly object LayoutWorkKey = new();

    private static readonly JsonSerializerOptions LayoutWriteOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() },
    };

    private static readonly JsonSerializerOptions LayoutReadOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() },
    };

    /// <summary>
    /// Captures the column order, widths, visibility, sort descriptions and active column filters of a grid.
    /// </summary>
    /// <param name="grid">The grid.</param>
    /// <returns>The layout.</returns>
    public static DataGridLayout GetLayout(DataGrid grid)
    {
        ArgumentNullException.ThrowIfNull(grid);

        var layout = new DataGridLayout { Culture = GetGridCulture(grid).Name };
        foreach (var column in grid.Columns)
        {
            var key = ResolveColumnKey(column);
            if (key.Length == 0)
            {
                continue;
            }

            layout.Columns.Add(new DataGridColumnLayout
            {
                Key = key,
                DisplayIndex = column.DisplayIndex,
                Width = double.IsFinite(column.Width.Value) ? column.Width.Value : 0,
                WidthUnit = column.Width.UnitType,
                IsVisible = column.Visibility == Visibility.Visible,
            });
        }

        foreach (var sort in grid.Items.SortDescriptions)
        {
            layout.Sorts.Add(new DataGridSortLayout { PropertyName = sort.PropertyName, Direction = sort.Direction });
        }

        layout.Filters.AddRange(GetColumnFilters(grid));
        return layout;
    }

    /// <summary>
    /// Restores a layout captured by <see cref="GetLayout"/>: columns are matched by key and columns the layout
    /// does not know (or keys the grid does not have) are ignored. Sorting and filters are applied in one refresh.
    /// </summary>
    /// <remarks>
    /// Values filters list formatted texts: when the layout's <see cref="DataGridLayout.Culture"/> differs from the
    /// grid's, each text is translated to the grid's culture through the rows the grid has when the layout is
    /// applied (a text no row formats to stays as it is).
    /// </remarks>
    /// <param name="grid">The grid.</param>
    /// <param name="layout">The layout.</param>
    public static void ApplyLayout(
        DataGrid grid,
        DataGridLayout layout)
    {
        ArgumentNullException.ThrowIfNull(grid);
        ArgumentNullException.ThrowIfNull(layout);

        var columnsByKey = new Dictionary<string, DataGridColumn>(StringComparer.Ordinal);
        foreach (var column in grid.Columns)
        {
            columnsByKey.TryAdd(ResolveColumnKey(column), column);
        }

        var known = (layout.Columns ?? [])
            .Where(x => x.Key is not null && columnsByKey.ContainsKey(x.Key))
            .GroupBy(x => x.Key, StringComparer.Ordinal)
            .Select(x => x.First())
            .ToList();

        ApplyColumns(grid, columnsByKey, known);

        var state = EnsureState(grid);
        var sorts = (layout.Sorts ?? [])
            .Where(x => !string.IsNullOrEmpty(x.PropertyName))
            .ToList();
        foreach (var column in grid.Columns)
        {
            var sort = sorts.Find(x => string.Equals(x.PropertyName, column.SortMemberPath, StringComparison.Ordinal));
            column.SortDirection = sort?.Direction;
        }

        var savedCulture = GetLayoutCulture(grid, layout.Culture);
        state.ReplaceFilters(
            (layout.Filters ?? [])
                .Where(x => x is not null && !string.IsNullOrEmpty(x.ColumnKey) && columnsByKey.ContainsKey(x.ColumnKey))
                .Select(x => savedCulture is null
                    ? x
                    : state.TranslateValues(columnsByKey[x.ColumnKey], x, savedCulture))
                .ToList());

        // Sorting and filtering are refused during an edit or add transaction: they wait for it to end.
        state.RunViewWork(LayoutWorkKey, () => ApplySortsAndFilters(grid, state, sorts));
    }

    /// <summary>
    /// Returns the culture a layout was saved in when it differs from the grid's, else <see langword="null"/>.
    /// </summary>
    private static CultureInfo? GetLayoutCulture(
        DataGrid grid,
        string? name)
    {
        if (string.IsNullOrEmpty(name) ||
            string.Equals(name, GetGridCulture(grid).Name, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        try
        {
            return CultureInfo.GetCultureInfo(name);
        }
        catch (CultureNotFoundException)
        {
            return null;
        }
    }

    private static void ApplySortsAndFilters(
        DataGrid grid,
        DataGridFilterState state,
        List<DataGridSortLayout> sorts)
    {
        var view = grid.ItemsSource is null ? null : CollectionViewSource.GetDefaultView(grid.ItemsSource);
        var deferral = view?.DeferRefresh();
        try
        {
            if (view is { CanSort: true })
            {
                view.SortDescriptions.Clear();
                foreach (var sort in sorts)
                {
                    view.SortDescriptions.Add(new SortDescription(sort.PropertyName, sort.Direction));
                }
            }

            state.ApplyToDeferredView();
        }
        finally
        {
            deferral?.Dispose();
        }

        state.RaiseFilterChanged();
    }

    /// <summary>
    /// Saves the layout of a grid (see <see cref="GetLayout"/>) as versioned JSON.
    /// </summary>
    /// <param name="grid">The grid.</param>
    /// <returns>The JSON.</returns>
    public static string SaveLayout(DataGrid grid)
        => JsonSerializer.Serialize(GetLayout(grid), LayoutWriteOptions);

    /// <summary>
    /// Restores a layout saved by <see cref="SaveLayout"/> (see <see cref="ApplyLayout"/>).
    /// Returns <see langword="false"/> and changes nothing when the JSON cannot be read.
    /// </summary>
    /// <param name="grid">The grid.</param>
    /// <param name="json">The JSON.</param>
    /// <returns><see langword="true"/> when the layout was applied.</returns>
    public static bool LoadLayout(
        DataGrid grid,
        string json)
    {
        ArgumentNullException.ThrowIfNull(grid);
        if (string.IsNullOrWhiteSpace(json))
        {
            return false;
        }

        DataGridLayout? layout;
        try
        {
            layout = JsonSerializer.Deserialize<DataGridLayout>(json, LayoutReadOptions);
        }
        catch (JsonException)
        {
            return false;
        }

        if (layout is null)
        {
            return false;
        }

        ApplyLayout(grid, layout);
        return true;
    }

    private static void ApplyColumns(
        DataGrid grid,
        Dictionary<string, DataGridColumn> columnsByKey,
        List<DataGridColumnLayout> known)
    {
        foreach (var columnLayout in known)
        {
            var column = columnsByKey[columnLayout.Key];
            column.Visibility = columnLayout.IsVisible ? Visibility.Visible : Visibility.Collapsed;
            if (ToLength(columnLayout) is { } width)
            {
                column.Width = width;
            }
        }

        if (grid.Columns.Count > 0)
        {
            foreach (var columnLayout in known.OrderBy(x => x.DisplayIndex))
            {
                columnsByKey[columnLayout.Key].DisplayIndex = System.Math.Clamp(columnLayout.DisplayIndex, 0, grid.Columns.Count - 1);
            }
        }
    }

    private static DataGridLength? ToLength(DataGridColumnLayout columnLayout)
        => columnLayout.WidthUnit switch
        {
            DataGridLengthUnitType.Auto => DataGridLength.Auto,
            DataGridLengthUnitType.SizeToCells => DataGridLength.SizeToCells,
            DataGridLengthUnitType.SizeToHeader => DataGridLength.SizeToHeader,
            DataGridLengthUnitType.Pixel or DataGridLengthUnitType.Star
                when double.IsFinite(columnLayout.Width) && columnLayout.Width > 0
                => new DataGridLength(columnLayout.Width, columnLayout.WidthUnit),
            _ => null,
        };
}