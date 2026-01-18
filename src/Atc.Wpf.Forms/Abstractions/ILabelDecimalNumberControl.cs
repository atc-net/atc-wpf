namespace Atc.Wpf.Forms.Abstractions;

public interface ILabelDecimalNumberControl : ILabelNumberControl
{
    decimal Maximum { get; set; }

    decimal Minimum { get; set; }
}