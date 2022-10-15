namespace Atc.Wpf.Controls.LabelControls.Abstractions;

public interface ILabelPixelSizeBox : ILabelIntegerNumberControl
{
    int ValueWidth { get; set; }

    int ValueHeight { get; set; }
}