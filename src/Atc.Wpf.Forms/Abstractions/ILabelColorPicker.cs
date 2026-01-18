namespace Atc.Wpf.Forms.Abstractions;

public interface ILabelColorPicker : ILabelControl
{
    Color? ColorValue { get; set; }

    SolidColorBrush? BrushValue { get; set; }
}