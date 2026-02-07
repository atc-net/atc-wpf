namespace Atc.Wpf.Forms.Abstractions;

public interface ILabelToggleSwitch : ILabelControlBase
{
    FlowDirection ContentDirection { get; set; }

    string? OffText { get; set; }

    string? OnText { get; set; }

    string? OffOnContent { get; set; }

    bool IsOn { get; set; }
}