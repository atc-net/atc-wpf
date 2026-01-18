namespace Atc.Wpf.Forms.Abstractions;

public interface ILabelIntegerNumberControl : ILabelNumberControl
{
    int Maximum { get; set; }

    int Minimum { get; set; }
}