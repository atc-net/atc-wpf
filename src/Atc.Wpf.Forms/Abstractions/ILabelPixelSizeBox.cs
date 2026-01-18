namespace Atc.Wpf.Forms.Abstractions;

public interface ILabelPixelSizeBox : ILabelIntegerNumberControl
{
    int ValueWidth { get; set; }

    int ValueHeight { get; set; }
}