// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Wpf.Controls.LabelControls;

public sealed class LabelControlsForm : ILabelControlsForm
{
    private const int GroupBoxWidthForSpace = 20;
    private const int GroupBoxWidthForMargin = 10;

    public IList<ILabelControlsFormRow>? Rows { get; set; }

    public void AddColumn(
        IList<ILabelControlBase> labelControls)
    {
        ArgumentNullException.ThrowIfNull(labelControls);

        var labelControlsFormColumn = new LabelControlsFormColumn(labelControls);

        AddColumn(labelControlsFormColumn);
    }

    public void AddColumn(
        ILabelControlsFormColumn labelControlsFormColumn)
    {
        ArgumentNullException.ThrowIfNull(labelControlsFormColumn);

        Rows ??= new List<ILabelControlsFormRow>();

        if (labelControlsFormColumn.HasMultiGroupIdentifiers())
        {
            var groupIdentifiers = labelControlsFormColumn.GetGroupIdentifiers();

            var row = new LabelControlsFormRow
            {
                Columns = new List<ILabelControlsFormColumn>(),
            };

            foreach (var groupIdentifier in groupIdentifiers)
            {
                var labelControls = labelControlsFormColumn.GetLabelControlsByGroupIdentifier(groupIdentifier);
                row.Columns.Add(new LabelControlsFormColumn(labelControls));
            }

            Rows.Add(row);
        }
        else
        {
            if (Rows.Count > 0)
            {
                Rows[0].Columns ??= new List<ILabelControlsFormColumn>();
                Rows[0].Columns!.Add(labelControlsFormColumn);
            }
            else
            {
                Rows.Add(new LabelControlsFormRow(labelControlsFormColumn));
            }
        }
    }

    public void Clear()
    {
        Rows = new List<ILabelControlsFormRow>();
    }

    public bool HasMultiGroupIdentifiers()
    {
        if (Rows is null)
        {
            return false;
        }

        foreach (var row in Rows)
        {
            if (row.Columns is null)
            {
                continue;
            }

            var uniqGroupIdentifier = new List<string?>();
            foreach (var column in row.Columns)
            {
                foreach (var groupIdentifier in column.GetGroupIdentifiers())
                {
                    if (!uniqGroupIdentifier.Contains(groupIdentifier, StringComparer.Ordinal))
                    {
                        uniqGroupIdentifier.Add(groupIdentifier);
                    }
                }
            }

            if (uniqGroupIdentifier.Count > 1)
            {
                return true;
            }
        }

        return false;
    }

    public int GetMaxHeight()
    {
        if (Rows is null)
        {
            return 0;
        }

        var rowHeight = 0;
        foreach (var row in Rows)
        {
            if (row.Columns is null)
            {
                continue;
            }

            rowHeight += row.Columns
                .Select(x => x.CalculateHeight())
                .Prepend(0)
                .Max();
        }

        return rowHeight;
    }

    public int GetMaxWidth()
    {
        if (Rows is null)
        {
            return 0;
        }

        var rowMaxWidth = 0;
        foreach (var row in Rows)
        {
            if (row.Columns is null)
            {
                continue;
            }

            var controlsWidth = 0;
            var marginOffset = 0;
            if (row.Columns.Count > 0)
            {
                var numberOfColumnsWithGroupBox = 0;
                foreach (var column in row.Columns)
                {
                    if (column.UseGroupBox)
                    {
                        numberOfColumnsWithGroupBox++;
                    }

                    controlsWidth = column.ControlWidth;
                }

                if (numberOfColumnsWithGroupBox > 0)
                {
                    marginOffset += numberOfColumnsWithGroupBox * (GroupBoxWidthForSpace + GroupBoxWidthForMargin);
                }
            }

            var columnMaxWidth = (row.Columns.Count * controlsWidth) + marginOffset;
            if (columnMaxWidth > rowMaxWidth)
            {
                rowMaxWidth = columnMaxWidth;
            }
        }

        return rowMaxWidth;
    }

    public Panel GeneratePanel()
    {
        var stackPanelRoot = new StackPanel
        {
            Orientation = Orientation.Vertical,
        };

        if (Rows is null)
        {
            return stackPanelRoot;
        }

        foreach (var row in Rows)
        {
            if (row.Columns is null)
            {
                continue;
            }

            var stackPanelRow = new StackPanel
            {
                Orientation = Orientation.Horizontal,
            };

            foreach (var column in row.Columns)
            {
                stackPanelRow.Children.Add(column.GeneratePanel());
            }

            stackPanelRoot.Children.Add(stackPanelRow);
        }

        return stackPanelRoot;
    }

    public bool IsValid()
    {
        if (Rows is null)
        {
            return true;
        }

        var isAllValid = true;
        foreach (var row in Rows)
        {
            if (row.Columns is null)
            {
                continue;
            }

            foreach (var column in row.Columns)
            {
                if (!column.IsValid())
                {
                    isAllValid = false;
                }
            }
        }

        return isAllValid;
    }

    public Dictionary<string, object> GetKeyValues()
    {
        if (Rows is null)
        {
            return new Dictionary<string, object>(StringComparer.Ordinal);
        }

        var result = new Dictionary<string, object>(StringComparer.Ordinal);
        foreach (var row in Rows)
        {
            if (row.Columns is null)
            {
                continue;
            }

            foreach (var column in row.Columns)
            {
                foreach (var item in column.GetKeyValues())
                {
                    result.Add(item.Key, item.Value);
                }
            }
        }

        return result;
    }

    public override string ToString()
    {
        if (Rows is null)
        {
            return "No LabelControls";
        }

        var rowLineInfo = new List<string>();
        for (var i = 0; i < Rows.Count; i++)
        {
            var row = Rows[i];
            var totalRowHeight = row.Columns?.Sum(x => x.CalculateHeight()) ?? 0;
            rowLineInfo.Add($"Row {i}: ColCount={row.Columns?.Count ?? 0}, TotalRowHeight={totalRowHeight}");
        }

        return string.Join(" # ", rowLineInfo);
    }
}