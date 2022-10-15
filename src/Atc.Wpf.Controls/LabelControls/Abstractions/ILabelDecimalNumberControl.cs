namespace Atc.Wpf.Controls.LabelControls.Abstractions;

public interface ILabelDecimalNumberControl : ILabelNumberControl
{
    decimal Maximum { get; set; }

    decimal Minimum { get; set; }
}