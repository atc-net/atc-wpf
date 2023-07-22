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

    public object? Value { get; set; }

    public object? Value2 { get; set; }

    public bool IsReadOnly { get; set; }

    public bool IsMandatory { get; set; }

    public decimal? Minimum { get; set; }

    public decimal? Maximum { get; set; }

    public string? RegexPattern { get; set; }

    public override string ToString()
        => $"{nameof(DataType)}: {DataType}, {nameof(LabelText)}: {LabelText}, {nameof(WatermarkText)}: {WatermarkText}, {nameof(Value)}: {Value}, {nameof(IsReadOnly)}: {IsReadOnly}, {nameof(IsMandatory)}: {IsMandatory}, {nameof(Minimum)}: {Minimum}, {nameof(Maximum)}: {Maximum}, {nameof(RegexPattern)}: {RegexPattern}";
}