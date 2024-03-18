namespace Atc.Wpf.Controls.Dialogs;

/// <summary>
/// Interaction logic for BasicApplicationSettingsDialogBox.
/// </summary>
public partial class BasicApplicationSettingsDialogBox
{
    public BasicApplicationSettingsDialogBox()
    {
        InitializeComponent();
    }

    public string GetDataAsJson()
        => DataContext is BasicApplicationSettingsDialogBoxViewModel vm
            ? vm.ToJson()
            : "{}";
}