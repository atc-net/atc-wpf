using Atc.Wpf.Controls.LabelControls.EventArgs;

namespace Atc.Wpf.Controls.LabelControls.Abstractions;

public interface ILabelComboBox : ILabelControlBase
{
    Dictionary<string, string> Items { get; set; }

    string SelectedKey { get; set; }

    static event EventHandler<SelectedKeyEventArgs>? SelectedKeyChanged;
}