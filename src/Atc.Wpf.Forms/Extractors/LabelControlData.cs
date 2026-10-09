namespace Atc.Wpf.Forms.Extractors;

/// <summary>
/// Describes a label control to create: its data type, label, value and constraints.
/// </summary>
public sealed class LabelControlData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LabelControlData"/> class.
    /// </summary>
    /// <param name="dataType">The data type, which decides the kind of control created.</param>
    /// <param name="labelText">The label text.</param>
    public LabelControlData(
        Type dataType,
        string labelText)
    {
        this.DataType = dataType;
        this.LabelText = labelText;
    }

    /// <summary>
    /// Gets the data type, which decides the kind of control created.
    /// </summary>
    public Type DataType { get; }

    /// <summary>
    /// Gets the label text.
    /// </summary>
    public string LabelText { get; }

    /// <summary>
    /// Gets or sets the watermark text.
    /// </summary>
    public string? WatermarkText { get; set; }

    /// <summary>
    /// Gets or sets the initial value.
    /// </summary>
    public object? Value { get; set; }

    /// <summary>
    /// Gets or sets the second value for two-part controls, such as the Y value of an XY box or the height of a pixel size box.
    /// </summary>
    public object? Value2 { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the control is read-only.
    /// </summary>
    public bool IsReadOnly { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a value is required.
    /// </summary>
    public bool IsMandatory { get; set; }

    /// <summary>
    /// Gets or sets the minimum allowed numeric value.
    /// </summary>
    public decimal? Minimum { get; set; }

    /// <summary>
    /// Gets or sets the maximum allowed numeric value.
    /// </summary>
    public decimal? Maximum { get; set; }

    /// <summary>
    /// Gets or sets the minimum allowed date and time.
    /// </summary>
    public DateTime? MinimumDateTime { get; set; }

    /// <summary>
    /// Gets or sets the maximum allowed date and time.
    /// </summary>
    public DateTime? MaximumDateTime { get; set; }

    /// <summary>
    /// Gets or sets the regular expression the value must match.
    /// </summary>
    public string? RegexPattern { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(DataType)}: {DataType}, {nameof(LabelText)}: {LabelText}, {nameof(WatermarkText)}: {WatermarkText}, {nameof(Value)}: {Value}, {nameof(IsReadOnly)}: {IsReadOnly}, {nameof(IsMandatory)}: {IsMandatory}, {nameof(Minimum)}: {Minimum}, {nameof(Maximum)}: {Maximum}, {nameof(RegexPattern)}: {RegexPattern}";
}