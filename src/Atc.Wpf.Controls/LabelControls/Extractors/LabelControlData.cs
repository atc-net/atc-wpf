namespace Atc.Wpf.Controls.LabelControls.Extractors;

public class LabelControlData
{
    public LabelControlData(
        Type dataType,
        string labelText)
    {
        this.DataType = dataType;
        this.LabelText = labelText;
    }

    public Type DataType { get; }

    public string LabelText { get; }

    public string? WatermarkText { get; set; }

    public object? ValueText { get; set; }

    public bool IsReadOnly { get; set; }

    public bool IsMandatory { get; set; }

    public uint? Minimum { get; set; }

    public uint? Maximum { get; set; }

    public string? RegexPattern { get; set; }
}