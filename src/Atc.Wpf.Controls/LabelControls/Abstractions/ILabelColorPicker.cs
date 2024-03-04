namespace Atc.Wpf.Controls.LabelControls.Abstractions;

public interface ILabelColorPicker : ILabelControl
{
    Color? ColorValue { get; set; }

    SolidColorBrush? BrushValue { get; set; }
}