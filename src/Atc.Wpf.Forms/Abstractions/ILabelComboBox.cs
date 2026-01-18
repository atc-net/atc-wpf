namespace Atc.Wpf.Forms.Abstractions;

public interface ILabelComboBox : ILabelComboBoxBase
{
    Dictionary<string, string> Items { get; set; }
}