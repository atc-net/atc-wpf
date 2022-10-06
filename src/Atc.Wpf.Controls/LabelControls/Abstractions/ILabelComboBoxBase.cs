namespace Atc.Wpf.Controls.LabelControls.Abstractions;

public interface ILabelComboBoxBase : ILabelControlBase
{
    string SelectedKey { get; set; }

    static event EventHandler<SelectedKeyEventArgs>? SelectedKeyChanged;
}