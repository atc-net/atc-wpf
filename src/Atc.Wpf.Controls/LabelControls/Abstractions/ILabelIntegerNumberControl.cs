namespace Atc.Wpf.Controls.LabelControls.Abstractions;

public interface ILabelIntegerNumberControl : ILabelNumberControl
{
    int Maximum { get; set; }

    int Minimum { get; set; }
}