namespace Atc.Wpf.Controls.LabelControls.Abstractions;

public interface ILabelControlsForm
{
    IList<ILabelControlsFormRow>? Rows { get; set; }

    void AddColumn(IList<ILabelControlBase> labelControls);

    void AddColumn(ILabelControlsFormColumn labelControlsFormColumn);

    void Clear();

    bool HasMultiGroupIdentifiers();

    int GetMaxHeight();

    int GetMaxWidth();

    Panel GeneratePanel();

    bool IsValid();

    Dictionary<string, object> GetKeyValues();
}