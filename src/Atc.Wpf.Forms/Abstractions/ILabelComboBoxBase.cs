namespace Atc.Wpf.Forms.Abstractions;

public interface ILabelComboBoxBase : ILabelControlBase
{
    string SelectedKey { get; set; }

    static event EventHandler<ValueChangedEventArgs<string?>>? SelectedKeyChanged;
}