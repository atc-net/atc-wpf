namespace Atc.Wpf.Controls.LabelControls.Abstractions;

public interface ILabelControlsFormColumn
{
    public bool UseGroupBox { get; set; }

    Orientation ControlOrientation { get; set; }

    int ControlWidth { get; set; }

    IList<ILabelControlBase> LabelControls { get; }

    void SetSettings(
        bool useGroupBox,
        Orientation controlOrientation,
        int controlWidth);

    bool HasMultiGroupIdentifiers();

    IList<string?> GetGroupIdentifiers();

    IList<ILabelControlBase> GetLabelControlsByGroupIdentifier(string? groupIdentifier);

    int CalculateHeight();

    Panel GeneratePanel();

    bool IsValid();

    Dictionary<string, object> GetKeyValues();
}