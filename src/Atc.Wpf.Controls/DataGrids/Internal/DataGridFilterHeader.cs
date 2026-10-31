namespace Atc.Wpf.Controls.DataGrids.Internal;

/// <summary>
/// The root of the header template that <see cref="DataGridFilter"/> wraps around a column's own header template:
/// the original header (with the original template) and the filter button at the right edge.
/// </summary>
internal sealed class DataGridFilterHeader : Grid
{
    /// <summary>
    /// Identifies the <see cref="HeaderContent"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HeaderContentProperty = DependencyProperty.Register(
        nameof(HeaderContent),
        typeof(object),
        typeof(DataGridFilterHeader),
        new PropertyMetadata(defaultValue: null, static (d, e) => ((DataGridFilterHeader)d).presenter.Content = e.NewValue));

    /// <summary>
    /// Identifies the <see cref="HeaderTemplate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(
        nameof(HeaderTemplate),
        typeof(DataTemplate),
        typeof(DataGridFilterHeader),
        new PropertyMetadata(defaultValue: null, static (d, e) => ((DataGridFilterHeader)d).presenter.ContentTemplate = (DataTemplate?)e.NewValue));

    /// <summary>
    /// Identifies the <see cref="HeaderTemplateSelector"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HeaderTemplateSelectorProperty = DependencyProperty.Register(
        nameof(HeaderTemplateSelector),
        typeof(DataTemplateSelector),
        typeof(DataGridFilterHeader),
        new PropertyMetadata(defaultValue: null, static (d, e) => ((DataGridFilterHeader)d).presenter.ContentTemplateSelector = (DataTemplateSelector?)e.NewValue));

    private static readonly BooleanToVisibilityConverter BooleanToVisibilityConverterInstance = new();

    private readonly ContentPresenter presenter;
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

    /// <summary>
    /// Gets or sets the header content of the column.
    /// </summary>
    public object? HeaderContent
    {
        get => GetValue(HeaderContentProperty);
        set => SetValue(HeaderContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the column's own header template.
    /// </summary>
    public DataTemplate? HeaderTemplate
    {
        get => (DataTemplate?)GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets the column's own header template selector.
    /// </summary>
    public DataTemplateSelector? HeaderTemplateSelector
    {
        get => (DataTemplateSelector?)GetValue(HeaderTemplateSelectorProperty);
        set => SetValue(HeaderTemplateSelectorProperty, value);
    }

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