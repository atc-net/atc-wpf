namespace Atc.Wpf.Controls.LabelControls.Abstractions;

public interface ILabelSlider : ILabelControl
{
    int Maximum { get; set; }

    int Minimum { get; set; }

    int Value { get; set; }

    AutoToolTipPlacement AutoToolTipPlacement { get; set; }

    int TickFrequency { get; set; }

    TickPlacement TickPlacement { get; set; }
}