namespace Atc.Wpf.Sample.SamplesWpfForms.LabelControls;

public partial class LabelMultiSelectComboBoxView
{
    public LabelMultiSelectComboBoxView()
    {
        InitializeComponent();
        DataContext = new LabelControlDemoViewModel();

        InteractiveLabelMultiSelectComboBox.SelectedKeysChanged += (_, e) => Log("SelectedKeysChanged", e.NewValue);
        InteractiveLabelMultiSelectComboBox.SelectedKeysChangedInvalid += (_, e) => Log("SelectedKeysChangedInvalid", e.NewValue);
    }

    private void Log(
        string eventName,
        IReadOnlyList<string> keys)
    {
        var line = string.Create(CultureInfo.InvariantCulture, $"{eventName}: [{string.Join(", ", keys)}]");
        EventLog.Text = EventLog.Text.Length == 0
            ? line
            : EventLog.Text + Environment.NewLine + line;
    }
}