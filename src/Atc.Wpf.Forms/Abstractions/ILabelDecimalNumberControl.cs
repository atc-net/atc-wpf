namespace Atc.Wpf.Forms.Abstractions;

public interface ILabelDecimalNumberControl : ILabelNumberControl
{
    int DecimalPlaces { get; set; }

    decimal Maximum { get; set; }

    decimal Minimum { get; set; }
}