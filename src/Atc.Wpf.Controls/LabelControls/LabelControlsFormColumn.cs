namespace Atc.Wpf.Controls.LabelControls;

public class LabelControlsFormColumn : ILabelControlsFormColumn
{
    private const int LabelControlsHeightForVertical = 103;
    private const int LabelControlsHeightForHorizontal = 77;

    public LabelControlsFormColumn(
        IReadOnlyList<ILabelControlBase> labelControls)
    {
        ArgumentNullException.ThrowIfNull(labelControls);

        this.LabelControls = labelControls;
    }

    public Orientation ControlOrientation { get; set; } = Orientation.Vertical;

    public int ControlWidth { get; set; } = 300;

    public IReadOnlyList<ILabelControlBase> LabelControls { get; }

    public int CalculateHeight()
        => ControlOrientation == Orientation.Vertical
            ? LabelControls.Sum(_ => LabelControlsHeightForVertical)
            : LabelControls.Sum(_ => LabelControlsHeightForHorizontal);

    public Panel GeneratePanel()
    {
        var stackPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
        };

        foreach (var labelControl in LabelControls)
        {
            labelControl.Orientation = ControlOrientation;
            labelControl.Width = ControlWidth;
            stackPanel.Children.Add((UIElement)labelControl);
        }

        return stackPanel;
    }

    public bool IsValid()
    {
        var isAllValid = true;
        foreach (var labelControl in LabelControls)
        {
            if (!labelControl.IsValid())
            {
                isAllValid = false;
            }
        }

        return isAllValid;
    }
}