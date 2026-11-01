#pragma warning disable IDE0130
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// The first column of the <see cref="MultiColumnComboBox"/> pop-up grid: a check box per row (bound to the row's
/// <see cref="MultiColumnRowState"/>) and a tri-state "select all shown" check box as header.
/// </summary>
internal sealed class MultiColumnCheckColumn : DataGridColumn
{
    private readonly Action<MultiColumnCheckBox> toggleRow;

    public MultiColumnCheckColumn(
        MultiColumnCheckBox header,
        Action<MultiColumnCheckBox> toggleRow)
    {
        this.toggleRow = toggleRow;
        Header = header;
        CanUserSort = false;
        CanUserResize = false;
        CanUserReorder = false;
        IsReadOnly = true;
        Width = new DataGridLength(32);
    }

    /// <inheritdoc />
    protected override FrameworkElement GenerateElement(
        DataGridCell cell,
        object dataItem)
    {
        // Not hit-testable: a press anywhere on the row (also on the box) toggles it once, through the row.
        var checkBox = new MultiColumnCheckBox(toggleRow)
        {
            IsRowCheckBox = true,
            IsHitTestVisible = false,
            Focusable = false,
            IsTabStop = false,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };

        BindToRow(checkBox, ToggleButton.IsCheckedProperty, MultiColumnRowState.IsCheckedProperty);
        BindToRow(checkBox, UIElement.IsEnabledProperty, MultiColumnRowState.CanToggleProperty);
        BindToRow(checkBox, AutomationProperties.NameProperty, MultiColumnRowState.DisplayTextProperty);
        return checkBox;
    }

    /// <inheritdoc />
    protected override FrameworkElement GenerateEditingElement(
        DataGridCell cell,
        object dataItem)
        => GenerateElement(cell, dataItem);

    private static void BindToRow(
        DependencyObject target,
        DependencyProperty targetProperty,
        DependencyProperty rowProperty)
        => BindingOperations.SetBinding(
            target,
            targetProperty,
            new Binding
            {
                Path = new PropertyPath(rowProperty),
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(DataGridRow), 1),
                Mode = BindingMode.OneWay,
            });
}