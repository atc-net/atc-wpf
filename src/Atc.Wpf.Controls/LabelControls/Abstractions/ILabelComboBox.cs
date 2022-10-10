namespace Atc.Wpf.Controls.LabelControls.Abstractions;

public interface ILabelComboBox : ILabelComboBoxBase
{
    Dictionary<string, string> Items { get; set; }

    string SelectedKey { get; set; }
}