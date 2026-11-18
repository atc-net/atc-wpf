namespace Atc.Wpf.Controls.DataGrids.Internal;

/// <summary>
/// The root of the header template that <see cref="DataGridFilter"/> wraps around a column's own header template:
/// the original header (with the original template) and the filter button at the right edge.
/// </summary>
internal sealed partial class DataGridFilterHeader : Grid
{
    private static readonly BooleanToVisibilityConverter BooleanToVisibilityConverterInstance = new();

    private readonly ContentPresenter presenter;

    /// <summary>
    /// Gets or sets the header content of the column.
    /// </summary>
    [DependencyProperty(GeneratePartialHooks = true)]
    private object? headerContent;

    /// <summary>
    /// Gets or sets the column's own header template.
    /// </summary>
    [DependencyProperty(GeneratePartialHooks = true)]
    private DataTemplate? headerTemplate;

    /// <summary>
    /// Gets or sets the column's own header template selector.
    /// </summary>
    [DependencyProperty(GeneratePartialHooks = true)]
    private DataTemplateSelector? headerTemplateSelector;

    private DataGridColumnHeader? columnHeader;
    private HorizontalAlignment? savedAlignment;

    public DataGridFilterHeader()
    {
        ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        presenter = new ContentPresenter { VerticalAlignment = VerticalAlignment.Center };
        Children.Add(presenter);

        Button = new DataGridFilterButton();
        SetColumn(Button, 1);
        Children.Add(Button);

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    partial void OnHeaderContentChanged(
        object? oldValue,
        object? newValue)
        => presenter.Content = newValue;

    partial void OnHeaderTemplateChanged(
        DataTemplate? oldValue,
        DataTemplate? newValue)
        => presenter.ContentTemplate = newValue;

    partial void OnHeaderTemplateSelectorChanged(
        DataTemplateSelector? oldValue,
        DataTemplateSelector? newValue)
        => presenter.ContentTemplateSelector = newValue;

    /// <summary>
    /// Gets the filter button.
    /// </summary>
    public DataGridFilterButton Button { get; }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        columnHeader = FindAncestor<DataGridColumnHeader>(this);
        var column = columnHeader?.Column;
        var grid = FindAncestor<DataGrid>(columnHeader);
        if (columnHeader is null || column is null || grid is null || !DataGridFilter.CanReadColumn(grid, column))
        {
            Button.Visibility = Visibility.Collapsed;
            return;
        }

        // The header shows its content left-aligned by default; stretch it so the button sits at the right edge,
        // and give the original header the alignment the column header had.
        if (savedAlignment is null && columnHeader.HorizontalContentAlignment != HorizontalAlignment.Stretch)
        {
            savedAlignment = columnHeader.HorizontalContentAlignment;
            columnHeader.SetCurrentValue(Control.HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch);
        }

        presenter.HorizontalAlignment = savedAlignment ?? HorizontalAlignment.Left;

        Button.Grid = grid;
        Button.Column = column;
        Button.UpdateName();
        Button.SetBinding(
            UIElement.VisibilityProperty,
            new Binding
            {
                Source = column,
                Path = new PropertyPath(DataGridFilter.IsColumnFilterEnabledProperty),
                Converter = BooleanToVisibilityConverterInstance,
            });
        Button.SetBinding(
            UIElement.IsEnabledProperty,
            new Binding
            {
                Source = column,
                Path = new PropertyPath(DataGridFilter.IsColumnFilterSupportedProperty),
            });
        Button.SetBinding(
            DataGridFilterButton.IsFilterActiveProperty,
            new Binding
            {
                Source = column,
                Path = new PropertyPath(DataGridFilter.IsColumnFilterActiveProperty),
            });
    }

    private void OnUnloaded(
        object sender,
        RoutedEventArgs e)
    {
        // When the filter is switched off the column gets its own template back: give the header its alignment back.
        if (columnHeader is not null &&
            savedAlignment is { } alignment &&
            columnHeader.Column is { } column &&
            !DataGridFilter.IsHeaderWrapped(column))
        {
            columnHeader.SetCurrentValue(Control.HorizontalContentAlignmentProperty, alignment);
            savedAlignment = null;
        }
    }

    private static T? FindAncestor<T>(DependencyObject? element)
        where T : DependencyObject
    {
        while (element is not null)
        {
            element = VisualTreeHelper.GetParent(element) ?? LogicalTreeHelper.GetParent(element);
            if (element is T match)
            {
                return match;
            }
        }

        return null;
    }
}