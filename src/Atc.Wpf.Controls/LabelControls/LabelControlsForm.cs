namespace Atc.Wpf.Controls.LabelControls;

public class LabelControlsForm : ILabelControlsForm
{
    public Orientation ControlsOrientation { get; set; } = Orientation.Vertical;

    public int ControlsWidth { get; set; } = 300;

    public ICollection<ILabelControlsFormColumn>? Columns { get; set; }

    public void AddColumn(
        IList<ILabelControlBase> labelControls)
    {
        ArgumentNullException.ThrowIfNull(labelControls);

        var labelControlsFormColumn = new LabelControlsFormColumn(labelControls)
        {
            ControlOrientation = ControlsOrientation,
            ControlWidth = ControlsWidth,
        };

        AddColumn(labelControlsFormColumn);
    }

    public void AddColumn(
        ILabelControlsFormColumn labelControlsFormColumn)
    {
        Columns ??= new List<ILabelControlsFormColumn>();

        Columns.Add(labelControlsFormColumn);
    }

    public void Clear()
    {
        Columns = new List<ILabelControlsFormColumn>();
    }

    public int GetMaxHeight()
        => Columns is null
            ? 0
            : Columns
                .Select(x => x.CalculateHeight())
                .Prepend(0)
                .Max();

    public int GetMaxWidth()
    {
        if (Columns is null)
        {
            return 0;
        }

        return Columns.Count * ControlsWidth;
    }

    public Panel GeneratePanel()
    {
        var stackPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
        };

        if (Columns is null)
        {
            return stackPanel;
        }

        foreach (var column in Columns)
        {
            stackPanel.Children.Add(column.GeneratePanel());
        }

        return stackPanel;
    }

    public bool IsValid()
    {
        if (Columns is null)
        {
            return true;
        }

        var isAllValid = true;
        foreach (var column in Columns)
        {
            if (!column.IsValid())
            {
                isAllValid = false;
            }
        }

        return isAllValid;
    }

    public Dictionary<string, object> GetKeyValues()
    {
        if (Columns is null)
        {
            return new Dictionary<string, object>(StringComparer.Ordinal);
        }

        var result = new Dictionary<string, object>(StringComparer.Ordinal);
        foreach (var column in Columns)
        {
            foreach (var item in column.GetKeyValues())
            {
                result.Add(item.Key, item.Value);
            }
        }

        return result;
    }
}