namespace Atc.Wpf.Controls.LabelControls;

public class LabelInputFormPanelSettings
{
    public Size MaxSize { get; set; } = new(1920, 1200);

    public Orientation ControlOrientation { get; set; } = Orientation.Vertical;

    public bool UseGroupBox { get; set; }

    public int ControlWidth { get; set; } = 320;

    public override string ToString()
        => $"{nameof(MaxSize)}: {MaxSize}, {nameof(ControlOrientation)}: {ControlOrientation}, {nameof(UseGroupBox)}: {UseGroupBox}, {nameof(ControlWidth)}: {ControlWidth}";
}